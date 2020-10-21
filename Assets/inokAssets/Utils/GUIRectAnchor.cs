using UnityEngine;
using System.Collections;

public class GUIRectAnchor : MonoBehaviour {

	public enum Anchor{
		UpperLeft, UpperRight, LowerLeft, LowerRight
	}

	[SerializeField] GraphViewer paramViewer = null;

	public Anchor anchor = Anchor.UpperLeft;

	public Vector2 margin = new Vector2(10,10);

	// Use this for initialization
	void Start () {
		OnGUI ();
	}
	
	// Update is called once per frame
	void OnGUI () {
		Rect rect = new Rect ();
		float w = paramViewer.WindowRect.width;
		float h = paramViewer.WindowRect.height;

		if (anchor == Anchor.UpperLeft || anchor == Anchor.LowerLeft) {
			rect.x = margin.x;
		}
		if (anchor == Anchor.UpperLeft || anchor == Anchor.UpperRight) {
			rect.y = margin.y;
		}
		//
		if (anchor == Anchor.UpperRight || anchor == Anchor.LowerRight) {
			rect.x = Screen.width - margin.x - w;
		}
		if (anchor == Anchor.LowerLeft || anchor == Anchor.LowerRight) {
			rect.y = Screen.height - margin.y - h;
		}
		rect.width = w;
		rect.height = h;
		paramViewer.WindowRect = rect;
	}
}
