using UnityEngine;
using System.Collections;

public class Sample2 : MonoBehaviour {

	[SerializeField] GraphViewer pramView = null;

    [SerializeField] Graph graph = null;
    [SerializeField] HorizontalLine extraLine = null;

	[SerializeField] Graph graphLerp = null;
	[SerializeField] float lerpV = 0;
	[SerializeField] float lerpT = 0.2f;
	[SerializeField] Graph graphSmoothDamp = null;
	[SerializeField] float smoothV = 0;
	[SerializeField] float smoothSpeed = 0;
	[SerializeField] float smoothT = 0.2f;



	[Header("Test")]
    [SerializeField, Range(-5, 5)] float v = 0;
    [SerializeField] float w = 0;

    [SerializeField] float amp = 5.0f;
    [SerializeField] float speedAmp = 1.0f;

	// Use this for initialization
	void Start () {

		pramView.SetGraph(graph);
		pramView.SetGraph(graphLerp);
		pramView.SetGraph(graphSmoothDamp);

		pramView.SetExtraLine(extraLine);
	}


	float t = 0;
	// Update is called once per frame
	void Update () {
		//t += Time.deltaTime * speedAmp;
		//v = amp * Mathf.Sin(t*2.0f);
		//w = amp * Mathf.Cos(t*2.0f);

		graph.AddData (v);

		//
		lerpV = Mathf.Lerp(lerpV, v, lerpT);
		graphLerp.AddData(lerpV);
		//

		smoothV = Mathf.SmoothDamp(smoothV, v, ref smoothSpeed, smoothT);
		graphSmoothDamp.AddData(smoothV);
	}
}
