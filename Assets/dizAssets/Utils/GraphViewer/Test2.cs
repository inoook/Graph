using UnityEngine;
using System.Collections;

public class Test2 : MonoBehaviour {

	public GraphViewer pramView;

	public Graph graph;
	public HorizontalLine extraLine;
	
	[Header("Test")]
	public float v = 0;
	public float w = 0;

	public float amp = 5.0f;

	// Use this for initialization
	void Start () {

		pramView.SetGraphParam(graph);
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
