using UnityEngine;
using System.Collections;

public class DragTrigger : MonoBehaviour {

	public UIPanel fencePanel;
	public UIWidget widget;

	void OnDragStart()
	{

	}

	void OnDrag(Vector2 delta)
	{
		Vector3 worldPos = transform.position;
		Vector3 screenPoint = Camera.main.WorldToScreenPoint (worldPos);

		//检测是否会超出边界
		if (null != fencePanel && null != widget) {
			Vector4 widgetScreenRange = GetScreenRange (widget.worldCorners);
			Vector4 panelScreenRanget = GetScreenRange (fencePanel.worldCorners);
			if (delta.x < 0 && widgetScreenRange.y + delta.x < panelScreenRanget.y) {
				delta.x = Mathf.Min(panelScreenRanget.y - widgetScreenRange.y, 0f);
			}
			else if (delta.x > 0 && widgetScreenRange.x + delta.x > panelScreenRanget.x) {
				delta.x = Mathf.Max(panelScreenRanget.x - widgetScreenRange.x, 0f);
			}
			if (delta.y < 0 && widgetScreenRange.z + delta.y < panelScreenRanget.z) {
				delta.y = Mathf.Min(panelScreenRanget.z - widgetScreenRange.z, 0f);
			}
			else if (delta.y > 0 && widgetScreenRange.w + delta.y > panelScreenRanget.w) {
				delta.y = Mathf.Max(panelScreenRanget.w - widgetScreenRange.w, 0f);
			}
		}
		screenPoint += new Vector3 (delta.x, delta.y, 0f);
		Vector3 newWorldPos = Camera.main.ScreenToWorldPoint (screenPoint);
		transform.position = newWorldPos;
	}

	/// <summary>
	/// Gets the screen range.
	/// </summary>
	/// <returns>The screen range.  xyzw -  left, right, top, bottom</returns>
	/// <param name="worldPos">World position.</param>
	Vector4 GetScreenRange(Vector3[] worldPos)
	{
		Vector2[] screenPoints = new Vector2[worldPos.Length];
		for (int i = 0; i < worldPos.Length; ++i) {
			screenPoints [i] = Camera.main.WorldToScreenPoint (worldPos[i]);
		}
		Vector4 ret = new Vector4 (float.MaxValue, float.MinValue, float.MinValue, float.MaxValue);
		for (int i = 0; i < screenPoints.Length; ++i) {
			Vector2 widgetScreenPoint = screenPoints[i];
			if (ret.x > widgetScreenPoint.x)
				ret.x = widgetScreenPoint.x;
			if (ret.y < widgetScreenPoint.x)
				ret.y = widgetScreenPoint.x;
			if (ret.z < widgetScreenPoint.y)
				ret.z = widgetScreenPoint.y;
			if (ret.w > widgetScreenPoint.y)
				ret.w = widgetScreenPoint.y;
		}
		return ret;
	}

	void OnDragEnd()
	{
		
	}
}
