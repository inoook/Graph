using UnityEngine;
using System.Collections;

public class Test2 : MonoBehaviour {

	public float v = 0;
	public float w = 0;
	public ParamViewer pramView;

	public GraphParam graph;

	public float amp = 5.0f;
	// Use this for initialization
	void Start () {

		graph = new GraphParam("cube.v");
		graph.maxY = 5.0f;
		graph.color = Color.blue;
		graph.drawType = GraphParam.DrawType.Line;
		pramView.SetGraphParam(graph);

		ExtraLineParam extraLine = new ExtraLineParam();
		extraLine.maxY = 5.0f;
		extraLine.color = Color.magenta;
		extraLine.y = 5.0f;
		pramView.SetExtraLine(extraLine);
	}


	float t = 0;
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		v = amp * Mathf.Sin(t*2.0f);
		w = amp * Mathf.Cos(t*2.0f);

		graph.AddData (v);
	}
}
