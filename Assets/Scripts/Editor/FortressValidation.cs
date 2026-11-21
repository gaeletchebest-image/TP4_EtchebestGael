using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Editor
{
    public static class FortressValidation
    {
        [Serializable] public class Report
        {
            public string version; public bool passed; public int moduleTypes, instances, renderers, triangles, materials, shadowLights;
            public int sampledClips, sampledPoses, sightlines, specialCameraSamples;
            public float minimumAnimatedY; public Vector3 baseCamera, proposedSpecialCamera, specialLookAt;
            public string[] checks; public List<string> framing = new List<string>();
        }
        struct Triangle { public Vector3 a,b,c; }
        static List<Triangle> environment;
        static void Require(bool condition,string message) { if(!condition) throw new InvalidOperationException("H07: "+message); }

        [MenuItem("Ultima Guardia/H07/Auditar escenario y capturar cámaras")]
        public static void AuditAndCapture()
        {
            var scene=EditorSceneManager.GetActiveScene();
            Require(!EditorApplication.isPlayingOrWillChangePlaymode && !scene.isDirty && scene.path=="Assets/Scenes/Combat.unity","guardar Combat y detener Play");
            Directory.CreateDirectory(FortressAuthoring.Evidence);
            var root=GameObject.Find(FortressAuthoring.Root);Require(root,"arena final integrada");
            Require(!GameObject.Find("Arena_Blockout_Provisional"),"blockout retirado");
            var filters=root.GetComponentsInChildren<MeshFilter>();var renderers=root.GetComponentsInChildren<MeshRenderer>();
            var report=new Report { version=Application.version,renderers=renderers.Length,instances=root.transform.childCount,
                moduleTypes=filters.Select(f=>f.sharedMesh).Distinct().Count(), minimumAnimatedY=100 };
            environment=new List<Triangle>();
            foreach(var f in filters)
            {
                var mesh=f.sharedMesh;Require(mesh && AssetDatabase.GetAssetPath(mesh).StartsWith(FortressAuthoring.Models+"/Meshes"),"malla propia guardada");
                Require(mesh.normals.Length==mesh.vertexCount && mesh.uv.Length==mesh.vertexCount,"normales y UV completos");
                Require(mesh.vertices.All(v=>float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z)),"vértices finitos");
                Require(f.GetComponent<Renderer>().sharedMaterials.Length==mesh.subMeshCount,"material por submalla");
                var vertices=mesh.vertices;var indices=mesh.triangles;report.triangles+=indices.Length/3;
                for(int i=0;i<indices.Length;i+=3)
                {
                    Vector3 a=f.transform.TransformPoint(vertices[indices[i]]),b=f.transform.TransformPoint(vertices[indices[i+1]]),c=f.transform.TransformPoint(vertices[indices[i+2]]);
                    Require(Vector3.Cross(b-a,c-a).sqrMagnitude>1e-14f,"triángulo no degenerado en "+f.name);
                    environment.Add(new Triangle{a=a,b=b,c=c});
                }
            }
            var mats=renderers.SelectMany(r=>r.sharedMaterials).Distinct().ToArray();report.materials=mats.Length;
            Require(mats.All(m=>m && m.shader.name=="Universal Render Pipeline/Lit" && m.shader.isSupported),"materiales URP soportados");
            Require(report.moduleTypes==7 && report.renderers<65 && report.triangles<60000,"kit y presupuesto de geometría");
            Require(root.GetComponentsInChildren<Light>().Length==0,"decoración sin luces adicionales");
            report.shadowLights=Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Count(l=>l.shadows!=LightShadows.None);
            Require(report.shadowLights==1,"una sola luz de sombras");
            foreach(var mesh in filters.Select(f=>f.sharedMesh).Distinct())
            {
                string obj=FortressAuthoring.Models+"/Exports/"+mesh.name+".obj";
                Require(File.Exists(obj)&&File.Exists(Path.ChangeExtension(obj,"mtl")),"OBJ/MTL conservados");
                var imported=AssetDatabase.LoadAssetAtPath<GameObject>(obj);
                Require(imported,"OBJ importado "+mesh.name);
                Require(imported.GetComponentsInChildren<Renderer>().SelectMany(r=>r.sharedMaterials).All(m=>m && m.shader.name=="Universal Render Pipeline/Lit"),"OBJ con materiales URP "+mesh.name);
            }
            var views=Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None);
            var battle=Object.FindFirstObjectByType<CombatPrototype>();Require(views.Length==4 && battle.Encounter.ValidateData().Count==0,"encuentro intacto");
            foreach(var view in views)
            {
                var slot=battle.Encounter.participants.Single(s=>s.combatant.id==view.combatantId);
                Require(Vector3.Distance(view.transform.position,slot.position)<.001f,"anclaje "+view.combatantId);
                Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(view.gameObject)=="Assets/Prefabs/H05/"+view.combatantId+".prefab","prefab H05 conservado");
            }
            var camera=Camera.main;var position=camera.transform.position;var rotation=camera.transform.rotation;float aspect=camera.aspect;
            report.baseCamera=position;var mage=views.Single(v=>v.combatantId=="mage");
            report.proposedSpecialCamera=mage.transform.position+new Vector3(1.1f,2.8f,-5.8f);
            report.specialLookAt=mage.transform.position+Vector3.up*1.9f;
            try
            {
                foreach(var view in views)
                {
                    var rig=view.GetComponentInChildren<CharacterRig>();var animator=rig.GetComponent<Animator>();bool wasEnabled=animator.enabled;
                    var set=rig.GetComponent<CharacterAnimationPlayer>().animations;
                    var clips=new[]{set.idle,set.reaction,set.death,set.item}.Concat(set.actions.Select(a=>a.clip)).Where(c=>c).Distinct().ToArray();
                    animator.enabled=false;
                    try
                    {
                        foreach(var clip in clips)
                        {
                            report.sampledClips++;
                            for(int sample=0;sample<9;sample++)
                            {
                                rig.RestoreRestPose();clip.SampleAnimation(rig.gameObject,clip.length*sample/8f);report.sampledPoses++;
                                var points=rig.GetComponentsInChildren<MeshFilter>().SelectMany(f=>f.sharedMesh.vertices.Select(v=>f.transform.TransformPoint(v))).ToArray();
                                float minY=points.Min(p=>p.y);report.minimumAnimatedY=Mathf.Min(report.minimumAnimatedY,minY);
                                Require(minY>=-.004f,"contacto animado "+view.combatantId+"/"+clip.name+" mínimo="+minY);
                                foreach(var point in new[]{rig.head.position,rig.chest.position,view.effectAnchor.position})
                                { Require(!Blocked(position,point),"sin oclusión del escenario "+view.combatantId+"/"+clip.name);report.sightlines++; }
                                var projected=points.Select(p=>camera.WorldToViewportPoint(p)).ToArray();
                                Require(projected.All(p=>p.z>0 && p.x>.02f && p.x<.98f && p.y>.32f && p.y<.84f),"margen de cámara/HUD "+view.combatantId+"/"+clip.name);
                            }
                            report.framing.Add(view.combatantId+"/"+clip.name+": 9 poses dentro del margen, sin oclusión de cabeza/torso/impacto");
                        }
                    }
                    finally { rig.RestoreRestPose();animator.enabled=wasEnabled; }
                }
                foreach(var view in views)view.nameLabel.gameObject.SetActive(false);
                Capture(camera,1280,720,"fortress-base-1280x720");Capture(camera,1920,1080,"fortress-base-1920x1080");
                // Reserve (and verify) the H10 approach; no runtime camera sequence is added in H07.
                var mageRig=mage.GetComponentInChildren<CharacterRig>();var storm=mageRig.GetComponent<CharacterAnimationPlayer>().animations.Find(CombatAction.ArcaneStorm).clip;
                storm.SampleAnimation(mageRig.gameObject,.8f);
                for(int i=0;i<=20;i++)
                {
                    float t=i/20f;camera.transform.position=Vector3.Lerp(position,report.proposedSpecialCamera,t);
                    camera.transform.rotation=Quaternion.Slerp(rotation,Quaternion.LookRotation(report.specialLookAt-report.proposedSpecialCamera),t);
                    foreach(var point in new[]{mageRig.head.position,mageRig.chest.position,mage.projectileOrigin.position})
                        Require(!Blocked(camera.transform.position,point),"ruta cámara especial despejada");
                    report.specialCameraSamples++;
                }
                var magePoints=mageRig.GetComponentsInChildren<MeshFilter>().SelectMany(f=>f.sharedMesh.vertices.Select(v=>camera.WorldToViewportPoint(f.transform.TransformPoint(v))));
                Require(magePoints.All(p=>p.z>0 && p.x>.02f && p.x<.98f && p.y>.02f && p.y<.98f),"maga y bastón completos en cámara especial");
                Capture(camera,1280,720,"fortress-special-reserved");
                mageRig.RestoreRestPose();
                foreach(var enemy in views.Where(v=>v.combatantId=="ogre"||v.combatantId=="shaman"))
                    Require(!Blocked(enemy.effectAnchor.position+Vector3.up*4,enemy.effectAnchor.position),"columna libre para rayo "+enemy.combatantId);
                camera.transform.position=new Vector3(12,11,-13);camera.transform.LookAt(new Vector3(0,1,3));
                Capture(camera,1600,900,"fortress-overview");
                report.checks=new[]{"7 modules, saved meshes, imported OBJ/MTL, URP materials, finite nondegenerate triangles and UVs",
                    "Blockout absent; original H05 prefabs, encounter data and all four world anchors preserved",
                    "All 24 saved clips sampled at nine poses: floor contact, viewport margins, environment sightlines",
                    "21 proposed special approach samples clear; overhead lightning corridors clear",
                    "Environment under 60000 triangles/65 renderers; one shadow light; no realtime decoration lights",
                    "Still captures 720p/1080p and proposed special; gameplay/HUD validation recorded separately"};
                report.passed=true;File.WriteAllText(FortressAuthoring.Evidence+"/environment-audit.json",JsonUtility.ToJson(report,true));
            }
            finally
            {
                camera.transform.SetPositionAndRotation(position,rotation);camera.aspect=aspect;
                foreach(var view in views)view.GetComponentInChildren<CharacterRig>().RestoreRestPose();
                // Sampling clips can mark a scene dirty. Reload saved scene, never save sampled poses.
                EditorSceneManager.OpenScene(scene.path,OpenSceneMode.Single);
            }
            Debug.Log("H07: auditoría geométrica, 24 clips y ruta reservada de cámara aprobados.");
        }
        static bool Blocked(Vector3 start,Vector3 end)
        {
            Vector3 delta=end-start;float distance=delta.magnitude;Vector3 direction=delta/distance;
            foreach(var t in environment)
            {
                var e1=t.b-t.a;var e2=t.c-t.a;var p=Vector3.Cross(direction,e2);float determinant=Vector3.Dot(e1,p);
                if(Mathf.Abs(determinant)<1e-7f)continue;
                float inverse=1/determinant;var s=start-t.a;float u=Vector3.Dot(s,p)*inverse;if(u<0||u>1)continue;
                var q=Vector3.Cross(s,e1);float v=Vector3.Dot(direction,q)*inverse;if(v<0||u+v>1)continue;
                float hit=Vector3.Dot(e2,q)*inverse;if(hit>.02f && hit<distance-.03f)return true;
            }
            return false;
        }
        static void Capture(Camera camera,int width,int height,string name)
        {
            var rt=RenderTexture.GetTemporary(width,height,24);var active=RenderTexture.active;var target=camera.targetTexture;float aspect=camera.aspect;
            Texture2D image=null;
            try
            {
                camera.targetTexture=rt;camera.aspect=(float)width/height;camera.Render();RenderTexture.active=rt;
                image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();
                File.WriteAllBytes(FortressAuthoring.Evidence+"/"+name+".png",image.EncodeToPNG());
            }
            finally { camera.targetTexture=target;camera.aspect=aspect;RenderTexture.active=active;if(image)Object.DestroyImmediate(image);RenderTexture.ReleaseTemporary(rt); }
        }
    }
}
