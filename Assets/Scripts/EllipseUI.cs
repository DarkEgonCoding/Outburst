using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class EllipseUI : Graphic
{
    [Range(3, 360)]
    public int segments = 64;   // Number of triangle segments (higher = smoother)
    public float width = 100f;  // Horizontal diameter
    public float height = 100f; // Vertical diameter

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float radiusX = width / 2f;
        float radiusY = height / 2f;

        // Add center vertex
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;
        vert.position = Vector2.zero;
        vh.AddVert(vert);

        // Add vertices around ellipse
        float angleStep = 2 * Mathf.PI / segments;
        for (int i = 0; i <= segments; i++) // <= to close loop
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle) * radiusX;
            float y = Mathf.Sin(angle) * radiusY;

            vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = new Vector2(x, y);
            vh.AddVert(vert);

            if (i > 0)
            {
                // Triangle fan from center
                vh.AddTriangle(0, i, i + 1);
            }
        }
    }
}