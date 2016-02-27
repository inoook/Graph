using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	
	public ParamViewer pramView;
	
	[Header("Test")]
	public float v = 0;
	public float w = 0;

	public float amp = 5.0f;

	// Use this for initialization
	void Start () {
		
		GraphParam graph = new GraphParam("param_v");
		graph.maxY = 5.0f;
		graph.color = Color.blue;
		graph.drawType = GraphParam.DrawType.Line;
		pramView.SetGraphParam(graph);
		
		GraphParam graph2 = new GraphParam("param_w");
		graph2.maxY = 5.0f;
		graph2.color = Color.green;
		pramView.SetGraphParam(graph2);
		
		ExtraLineParam extraLine = new ExtraLineParam();
		extraLine.maxY = 5.0f;
		extraLine.color = Color.magenta;
		extraLine.y = 5.0f;
		pramView.SetExtraLine(extraLine);

//		InvokeRepeating("Update_", 0.0f, 0.05f);
	}

	public Color c = Color.white;

	float t = 0;
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		v = amp * Mathf.Sin(t*2.0f);
		w = amp * Mathf.Cos(t*2.0f);
		
		pramView.GetGraphByName ("param_v").color = c;
		pramView.AddData("param_v", v);
		pramView.AddData("param_w", w);
	}
}
