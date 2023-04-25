using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public static class OnGUIGraph
{
    public static void DrawAnimationCurve(AnimationCurve curve,string name, int bufferNum, Color color, float maxY, Vector2 viewSize, bool drawPlusOnly = false)
    {
        Graph graph = GraphFrom(curve, name, bufferNum, color, maxY);
        DrawGraph(graph, viewSize, bufferNum, drawPlusOnly);
    }
    public static Graph GraphFrom(AnimationCurve curve, string name, int bufferNum, Color color,float maxY)
    {
        Graph graph = new Graph(name, maxY, color, Graph.DrawType.Line);
        graph.Clear();
        graph.bufferNum = bufferNum;
        int num = graph.bufferNum;

        Keyframe[] keys = curve.keys;
        float startTime = keys[0].time;
        float endTime = keys[keys.Length - 1].time;
        float delta = endTime - startTime;
        float d = delta / (bufferNum - 1);

        for (var i = 0; i <= bufferNum - 1; i++)
        {
            float v = curve.Evaluate(d * i);
            graph.AddData(v);
        }
        return graph;
    }


    // -----
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material(Shader.Find("Hidden/Lines/Colored Blended"));
        }
    }

    static GUIStyle boxStyle;
    static void CreateBoxStyle()
    {
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle("box");
            boxStyle.normal.background = Texture2D.whiteTexture;
        }
    }

    public static void DrawGraph(Graph graph, Vector2 viewSize, int bufferNum, bool drawPlusOnly = false)
    {

        if (!graph.enable) { return; }

        CreateBoxStyle();

        GUILayout.Box("", boxStyle, GUILayout.Width(viewSize.x), GUILayout.Height(viewSize.y));
        
        Vector3 offset = new Vector3(boxStyle.margin.left, boxStyle.margin.top, 0);

        float graphHeight = viewSize.y;

        float graphHeightHalf = drawPlusOnly ? graphHeight : graphHeight / 2f;
        float zoomX = viewSize.x / bufferNum;

        // -------
        if (Event.current.type.Equals(EventType.Repaint))
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            offset.x = lastRect.x;
            offset.y = lastRect.y;

            CreateLineMaterial();

            UnityEngine.GL.PushMatrix();
            UnityEngine.GL.LoadIdentity();
            lineMaterial.SetPass(0);

            List<Vector3> values = graph.values;

            if (values != null && values.Count > 0)
            {
                float maxY = graph.maxY;

                graph.zoomY = graphHeightHalf / maxY;

                float zoomY = graph.zoomY;

                Color color = graph.color;

                Vector3[] _values = values.ToArray();

                for (int i = 0; i < _values.Length; i++)
                {

                    _values[i].x = i * zoomX;
                    _values[i].y = -1 * _values[i].y * zoomY + graphHeightHalf;

                }

                // draw
                if (graph.drawType == Graph.DrawType.Line)
                {
                    DrawAAPolyLine(_values, color, graphHeight, offset);
                }
                //else
                //{
                //    DrawBarLine(_values, color);
                //}
            }

            UnityEngine.GL.PopMatrix();
        }

    }

    static void DrawAAPolyLine(Vector3[] vectors, Color color, float graphHeight, Vector3 offset)
    {
        if (vectors.Length < 2) { return; }

        UnityEngine.GL.Begin(UnityEngine.GL.LINES);
        UnityEngine.GL.Color(color);
        for (int i = 0; i < vectors.Length - 1; i++)
        {
            Vector3 current = vectors[i];
            Vector3 next = vectors[i + 1];
            if (current.y > graphHeight)
            {
                current = getClossPoint(current, new Vector3(0, graphHeight, 0), next, new Vector3(10, graphHeight, 0));
            }
            else if (current.y < 0)
            {
                current = getClossPoint(current, new Vector3(0, 0, 0), next, new Vector3(10, 0, 0));
            }
            if (next.y > graphHeight)
            {
                next = getClossPoint(current, new Vector3(0, graphHeight, 0), next, new Vector3(10, graphHeight, 0));
            }
            else if (next.y < 0)
            {
                next = getClossPoint(current, new Vector3(0, 0, 0), next, new Vector3(10, 0, 0));
            }

            UnityEngine.GL.Vertex(current + offset);
            UnityEngine.GL.Vertex(next + offset);
        }

        UnityEngine.GL.End();
    }

    static Vector3 getClossPoint(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
    {
        // http://www.mztm.jp/2010/08/05/clossline/
        float S1 = ((P4.x - P2.x) * (P1.y - P2.y) - (P4.y - P2.y) * (P1.x - P2.x)) * 0.5f;
        float S2 = ((P4.x - P2.x) * (P2.y - P3.y) - (P4.y - P2.y) * (P2.x - P3.x)) * 0.5f;
        Vector3 crossPoint = new Vector3(0, 0, 0);
        crossPoint.x = P1.x + (P3.x - P1.x) * (S1 / (S1 + S2));
        crossPoint.y = P1.y + (P3.y - P1.y) * (S1 / (S1 + S2));

        return crossPoint;
    }
}
