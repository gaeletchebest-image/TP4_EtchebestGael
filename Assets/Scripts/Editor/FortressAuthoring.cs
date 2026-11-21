using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace UltimaGuardia.Editor
{
    // Explicit, editable polygon source. Importing this file never rebuilds the scene.
    // Metres, Y up, ground pivot. Meshes are combined by material inside each module.
    public static class FortressAuthoring
    {
        public const string Root = "Arena_Fortress_H07";
        public const string Models = "Assets/Models/H07";
        public const string Prefabs = "Assets/Prefabs/H07";
        public const string Evidence = "Documentation/Evidence/H07";
        static readonly string[] Names = { "Limestone", "StoneLight", "StoneDark", "Oak", "Iron", "BannerBlue", "Bronze", "Ember" };
        static readonly Color[] Colors = {
            new Color(.38f,.405f,.43f), new Color(.51f,.53f,.54f), new Color(.255f,.28f,.31f),
            new Color(.26f,.145f,.075f), new Color(.11f,.14f,.18f), new Color(.055f,.19f,.32f),
            new Color(.63f,.43f,.18f), new Color(1,.45f,.12f) };
        static Material[] materials;
        static readonly Dictionary<string, GameObject> modules = new Dictionary<string, GameObject>();
        static Vector3 V(float x, float y, float z) => new Vector3(x,y,z);

        [MenuItem("Ultima Guardia/H07/Construir fortaleza e integrar")]
        public static void BuildAndIntegrate()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (EditorApplication.isPlayingOrWillChangePlaymode || scene.path != "Assets/Scenes/Combat.unity" || scene.isDirty)
                throw new InvalidOperationException("Abrir y guardar Combat, detener Play antes de integrar H07.");
            if (Object.FindObjectsByType<CombatantView>(FindObjectsSortMode.None).Length != 4)
                throw new InvalidOperationException("Combat debe conservar sus cuatro combatientes.");
            foreach (var folder in new[] { Models+"/Meshes", Models+"/Exports", Prefabs, "Assets/Materials/H07", Evidence }) Directory.CreateDirectory(folder);
            materials = new Material[Names.Length]; modules.Clear();
            for (int i=0;i<Names.Length;i++)
            {
                string path="Assets/Materials/H07/"+Names[i]+".mat";
                var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!mat) { mat=new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(mat,path); }
                mat.color=Colors[i]; mat.SetFloat("_Smoothness", i==4 || i==6 ? .35f : .12f);
                mat.SetFloat("_Metallic", i==4 || i==6 ? .65f : 0);
                if (i==7) { mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor",Colors[i]*1.5f); }
                EditorUtility.SetDirty(mat); materials[i]=mat;
            }
            Floor(); Wall(); Crown(); Gate(); Pier(); Banner(); Brazier();
            // Build resources first; only then replace the environment root, never characters/UI.
            foreach (string name in new[] { "Arena_Blockout_Provisional", Root })
            { var old=GameObject.Find(name); if(old) Object.DestroyImmediate(old); }
            var root=new GameObject(Root).transform;
            for(int x=-3;x<=3;x++) for(int z=-2;z<=2;z++) Place(root,"Paving",V(x*3,0,2+z*3));
            foreach(float side in new[]{-1f,1f})
            {
                for(int i=0;i<3;i++)
                {
                    float x=side*(3.65f+i*2.4f);
                    Place(root,"CurtainWall",V(x,0,6.8f));
                    Place(root,"Battlement",V(x,4.8f,6.8f));
                }
                Place(root,"Buttress",V(side*2.6f,0,6.35f));
                Place(root,"Buttress",V(side*8.45f,0,6.35f));
                Place(root,"Standard",V(side*5.5f,2.4f,6.11f));
                Place(root,"Brazier",V(side*3.25f,0,5.1f));
            }
            Place(root,"Gatehouse",V(0,0,6.8f));
            var floor=root.gameObject.AddComponent<BoxCollider>(); floor.center=V(0,-.16f,2); floor.size=V(21,.32f,15);
            var key=GameObject.Find("Directional Light").GetComponent<Light>();
            key.type=LightType.Directional; key.intensity=1.65f; key.color=new Color(1,.91f,.79f);
            key.transform.rotation=Quaternion.Euler(43,-32,0); key.shadows=LightShadows.Soft; key.shadowStrength=.72f;
            var fillObject=GameObject.Find("H07_SkyFill"); if(fillObject) Object.DestroyImmediate(fillObject);
            var fill=new GameObject("H07_SkyFill",typeof(Light)).GetComponent<Light>();
            fill.type=LightType.Directional; fill.intensity=.55f; fill.color=new Color(.65f,.78f,1);
            fill.transform.rotation=Quaternion.Euler(28,145,0); fill.shadows=LightShadows.None;
            RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.42f,.49f,.61f);
            RenderSettings.ambientEquatorColor=new Color(.27f,.30f,.36f);
            RenderSettings.ambientGroundColor=new Color(.16f,.17f,.20f);
            RenderSettings.fog=false;
            var camera=Camera.main; camera.farClipPlane=60;
            camera.backgroundColor=new Color(.13f,.18f,.25f);
            // H05 framing is retained: full HUD and all status rows already have space.
            PlayerSettings.bundleVersion="0.0.8";
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh(); RemapExports();
            Debug.Log("H07: siete módulos propios integrados; personajes, reglas, anclajes y Canvas conservados.");
        }

        static void Place(Transform parent,string id,Vector3 position)
        {
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(modules[id],parent);
            instance.transform.localPosition=position; instance.isStatic=true;
        }
        public static void RemapExports()
        {
            foreach(string path in Directory.GetFiles(Models+"/Exports","*.obj"))
            {
                string assetPath=path.Replace('\\','/');
                var importer=(ModelImporter)AssetImporter.GetAtPath(assetPath);
                importer.importNormals=ModelImporterNormals.Import;
                foreach(string name in Names)
                {
                    var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/H07/"+name+".mat");
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),name),material);
                }
                importer.SaveAndReimport();
            }
        }
        static void Save(string name,Geometry geometry)
        {
            var mesh=geometry.Mesh(name); string path=Models+"/Meshes/"+name+".asset";
            var previous=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(previous) { EditorUtility.CopySerialized(mesh,previous); Object.DestroyImmediate(mesh); mesh=previous; }
            else AssetDatabase.CreateAsset(mesh,path);
            var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));
            go.GetComponent<MeshFilter>().sharedMesh=mesh;
            go.GetComponent<MeshRenderer>().sharedMaterials=geometry.UsedMaterials.Select(i=>materials[i]).ToArray();
            go.isStatic=true;
            modules[name]=PrefabUtility.SaveAsPrefabAsset(go,Prefabs+"/"+name+".prefab");
            Object.DestroyImmediate(go);
            Export(mesh,geometry.UsedMaterials);
        }
        static void Floor()
        {
            var g=new Geometry();
            g.Box(V(0,-.19f,0),V(3,.26f,3),2);
            for(int row=0;row<3;row++) for(int col=0;col<3;col++)
                g.Bevel(V(-1+col,-.065f,-1+row),V(.988f,.13f,.988f),.035f,(row+col)%4==0?0:2);
            Save("Paving",g);
        }
        static void Wall()
        {
            var g=new Geometry();g.Box(V(0,2.4f,.09f),V(2.4f,4.8f,.8f),2);
            for(int row=0;row<8;row++)
            {
                float y=.3f+row*.6f;
                if(row%2==0) for(int col=0;col<2;col++) g.Bevel(V(-.6f+col*1.2f,y,-.18f),V(1.18f,.58f,.6f),.055f,(row+col)%3==0?1:0);
                else { g.Bevel(V(0,y,-.18f),V(1.18f,.58f,.6f),.055f,0);
                    foreach(float side in new[]{-1f,1f}) g.Bevel(V(side*.9f,y,-.18f),V(.58f,.58f,.6f),.045f,1); }
            }
            g.Bevel(V(0,.16f,-.25f),V(2.4f,.32f,1.05f),.045f,1);
            Save("CurtainWall",g);
        }
        static void Crown()
        {
            var g=new Geometry();g.Bevel(V(0,.08f,0),V(2.4f,.16f,1.12f),.04f,1);
            foreach(float x in new[]{-.8f,0,.8f}) g.Bevel(V(x,.43f,0),V(.5f,.56f,.85f),.05f,0);
            Save("Battlement",g);
        }
        static void Gate()
        {
            var g=new Geometry();
            // Closed arched gate, vertical planks. Semicircle springs at 2.6 metres.
            for(int i=0;i<12;i++)
            {
                float x=-1.925f+i*.35f;
                float top=2.6f+Mathf.Sqrt(Mathf.Max(0,2.1f*2.1f-x*x));
                g.Bevel(V(x,top*.5f,.12f),V(.337f,top,.25f),.018f,3);
            }
            foreach(float side in new[]{-1f,1f})
            {
                for(int row=0;row<5;row++) g.Bevel(V(side*2.35f,.26f+row*.52f,-.1f),V(.5f,.51f,1.05f),.045f,row%2);
                foreach(float y in new[]{.65f,1.65f,2.5f})
                {
                    g.Bevel(V(side*1.05f,y,-.055f),V(1.94f,.14f,.09f),.018f,4);
                    foreach(float x in new[]{.28f,1.0f,1.82f}) g.Bevel(V(side*x,y,-.12f),V(.055f,.065f,.035f),.009f,6);
                }
                g.Box(V(side*.16f,1.15f,-.16f),V(.075f,.32f,.09f),6);
            }
            for(int i=0;i<15;i++)
            {
                float a=i*Mathf.PI/15+.009f,b=(i+1)*Mathf.PI/15-.009f;
                var front=new[]{V(2.1f*Mathf.Cos(a),2.6f+2.1f*Mathf.Sin(a),-.64f),V(2.62f*Mathf.Cos(a),2.6f+2.62f*Mathf.Sin(a),-.64f),
                    V(2.62f*Mathf.Cos(b),2.6f+2.62f*Mathf.Sin(b),-.64f),V(2.1f*Mathf.Cos(b),2.6f+2.1f*Mathf.Sin(b),-.64f)};
                g.Extrude(front,1.15f,i%3==0?1:0);
            }
            // The crown intentionally follows the arch; no rectangular block fills the arch opening.
            Save("Gatehouse",g);
        }
        static void Pier()
        {
            var g=new Geometry();g.Bevel(V(0,.18f,-.13f),V(1,.36f,1.65f),.06f,1);
            for(int i=0;i<7;i++) g.Bevel(V(0,.68f+i*.61f,0),V(.75f,.59f,1.28f),.055f,i%3==0?1:0);
            g.Bevel(V(0,4.9f,0),V(1,.3f,1.55f),.06f,1);
            g.Bevel(V(0,5.25f,0),V(.78f,.4f,1.24f),.05f,0);
            Save("Buttress",g);
        }
        static void Banner()
        {
            var g=new Geometry();g.Bevel(V(0,1.15f,-.07f),V(1.24f,.08f,.12f),.02f,6);
            // Two sided folded cloth: central ridge and a pointed lower hem.
            var left=new[]{V(-.52f,1.1f,0),V(-.52f,-.35f,0),V(0,-.65f,-.1f),V(0,1.1f,-.1f)};
            var right=new[]{V(0,1.1f,-.1f),V(0,-.65f,-.1f),V(.52f,-.35f,0),V(.52f,1.1f,0)};
            g.DoubleQuad(left,5);g.DoubleQuad(right,5);
            foreach(float side in new[]{-1f,1f}) g.Box(V(side*.47f,.36f,-.025f),V(.035f,1.4f,.018f),6);
            // Fortress sigil: bronze shield and a pale vertical gate bar.
            var shield=new[]{V(-.23f,.75f,-.12f),V(-.23f,.30f,-.12f),V(0,.10f,-.14f),V(.23f,.30f,-.12f),V(.23f,.75f,-.12f)};
            for(int i=1;i<shield.Length-1;i++) g.Tri(shield[0],shield[i],shield[i+1],6);
            g.Box(V(0,.48f,-.16f),V(.055f,.43f,.018f),1);
            Save("Standard",g);
        }
        static void Brazier()
        {
            var g=new Geometry();g.Bevel(V(0,.09f,0),V(.68f,.18f,.68f),.05f,1);
            g.Bevel(V(0,.65f,0),V(.25f,1.0f,.25f),.025f,4);
            g.Bevel(V(0,1.18f,0),V(.64f,.22f,.64f),.06f,4);
            foreach(float x in new[]{-.18f,0,.18f}) g.Bevel(V(x,1.32f,0),V(.12f,.11f,.25f),.02f,7);
            Save("Brazier",g);
        }

        // Bevelled blocks are authored as six inset faces plus twelve chamfers and eight corners.
        sealed class Geometry
        {
            readonly List<Vector3> vertices=new List<Vector3>(); readonly List<Vector2> uv=new List<Vector2>();
            readonly SortedDictionary<int,List<int>> indices=new SortedDictionary<int,List<int>>();
            public int[] UsedMaterials => indices.Keys.ToArray();
            public void Tri(Vector3 a,Vector3 b,Vector3 c,int material)
            {
                if(!indices.ContainsKey(material)) indices[material]=new List<int>();
                int start=vertices.Count; vertices.AddRange(new[]{a,b,c});
                uv.AddRange(new[]{new Vector2(a.x+a.z,a.y+a.z),new Vector2(b.x+b.z,b.y+b.z),new Vector2(c.x+c.z,c.y+c.z)});
                indices[material].AddRange(new[]{start,start+1,start+2});
            }
            void Face(Vector3[] p,int material,Vector3 outward)
            {
                if(Vector3.Dot(Vector3.Cross(p[1]-p[0],p[2]-p[0]),outward)<0) Array.Reverse(p);
                for(int i=1;i<p.Length-1;i++) Tri(p[0],p[i],p[i+1],material);
            }
            public void DoubleQuad(Vector3[] p,int material) { Face((Vector3[])p.Clone(),material,Vector3.back);Face((Vector3[])p.Clone(),material,Vector3.forward); }
            public void Box(Vector3 center,Vector3 size,int material) => Bevel(center,size,0,material);
            public void Bevel(Vector3 center,Vector3 size,float bevel,int material)
            {
                Vector3 h=size*.5f;float b=Mathf.Min(bevel,Mathf.Min(h.x,Mathf.Min(h.y,h.z))*.8f);
                Vector3 Point(int axis,float value,int u,float vu,int v,float vv) { var p=Vector3.zero;p[axis]=value;p[u]=vu;p[v]=vv;return center+p; }
                for(int axis=0;axis<3;axis++) foreach(int sign in new[]{-1,1})
                {
                    int u=(axis+1)%3,v=(axis+2)%3;var normal=Vector3.zero;normal[axis]=sign;
                    Face(new[]{Point(axis,sign*h[axis],u,-h[u]+b,v,-h[v]+b),Point(axis,sign*h[axis],u,h[u]-b,v,-h[v]+b),
                        Point(axis,sign*h[axis],u,h[u]-b,v,h[v]-b),Point(axis,sign*h[axis],u,-h[u]+b,v,h[v]-b)},material,normal);
                }
                if(b<=0)return;
                for(int axis=0;axis<3;axis++)
                {
                    int u=(axis+1)%3,v=(axis+2)%3;
                    foreach(int su in new[]{-1,1}) foreach(int sv in new[]{-1,1})
                    {
                        var a=Point(axis,-h[axis]+b,u,su*h[u],v,sv*(h[v]-b));
                        var d=Point(axis,-h[axis]+b,u,su*(h[u]-b),v,sv*h[v]);
                        var c=d; c[axis]=center[axis]+h[axis]-b;var e=a;e[axis]=center[axis]+h[axis]-b;
                        var n=Vector3.zero;n[u]=su;n[v]=sv;Face(new[]{a,d,c,e},material,n);
                    }
                }
                foreach(int x in new[]{-1,1}) foreach(int y in new[]{-1,1}) foreach(int z in new[]{-1,1})
                    Face(new[]{center+V(x*h.x,y*(h.y-b),z*(h.z-b)),center+V(x*(h.x-b),y*h.y,z*(h.z-b)),
                        center+V(x*(h.x-b),y*(h.y-b),z*h.z)},material,V(x,y,z));
            }
            public void Extrude(Vector3[] front,float depth,int material)
            {
                var back=front.Select(p=>p+Vector3.forward*depth).ToArray();
                Face((Vector3[])front.Clone(),material,Vector3.back);Face((Vector3[])back.Clone(),material,Vector3.forward);
                var center=front.Aggregate(Vector3.zero,(s,p)=>s+p)/front.Length;
                for(int i=0;i<front.Length;i++) { int j=(i+1)%front.Length;Face(new[]{front[i],front[j],back[j],back[i]},material,(front[i]+front[j])*.5f-center); }
            }
            public Mesh Mesh(string name)
            {
                var mesh=new Mesh{name=name,indexFormat=IndexFormat.UInt32};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.subMeshCount=indices.Count;
                int sub=0;foreach(var pair in indices)mesh.SetTriangles(pair.Value,sub++);
                mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
            }
        }
        static void Export(Mesh mesh,int[] slots)
        {
            var inv=CultureInfo.InvariantCulture;var obj=new StringBuilder("# Ultima Guardia H07; metres; editable source FortressAuthoring.cs\nmtllib "+mesh.name+".mtl\n");
            foreach(var v in mesh.vertices)obj.AppendFormat(inv,"v {0} {1} {2}\n",-v.x,v.y,v.z);
            foreach(var v in mesh.uv)obj.AppendFormat(inv,"vt {0} {1}\n",v.x,v.y);
            foreach(var n in mesh.normals)obj.AppendFormat(inv,"vn {0} {1} {2}\n",-n.x,n.y,n.z);
            var mtl=new StringBuilder();
            for(int sub=0;sub<slots.Length;sub++)
            {
                int slot=slots[sub];obj.Append("usemtl "+Names[slot]+"\ng "+Names[slot]+"\n");
                var t=mesh.GetTriangles(sub);for(int i=0;i<t.Length;i+=3) { var ids=new[]{t[i]+1,t[i+2]+1,t[i+1]+1};obj.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",ids[0],ids[1],ids[2]); }
                var c=Colors[slot];mtl.AppendFormat(inv,"newmtl {0}\nKd {1} {2} {3}\nd 1\n",Names[slot],c.r,c.g,c.b);
            }
            File.WriteAllText(Models+"/Exports/"+mesh.name+".obj",obj.ToString());File.WriteAllText(Models+"/Exports/"+mesh.name+".mtl",mtl.ToString());
        }
    }
}
