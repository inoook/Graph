using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGUIGraphTest : MonoBehaviour
{
    [SerializeField] AnimationCurve curve = null;
    //[SerializeField] Graph graph = default;
    [SerializeField] int bufferNum = 50;
    [SerializeField] bool drawPlusOnly = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  
        //graph.Clear();
        //graph.bufferNum = bufferNum;
        //int num = graph.bufferNum;

        //Keyframe[] keys = curve.keys;
        //float startTime = keys[0].time;
        //float endTime = keys[keys.Length - 1].time;
        //float delta = endTime - startTime;
        //float d = delta / (bufferNum-1);

        //for (var i = 0; i <= bufferNum - 1; i++)
        //{
        //    float v = curve.Evaluate(d * i);
        //    graph.AddData(v);
        //}
        
    }

    [SerializeField, Range(-5, 5)] float outTangent = 0;
    [SerializeField, Range(-5, 5)] float inTangent = 0;
    [ContextMenu("EditTangent")]
    void EditTangent()
    {
        Keyframe[] keys = curve.keys;
        Keyframe key0 = keys[0];
        Keyframe key1 = keys[1];
        key0.outTangent = outTangent;
        key1.inTangent = inTangent;

        keys[0] = key0;
        keys[1] = key1;

        curve.keys = keys;
    }

    [SerializeField] Rect drawRecrt = new Rect(100,100,300,300);
    [SerializeField] float scale = 1;
    private void OnGUI()
    {
        EditTangent();

        GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
        GUILayout.BeginArea(drawRecrt);
        GUILayout.BeginVertical("box");
        GUILayout.Label("Curve");
        //Graph graph = OnGUIGraph.GraphFrom(curve, "curve", bufferNum, Color.red, 1);
        //OnGUIGraph.DrawGraph(graph, new Vector2(200, 200), bufferNum, drawPlusOnly);
        OnGUIGraph.DrawAnimationCurve(curve, "curve", bufferNum, Color.red, 1, new Vector2(200, 200), true);
        outTangent = GUILayout.HorizontalSlider(outTangent, -5, 5);
        inTangent = GUILayout.HorizontalSlider(inTangent, -5, 5);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
