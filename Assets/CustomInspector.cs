// http://tips.hecomi.com/entry/2014/06/23/222805
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomInspector : MonoBehaviour 
{
	private List<float> data_ = new List<float>();

	private float t = 0;
	public float amp = 5.0f;

	void Update() 
	{
		//data_.Add(Random.Range (0, 50));

		t += Time.deltaTime;
		float v = amp * Mathf.Sin(t*2.0f);
		data_.Add(v);

		if (data_.Count > 100) {
			data_.RemoveAt(0);
		}
	}
	
	void OnGUI()
	{
		var area = GUILayoutUtility.GetRect(Screen.width, Screen.height);
		
		// Grid
		const int div = 10;
		for (int i = 0; i <= div; ++i) {
			var lineColor = (i == 0 || i == div) ? Color.white : Color.gray;
			var lineWidth = (i == 0 || i == div) ? 2f : 1f;
			var x = (area.width  / div) * i;
			var y = (area.height / div) * i;
			Drawing.DrawLine (
				new Vector2(area.x + x, area.y),
				new Vector2(area.x + x, area.yMax), lineColor, lineWidth, false);
			Drawing.DrawLine (
				new Vector2(area.x,    area.y + y),
				new Vector2(area.xMax, area.y + y), lineColor, lineWidth, false);
		}
		
		// Data
		if (data_.Count > 0) {
//			var max = data_.Max();
//			var dx  = area.width / data_.Count; 
//			var dy  = area.height / max;
//			Vector2 previousPos = new Vector2(area.x, area.yMax); 
//			for (var i = 0; i < data_.Count; ++i) {
//				var x = area.x + dx * i;
//				var y = area.yMax - dy * data_[i];
//				var currentPos = new Vector2(x, y);
//				Drawing.DrawLine(previousPos, currentPos, Color.red, 3f, true);
//				previousPos = currentPos;
//			}
			var max = data_.Max();
			var dx  = area.width / 100; 
			var dy  = area.height;
			Vector2 previousPos = new Vector2(area.x, area.yMax); 
			for (var i = 0; i < data_.Count; ++i) {
				var x = area.x + dx * i;
				var y = area.yMax - dy * data_[i] - area.yMax/2;
				var currentPos = new Vector2(x, y);
				Drawing.DrawLine(previousPos, currentPos, Color.red, 3f, false);
				//Drawing0.DrawLine(previousPos, currentPos, Color.red, 3f);
				previousPos = currentPos;
			}
		}
	}
}