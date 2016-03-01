//using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Graph{
	
	public enum DrawType
	{
		Line, Bar
	}

	[HideInInspector]
	public List<Vector3> values;

	[HideInInspector]
	public float zoomX, zoomY;
	
	public string name;
	public float maxY;
	public Color color;
	public DrawType drawType = DrawType.Line;
	
	[HideInInspector]
	public float min, max;
	[HideInInspector]
	public int bufferNum;
	
	//public bool autoResizeY;
	public Graph(string name_) : this()
	{
		name = name_;
	}
	
	public Graph(string name_, float maxY_, Color color_, DrawType type_) : this()
	{
		name = name_;
		maxY = maxY_;
		color = color_;
		drawType = type_;
	}
	
	public Graph()
	{
		values = new List<Vector3>();
		
		zoomX = 2.5f;
		zoomY = 1;
		maxY = 150;
		color = Color.red;
		
		min = 0;
		max = 0;
		
		bufferNum = 100;
	}
	
	public void AddData(float v)
	{
		if(values == null){
			values = new List<Vector3>();
		}
		min = Mathf.Min(min, v);
		max = Mathf.Max(max, v);
		
		Vector3 tPos = new Vector3(Time.time, v, 0);
		values.Add( tPos );
		
		if(values.Count > bufferNum){
			values.RemoveRange(0, values.Count - bufferNum);
		}
	}
	
	public void Clear()
	{
		if(values != null && values.Count > 0){
			values.Clear();
		}
		max = 0;
		min = 0;
	}
	
}

[System.Serializable]
public class HorizontalLine{

	public float maxY;
	public float y;
	public Color color = Color.red;
	
	[HideInInspector]
	public float zoomY;


	public HorizontalLine()
	{
		y = 0;
		color = Color.red;
		zoomY = 1.0f;
		maxY = 150;
	}
}

public class GraphViewer : MonoBehaviour {

	public static int WINDOW_COUNT = 0;
	public static int BASE_WINDOW_ID = 0;

	
	private int windowId = 0;
	public GUISkin guiSkin;

	[Header("windowRect w, h override and auto resize by viewSize x, y.")]
	public Rect windowRect = new Rect();
	public Vector2 viewSize = new Vector2(100,100);
	public Color horitontalLineColor = new Color(0,0,0,0.25f);
	
	private int graphHeight;
	private int graphHeightHalf;
	private float zoomX;
	
	public int bufferNum = 100;
	
	public List<Graph> graphs;
	public List<HorizontalLine> extralines;
	
	// Use this for initialization
	void Awake () {
		WINDOW_COUNT++;
		windowId = BASE_WINDOW_ID + WINDOW_COUNT;
	}

	void Start()
	{
		windowRect.width = viewSize.x;
	}
	
	public void SetGraphParam(Graph graph)
	{
		graph.bufferNum = this.bufferNum;
		graphs.Add(graph);
	}
	
	public void AddData(int id, float v)
	{
		if(id >= graphs.Count){
			Graph g = new Graph();
			graphs.Add(g);
		}
		graphs[id].AddData(v);
	}
	public void AddData(string name, float v)
	{
		Graph g = GetGraphByName(name);
		if (g == null) {
			g = new Graph (name);
			SetGraphParam (g);
		}
		if(g != null){
			g.AddData(v);
		}else{
			Debug.LogWarning("no Graph name: "+name);
		}
	}
	
	public Graph GetGraphByName(string name)
	{
		return graphs.Find(delegate(Graph graph) {
			return graph.name == name;
		});
	}
	
	private void Clear()
	{
		for(int i = 0; i < graphs.Count; i++){
			Graph graph = graphs[i];
			graph.Clear();
		}
	}
	void Update()
	{
		for(int i = 0; i < graphs.Count; i++){
			graphs[i].zoomX = zoomX;
			graphs[i].bufferNum = bufferNum;
		}
	}
	
//	public Color guiColor = Color.white;
	
