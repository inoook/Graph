using UnityEngine;
using System.Collections;

public class Sample2 : MonoBehaviour {

	[SerializeField] GraphViewer pramView = null;

    [SerializeField] Graph graph = null;
    [SerializeField] HorizontalLine extraLine = null;

	[Header("Test")]
    [SerializeField] float v = 0;
    [SerializeField] float w = 0;

    [SerializeField] float amp = 5.0f;

	// Use this for initialization
	void Start () {

		pramView.SetGraph(graph);
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
