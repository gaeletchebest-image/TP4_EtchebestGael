using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UltimaGuardia.Editor
{
    public static class BuildH11
    {
        [Serializable] private sealed class Summary
        {
            public string version, result, unity, output;
            public int errors, warnings;
            public ulong totalBytes;
            public double durationSeconds;
            public string[] messages;
        }

        [MenuItem("Ultima Guardia/H11/Compilar build de revisión")]
        public static void BuildReview()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Detener Play y guardar la escena antes de compilar.");
            PlayerSettings.bundleVersion = "0.0.12";
            Directory.CreateDirectory("Builds/H11");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Combat.unity" },
                locationPathName = "Builds/H11/UltimaGuardia.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.DetailedBuildReport
            });
            var s = report.summary;
            var summary = new Summary
            {
                version = PlayerSettings.bundleVersion + "-H11",
                result = s.result.ToString(),
                unity = Application.unityVersion,
                output = s.outputPath,
                errors = s.totalErrors,
                warnings = s.totalWarnings,
                totalBytes = s.totalSize,
                durationSeconds = s.totalTime.TotalSeconds,
                messages = report.steps.SelectMany(step => step.messages.Select(m => m.type + " / " + step.name + ": " + m.content)).ToArray()
            };
            Directory.CreateDirectory("Documentation/Evidence/H11");
            File.WriteAllText("Documentation/Evidence/H11/build-summary.json", JsonUtility.ToJson(summary, true));
            if (s.result != BuildResult.Succeeded) throw new InvalidOperationException("La build H11 falló.");
        }
    }
}