	void OnGUI()
	{
		
		if(guiSkin != null){
			GUI.skin = guiSkin;
		}

		graphHeight = (int)viewSize.y;

		graphHeightHalf = graphHeight / 2;

		windowRect = GUILayout.Window(windowId, windowRect, DoMyWindow, this.gameObject.name);

	}
	
	
	void DoMyWindow(int windowID) {

		windowRect.width = viewSize.x;

		GUILayout.BeginArea(new Rect(0,18, windowRect.width, graphHeight));
		Color myColor = GUI.color;
		Color c = myColor;
		c.a = 0.5f;
		GUI.color = c;
		GUILayout.Box("", GUILayout.MinHeight(graphHeight), GUILayout.Width(windowRect.width));
		GUI.color = myColor;
		
		zoomX = windowRect.width / bufferNum;
		
		DrawBaseLine();
		
		for(int n = 0; n < graphs.Count; n++){
			Graph graph = graphs[n];
			List<Vector3> values = graph.values;
			
			if(values != null && values.Count > 0){
				float maxY = graph.maxY;
				
				graph.zoomY = graphHeightHalf / maxY;

				float zoomY = graph.zoomY;
				float min = graph.min;
				float max = graph.max;
				
				Color color = graph.color;
				
				Vector3[] _values = values.ToArray();
				
				for(int i = 0; i < _values.Length; i++){
					_values[i].x = i * zoomX;
					_values[i].y = -1 * _values[i].y * zoomY + graphHeightHalf;
				}
				
				string name = graph.name;
				
				GUI.color = color;
				float v = values[values.Count-1].y;
				GUI.Label(new Rect(4,18*n,280, 50), name +": "+ v.ToString("0.00")+" / min: " + min.ToString("0.00") + " / max: "+ max.ToString("0.00"));
				
				// draw
				if(graph.drawType == Graph.DrawType.Line){
					DrawAAPolyLine(_values, color);
				}else{
					DrawBarLine(_values, color);
				}
			}
		}
		
		DrawExtraLine();
		GUI.color = Color.white;
		
		GUILayout.EndArea();
		GUILayout.Space(graphHeight+2);
		
		if( GUILayout.Button("CLEAR") ){
			Clear();
		}
		GUI.DragWindow();
    }
	
