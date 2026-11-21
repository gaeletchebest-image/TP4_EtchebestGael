using UnityEngine;
using UnityEngine.UI;

namespace UltimaGuardia
{
    // Provisional vector icons; final UI artwork belongs to H08.
    public sealed class StatusIcon : Graphic
    {
        public StatusKind Kind;
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (Kind == StatusKind.Poison)
                Polygon(mesh, new[] { new Vector2(0, 10), new Vector2(7, -1), new Vector2(6, -7), new Vector2(0, -10), new Vector2(-6, -7), new Vector2(-7, -1) });
            else if (Kind == StatusKind.Haste)
            {
                Polygon(mesh, new[] { new Vector2(0, 10), new Vector2(9, 1), new Vector2(-9, 1) });
                Polygon(mesh, new[] { new Vector2(-4, 1), new Vector2(4, 1), new Vector2(4, -9), new Vector2(-4, -9) });
            }
            else
            {
                Polygon(mesh, new[] { new Vector2(-9, 8), new Vector2(1, 8), new Vector2(-3, 0), new Vector2(0, -9), new Vector2(-7, -4) });
                Polygon(mesh, new[] { new Vector2(5, 8), new Vector2(9, 8), new Vector2(7, -4), new Vector2(3, -8), new Vector2(1, 0) });
            }
        }
        private void Polygon(VertexHelper mesh, Vector2[] points)
        {
            int start = mesh.currentVertCount;
            foreach (var p in points) mesh.AddVert(p, color, Vector2.zero);
            for (int i = 1; i < points.Length - 1; i++) mesh.AddTriangle(start, start + i, start + i + 1);
        }
    }

}
