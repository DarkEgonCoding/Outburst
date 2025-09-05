using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class TrazepoidUI : Graphic
{
    public float topWidth = 100f;
    public float bottomWidth = 200f;
    public float height = 100f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float halfTop = topWidth / 2f;
        float halfBottom = bottomWidth / 2f;

        // Define trapezoid corners
        Vector2 topLeft = new Vector2(-halfTop, height / 2f);
        Vector2 topRight = new Vector2(halfTop, height / 2f);
        Vector2 bottomRight = new Vector2(halfBottom, -height / 2f);
        Vector2 bottomLeft = new Vector2(-halfBottom, -height / 2f);

        // Add verts
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = topLeft; vh.AddVert(vert);
        vert.position = topRight; vh.AddVert(vert);
        vert.position = bottomRight; vh.AddVert(vert);
        vert.position = bottomLeft; vh.AddVert(vert);

        // Add tris
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}
    
