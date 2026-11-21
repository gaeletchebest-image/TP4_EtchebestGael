using System;
using System.IO;
using UnityEngine;

namespace UltimaGuardia.Tests
{
    // A run never overwrites an image that a reviewer may have open/memory-mapped.
    internal static class InterfaceEvidence
    {
        private static string root;
        public static string Root
        {
            get
            {
                if (root == null)
                {
                    root = Path.Combine(Application.dataPath, "../Documentation/Evidence/H08/Run-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6));
                    Directory.CreateDirectory(root);
                }
                return root;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => root = null;
    }
}
