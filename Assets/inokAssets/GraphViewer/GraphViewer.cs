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
	
	public string name = "name";
	public float maxY = 150;
	public Color color = Color.red;
	public DrawType drawType = DrawType.Line;
	public bool enable = true;
	
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
		
		zoomX = 1;
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
public class Graph3D
{
	[SerializeField] public Graph xGraph = null;
	[SerializeField] public Graph yGraph = null;
	[SerializeField] public Graph zGraph = null;

	public void AddData(Vector3 vector3)
    {
		xGraph.AddData(vector3.x);
		yGraph.AddData(vector3.y);
		zGraph.AddData(vector3.z);
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

	
	[SerializeField] private int windowId = 0;
    [SerializeField] GUISkin guiSkin = null;

    [SerializeField] Vector2 windowPos = new Vector2(10, 10);
    //[Header("windowRect w, h override and auto resize by viewSize x, y.")]
    Rect windowRect = new Rect();
    [SerializeField] Vector2 viewSize = new Vector2(100,100);
    [SerializeField] int bufferNum = 100;

    [SerializeField] Color horitontalLineColor = new Color(0,0,0,0.25f);
	[SerializeField] bool drawPlusOnly = false;

    [SerializeField] public float guiScale = 1.0f;

    private int graphHeight;
	private int graphHeightHalf;
	private float zoomX;

    [SerializeField] List<Graph> graphs = new List<Graph>();
    [SerializeField] List<HorizontalLine> extralines = new List<HorizontalLine>();

    public Rect WindowRect {
        get { return windowRect; }
        set {
            windowPos.x = value.x;
            windowPos.y = value.y; 
            windowRect = value;
        }
    }

    // Use this for initialization
    void Awake () {
		WINDOW_COUNT++;
		windowId = 10+BASE_WINDOW_ID + WINDOW_COUNT;
	}

	void Start()
	{
		windowRect.width = viewSize.x;
	}
    
    public void SetGraph(Graph graph)
	{
		graph.bufferNum = this.bufferNum;
		graphs.Add(graph);
	}

	public void SetGraph(Graph3D graph3D)
	{
		SetGraph(graph3D.xGraph);
		SetGraph(graph3D.yGraph);
		SetGraph(graph3D.zGraph);
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
			SetGraph (g);
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
		bufferNum = Mathf.Max (2, bufferNum);
		for(int i = 0; i < graphs.Count; i++){
			graphs[i].zoomX = zoomX;
			graphs[i].bufferNum = bufferNum;
		}
	}

	//
	void OnGUI()
	{
		if (guiSkin != null)
		{
			GUI.skin = guiSkin;
		}

		GUI.matrix = Matrix4x4.Scale(Vector3.one * guiScale);

		graphHeight = (int)viewSize.y;

		graphHeightHalf = drawPlusOnly ? graphHeight : graphHeight / 2;

		if (drawAsWindow)
		{
			windowRect.x = windowPos.x;
			windowRect.y = windowPos.y;

			windowRect = GUILayout.Window(windowId, windowRect, DoMyWindow, this.gameObject.name);

			windowPos.x = windowRect.x;
			windowPos.y = windowRect.y;
		}
		else
		{
			windowRect.x = windowPos.x;
			windowRect.y = windowPos.y;
			windowRect.height = viewSize.y + 80;

			GUILayout.BeginArea(windowRect);
			GUILayout.BeginVertical("box");

			windowRect.width = viewSize.x;

			GUILayout.BeginArea(new Rect(0, 0, windowRect.width, graphHeight));
			Color myColor = GUI.color;
			Color c = myColor;

			GUI.color = myColor;

			offset = Vector3.zero;

			zoomX = windowRect.width / bufferNum;

			// draw
			DrawGraph();


			GUI.color = Color.white;

			GUILayout.EndArea();
			GUILayout.Space(graphHeight + 2);

			if (GUILayout.Button("CLEAR"))
			{
				Clear();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	[SerializeField] bool drawAsWindow = true;
	//

	void OnGUI__()
	{
		if (guiSkin != null){
			GUI.skin = guiSkin;
		}

		GUI.matrix = Matrix4x4.Scale(Vector3.one * guiScale);

		graphHeight = (int)viewSize.y;

		graphHeightHalf = drawPlusOnly ? graphHeight : graphHeight / 2;

        windowRect.x = windowPos.x;
        windowRect.y = windowPos.y;

        windowRect = GUILayout.Window(windowId, windowRect, DoMyWindow, this.gameObject.name);
        
        windowPos.x = windowRect.x;
        windowPos.y = windowRect.y;
    }


	Vector3 offset;

	void DrawGraph()
    {
        for (int n = 0; n < graphs.Count; n++) {
            Graph graph = graphs[n];

            List<Vector3> values = graph.values;

            if (values != null && values.Count > 0) {
                //float maxY = graph.maxY;
                //graph.zoomY = graphHeightHalf / maxY;

                float zoomY = graph.zoomY;
                float min = graph.min;
                float max = graph.max;

                string name = graph.name;

                Color color = graph.color;
                GUI.color = color;
                float v = values[values.Count - 1].y;
                //GUILayout.Label(name + ": " + v.ToString("0.00") + " / min: " + min.ToString("0.00") + " / max: " + max.ToString("0.00"));
                string label = name;
                if (graph.enable) {
                    label += ": " + v.ToString("0.00") + " / min: " + min.ToString("0.00") + " / max: " + max.ToString("0.00");
                }
                graph.enable = GUILayout.Toggle(graph.enable, label);
            }
        }
        
        // -------
        if (Event.current.type.Equals(EventType.Repaint)) {
            CreateLineMaterial();

            UnityEngine.GL.PushMatrix();
            UnityEngine.GL.LoadIdentity();
            lineMaterial.SetPass(0);

            DrawBaseLine();

            for (int n = 0; n < graphs.Count; n++) {
                Graph graph = graphs[n];

                List<Vector3> values = graph.values;

                if (values != null && values.Count > 0) {
                    float maxY = graph.maxY;

                    graph.zoomY = graphHeightHalf / maxY;

                    float zoomY = graph.zoomY;

                    Color color = graph.color;

                    Vector3[] _values = values.ToArray();

                    for (int i = 0; i < _values.Length; i++) {
                        _values[i].x = i * zoomX;
                        _values[i].y = -1 * _values[i].y * zoomY + graphHeightHalf;
                    }

                    if (!graph.enable) { continue; } // NO DRAW GRAPH

                    // draw
                    if (graph.drawType == Graph.DrawType.Line) {
                        DrawAAPolyLine(_values, color);
                    }
                    else {
                        DrawBarLine(_values, color);
                    }
                }
            }

            DrawExtraLine();

            UnityEngine.GL.PopMatrix();
        }

    }

	void DoMyWindow(int windowID) {

		windowRect.width = viewSize.x;

		GUILayout.BeginArea(new Rect(0,18, windowRect.width, graphHeight));
        Color myColor = GUI.color;
		
		GUI.color = myColor;

		offset = Vector3.zero;

		zoomX = windowRect.width / bufferNum;

        // draw
        DrawGraph();


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
		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		UnityEngine.GL.Color( horitontalLineColor );
		UnityEngine.GL.Vertex(new Vector3(0,0,0) + offset);
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,0,0) + offset);
		
		UnityEngine.GL.Vertex(new Vector3(0,graphHeightHalf,0) + offset);
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,graphHeightHalf,0) + offset);
		
		UnityEngine.GL.Vertex(new Vector3(0,graphHeight,0) + offset);
		UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,graphHeight,0) + offset);
		UnityEngine.GL.End();
	}

	public void SetExtraLine(HorizontalLine extraLine)
	{
		extralines.Add(extraLine);
	}
	
	void DrawExtraLine()
	{
        if (extralines == null || extralines.Count < 1){ return; }

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
			UnityEngine.GL.Vertex(new Vector3(0,y,0) + offset);
			UnityEngine.GL.Vertex(new Vector3(bufferNum*zoomX,y,0) + offset);
		}
		UnityEngine.GL.End();
	}

	void DrawAAPolyLine(Vector3[] vectors, Color color)
	{
		if (vectors.Length < 2) { return; }

		UnityEngine.GL.Begin(UnityEngine.GL.LINES);
		UnityEngine.GL.Color(color);
		for (int i = 0; i < vectors.Length-1; i++)
		{
			Vector3 current = vectors[i];
			Vector3 next = vectors[i + 1];
			if (current.y > graphHeight)
			{
				current = getClossPoint(current, new Vector3(0, graphHeight, 0), next, new Vector3(10, graphHeight, 0));
			}
			else if (current.y < 0)
			{
				current = getClossPoint(current, new Vector3(0, 0, 0), next, new Vector3(10, 0, 0));
			}
			if (next.y > graphHeight)
			{
				next = getClossPoint(current, new Vector3(0, graphHeight, 0), next, new Vector3(10, graphHeight, 0));
			}
			else if (next.y < 0)
			{
				next = getClossPoint(current, new Vector3(0, 0, 0), next, new Vector3(10, 0, 0));
			}

			UnityEngine.GL.Vertex(current + offset);
			UnityEngine.GL.Vertex(next + offset);
		}

		UnityEngine.GL.End();
	}

    void DrawBarLine(Vector3[] vectors, Color color){
		
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
			UnityEngine.GL.Vertex(t_z + offset);
			UnityEngine.GL.Vertex(v + offset);
		}
		
		UnityEngine.GL.End();
	}

	
	static Material lineMaterial;
	static void CreateLineMaterial() {
	    if( !lineMaterial ) {
			lineMaterial = new Material(Shader.Find("Hidden/Lines/Colored Blended"));
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
