using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
	[SerializeField] GraphViewer pramView = null;

	[SerializeField] Graph graph = null;



	// Use this for initialization
	void Start()
	{
		pramView.SetGraph(graph);
		graph.AddData(10);
		graph.AddData(-10);
        graph.AddData(-10);
        graph.AddData(0);
    }


	float t = 0;
	// Update is called once per frame
	void Update()
	{

		
	}
}
