using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveGraph : MonoBehaviour
{
    [SerializeField] GraphViewer pramView = null;
    [SerializeField] Graph graph = null;
    [SerializeField] AnimationCurve curve = null;

    // Use this for initialization
    void Start()
    {
        pramView.SetGraph(graph);
        DrawAnimationCurve();
    }

    private void OnValidate()
    {
        EditTangent();
        DrawAnimationCurve();
    }


    void DrawAnimationCurve()
    {
        graph.Clear();
        int num = graph.bufferNum;
        Keyframe[] keys = curve.keys;
        float startTime = keys[0].time;
        float endTime = keys[keys.Length - 1].time;
        float delta = endTime - startTime;
        float d = delta / num;
        for (var i = 0; i <= num; i++)
        {
            float v = curve.Evaluate(d * i);
            graph.AddData(v);
        }
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
}
