using UnityEngine;
using System.Collections;

public class Sample1 : MonoBehaviour {

	[SerializeField] GraphViewer pramView = null;

	[Header("Test")]
    [SerializeField] float v = 0;
    [SerializeField] float w = 0;

    [SerializeField] float amp = 5.0f;

	// Use this for initialization
	void Start () {

		Graph graph = new Graph("param_v");
		graph.maxY = 5.0f;
		graph.color = Color.blue;
		graph.drawType = Graph.DrawType.Line;
		pramView.SetGraph(graph);

		Graph graph2 = new Graph("param_w");
		graph2.maxY = 5.0f;
		graph2.color = Color.green;
		pramView.SetGraph(graph2);

		HorizontalLine extraLine = new HorizontalLine();
		extraLine.maxY = 5.0f;
		extraLine.color = Color.magenta;
		extraLine.y = 5.0f;
		pramView.SetExtraLine(extraLine);
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
