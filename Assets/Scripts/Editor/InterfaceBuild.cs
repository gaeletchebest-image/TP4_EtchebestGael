using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UltimaGuardia.Editor
{
    public static class InterfaceBuild
    {
        [Serializable] private sealed class Summary
        {
            public string version, result, unity, target, output, startedUtc, finishedUtc;
            public string[] scenes, messages;
            public double durationSeconds;
            public ulong totalBytes;
            public int errors, warnings;
        }
        [MenuItem("Ultima Guardia/H08/Compilar build de revisión")]
        public static void BuildReview()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de compilar.");
            string[] scenes = { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Combat.unity" };
            Directory.CreateDirectory("Builds/H08");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes, locationPathName = "Builds/H08/UltimaGuardia.exe", target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.DetailedBuildReport
            });
            var s = report.summary;
            var summary = new Summary
            {
                version = PlayerSettings.bundleVersion + "-H08", unity = Application.unityVersion, result = s.result.ToString(),
                target = s.platform.ToString(), output = s.outputPath, scenes = scenes,
                startedUtc = s.buildStartedAt.ToUniversalTime().ToString("O"), finishedUtc = s.buildEndedAt.ToUniversalTime().ToString("O"),
                durationSeconds = s.totalTime.TotalSeconds, totalBytes = s.totalSize, errors = s.totalErrors, warnings = s.totalWarnings,
                messages = report.steps.SelectMany(step => step.messages.Select(m => m.type + " / " + step.name + ": " + m.content)).ToArray()
            };
            Directory.CreateDirectory("Documentation/Evidence/H08");
            File.WriteAllText("Documentation/Evidence/H08/build-summary.json", JsonUtility.ToJson(summary, true));
            Debug.Log("H08 build: " + summary.result + ", errors=" + summary.errors + ", warnings=" + summary.warnings);
            if (s.result != BuildResult.Succeeded) throw new InvalidOperationException("La build H08 no terminó correctamente.");
        }
    }
}