	void DrawBaseLine()
	{
		CreateLineMaterial();
		    
		UnityEngine.GL.PushMatrix();
		UnityEngine.GL.LoadIdentity();
		lineMaterial.SetPass(0);
		
		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		UnityEngine.GL.Color( horitontalLineColor );
		UnityEngine.GL.Vertex(new Vector3(0,0,0));
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,0,0));
		
		UnityEngine.GL.Vertex(new Vector3(0,graphHeightHalf,0));
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,graphHeightHalf,0));
		
		UnityEngine.GL.Vertex(new Vector3(0,graphHeight,0));
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,graphHeight,0));
		UnityEngine.GL.End();
		UnityEngine.GL.PopMatrix();
	}
	
	
	public void SetExtraLine(HorizontalLine extraLine)
	{
		extralines.Add(extraLine);
	}
	
	void DrawExtraLine()
	{
		if(extralines == null || extralines.Count < 1){ return; }
		CreateLineMaterial();
		    
		UnityEngine.GL.PushMatrix();
		UnityEngine.GL.LoadIdentity();
		lineMaterial.SetPass(0);
		
		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		for(int i = 0; i < extralines.Count; i++){
			HorizontalLine lineParam = extralines[i];
			Color color = lineParam.color;
			float maxY = lineParam.maxY;
			//if(lineParam.autoResizeY){
				lineParam.zoomY = graphHeightHalf / maxY;
			//}
			float zoomY = lineParam.zoomY;
			float y = -lineParam.y * zoomY + graphHeightHalf;
			UnityEngine.GL.Color( color );
			UnityEngine.GL.Vertex(new Vector3(0,y,0));
			UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,y,0));
		}
		UnityEngine.GL.End();
		UnityEngine.GL.PopMatrix();
	}
	
	void DrawAAPolyLine(Vector3[] vectors, Color color){
		
		CreateLineMaterial();
		    
		UnityEngine.GL.PushMatrix();
		UnityEngine.GL.LoadIdentity();
		lineMaterial.SetPass(0);

		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		UnityEngine.GL.Color( color );
		for(int i = 0; i < vectors.Length-1; i++){
			if( (vectors[i].y < graphHeight && vectors[i].y > 0 ) && (vectors[i+1].y < graphHeight && vectors[i+1].y > 0 ) ){
				UnityEngine.GL.Color( color );
				UnityEngine.GL.Vertex(vectors[i]);
				UnityEngine.GL.Vertex(vectors[i+1]);
			//}else if((vectors[i].y > graphHeight && vectors[i+1].y > graphHeight ) || (vectors[i+1].y < graphHeight && vectors[i+1].y < graphHeight ) ){
			}else{ 
				//UnityEngine.GL.Color( Color.red );
				if( vectors[i].y > graphHeight && vectors[i+1].y < graphHeight ){
					Vector3 pos = getClossPoint(vectors[i], new Vector3( 0, graphHeight, 0 ), vectors[i+1], new Vector3( 10, graphHeight, 0 ) );
					UnityEngine.GL.Vertex(vectors[i+1]);
					UnityEngine.GL.Vertex(pos);
				}else if( vectors[i].y < graphHeight && vectors[i+1].y > graphHeight ) {
					Vector3 pos = getClossPoint(vectors[i], new Vector3( 0, graphHeight, 0 ), vectors[i+1], new Vector3( 10, graphHeight, 0 ) );
					UnityEngine.GL.Vertex(vectors[i]);
					UnityEngine.GL.Vertex(pos);
				}
				if( vectors[i].y > 0 && vectors[i+1].y < 0 ){
					Vector3 pos = getClossPoint(vectors[i], new Vector3( 0, 0, 0 ), vectors[i+1], new Vector3( 10, 0, 0 ) );
					UnityEngine.GL.Vertex(vectors[i]);
					UnityEngine.GL.Vertex(pos);
				}else if( vectors[i].y < 0 && vectors[i+1].y > 0 ){
					Vector3 pos = getClossPoint(vectors[i], new Vector3( 0, 0, 0 ), vectors[i+1], new Vector3( 10, 0, 0 ) );
					UnityEngine.GL.Vertex(vectors[i+1]);
					UnityEngine.GL.Vertex(pos);
				}
			}
		}
		
		UnityEngine.GL.End();
        UnityEngine.GL.PopMatrix();
	}
	void DrawBarLine(Vector3[] vectors, Color color){
		
		CreateLineMaterial();
		    
		UnityEngine.GL.PushMatrix();
		UnityEngine.GL.LoadIdentity();
		lineMaterial.SetPass(0);
		
		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		UnityEngine.GL.Color( color );
		for(int i = 0; i < vectors.Length; i++){
			Vector3 v = vectors[i];
			if(v.y < 0){
				v.y = 0;
			}
			if(v.y > graphHeight){
				v.y = graphHeight;
			}
			Vector3 t_z = v;
			t_z.y = graphHeightHalf;
			UnityEngine.GL.Vertex(t_z);
			UnityEngine.GL.Vertex(v);
		}
		
		UnityEngine.GL.End();
        UnityEngine.GL.PopMatrix();
	}

	
	static Material lineMaterial;
	static void CreateLineMaterial() {
	    if( !lineMaterial ) {
			lineMaterial = new Material(Shader.Find("Hidden/Lines/Colored Blended"));
	        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	    }
	}

	Vector3 getClossPoint(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4){
		// http://www.mztm.jp/2010/08/05/clossline/
		float S1 = ((P4.x-P2.x)*(P1.y-P2.y)-(P4.y-P2.y)*(P1.x-P2.x))*0.5f;
		float S2 = ((P4.x-P2.x)*(P2.y-P3.y)-(P4.y-P2.y)*(P2.x-P3.x))*0.5f;
		Vector3 crossPoint = new Vector3(0,0,0);
		crossPoint.x = P1.x + (P3.x-P1.x)*(S1/(S1 + S2));
		crossPoint.y = P1.y + (P3.y-P1.y)*(S1/(S1 + S2));

		return crossPoint;
	}

}
