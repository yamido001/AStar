using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapView : MonoBehaviour {

	public enum GridType
	{
		Empty,
		Block,
		StartPoint,
		EndPoint,
	}

	#region UIRef
	public int gridSize = 10;
	public int edgeSize = 30;
	public float togHeight = 100f;

	public UIButton btnFindPath;
	public UIButton btnClearFindPath;
	public UIButton btnLoadData;
	public UIButton btnSaveData;
	public UIInput inputSaveName;

	public Transform togParent;
	public Transform transView;
	public GameObject templateObjPar;

	public UIToggle togTemplate;
	public UISprite blockTemplate;
	public UISprite startPointTemplate;
	public UISprite endPointTemplate;
	public UISprite lineTemplate;
	public UISprite pathTemplate;
	public UISprite sprBg;
	#endregion

	private UISprite mStartPoint;
	private UISprite mEndPoint;
	private List<UIToggle> mTogList = new List<UIToggle>();
	private List<UISprite> mPathList = new List<UISprite> ();
	private List<UISprite> mBlockList = new List<UISprite> ();
	private GridType mSelectType = GridType.Empty;

	public void Init()
	{
		templateObjPar.SetActive (false);
		//描画背景
		int bgWidth = gridSize * (int)MapManager.Instant.MapWidth + edgeSize * 2;
		int bgHeight = gridSize * (int)MapManager.Instant.MapHeight + edgeSize * 2;
		sprBg.transform.localPosition = new Vector3 (-edgeSize, edgeSize, 0f);
		sprBg.width = bgWidth;
		sprBg.height = bgHeight;
		BoxCollider collider = sprBg.GetComponent<BoxCollider> ();
		collider.size = new Vector3 (MapManager.Instant.MapWidth * gridSize, MapManager.Instant.MapHeight * gridSize, 0f);
		collider.center = new Vector3 (bgWidth / 2f, -bgHeight / 2f, 0f);
		UIEventListener.Get (collider.gameObject).onClick = OnBgClick;

		//描画格子
		float bottomPos = -MapManager.Instant.MapHeight * gridSize;
		float topPos = 0f;
		float leftPos = 0f;
		float rightPos = MapManager.Instant.MapWidth * gridSize;
		//竖线
		for (int i = 0; i <= MapManager.Instant.MapWidth; ++i) {
			float posX = i * gridSize + leftPos;
			UISprite sprLine = CreateLine ();
			sprLine.width = (int)(topPos - bottomPos);
			sprLine.transform.localEulerAngles = new Vector3 (0f, 0f, -90f);
			sprLine.transform.localPosition = new Vector3 (posX, topPos, 0f);
		}
		//横线
		for (int i = 0; i <= MapManager.Instant.MapHeight; ++i) {
			float posY = topPos -i * gridSize;
			UISprite sprLine = CreateLine ();
			sprLine.width = (int)(rightPos - leftPos);
			sprLine.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			sprLine.transform.localPosition = new Vector3 (leftPos, posY, 0f);
		}

		//创建起始点
		mStartPoint = GameObject.Instantiate (startPointTemplate) as UISprite;
		mStartPoint.transform.parent = transView;
		mStartPoint.transform.localScale = Vector3.one;
		HideStartPoint ();

		//创建结束点
		mEndPoint = GameObject.Instantiate (endPointTemplate) as UISprite;
		mEndPoint.transform.parent = transView;
		mEndPoint.transform.localScale = Vector3.one;
		HideEndPoint ();

		//选择列表
		Array array = Enum.GetValues(typeof(GridType));
		for (int i = 0; i < array.Length; ++i) {
			GridType type = (GridType)array.GetValue (i);
			UIToggle tog = GameObject.Instantiate (togTemplate) as UIToggle;
			tog.transform.parent = togParent;
			tog.transform.localScale = Vector3.one;
			tog.transform.localPosition = new Vector3 (0f, -togHeight * i, 0f);
			tog.name = type.ToString ();
			tog.GetComponentInChildren<UILabel> ().text = type.ToString ();
			mTogList.Add (tog);
		}
		mTogList [0].value = true;

		UIEventListener.Get (btnFindPath.gameObject).onClick = OnFindPathClicked;
		UIEventListener.Get (btnClearFindPath.gameObject).onClick = OnClearPathClicked;
		UIEventListener.Get (btnSaveData.gameObject).onClick = OnSaveData;
		UIEventListener.Get (btnLoadData.gameObject).onClick = OnLoadData;

		RefreshBlock ();
	}

	#region Data setting
	public void ClearData()
	{
		RefreshBlock ();
		SetPath (null);
		HideStartPoint ();
		HideEndPoint ();
	}

	public void RefreshBlock()
	{
		for (int i = 0; i < mBlockList.Count; ++i) {
			GameObject.Destroy (mBlockList [i]);
		}
		for (int i = 0; i < MapManager.Instant.MapWidth; ++i) {
			for (int j = 0; j < MapManager.Instant.MapHeight; ++j) {
				Coord co = new Coord ((uint)i, (uint)j);
				if (!MapManager.Instant.CanPass (co)) {
					UISprite sprBlock = GameObject.Instantiate (blockTemplate) as UISprite;
					sprBlock.gameObject.SetActive (true);
					sprBlock.transform.parent = transView;
					sprBlock.transform.localScale = Vector3.one;
					sprBlock.transform.localPosition = GetGridPos(co);
					mBlockList.Add (sprBlock);
				}
			}
		}
	}

	public void SetStartPoint(Coord co)
	{
		mStartPoint.gameObject.SetActive (true);
		mStartPoint.transform.localPosition = GetGridPos (co);
	}

	public void HideStartPoint()
	{
		mStartPoint.gameObject.SetActive (false);
	}

	public void SetEndPoint(Coord co)
	{
		mEndPoint.gameObject.SetActive (true);
		mEndPoint.transform.localPosition = GetGridPos (co);
	}

	public void HideEndPoint()
	{
		mEndPoint.gameObject.SetActive (false);
	}

	public void SetPath(List<Coord> pathList)
	{
		for (int i = 0; i < mPathList.Count; ++i) {
			GameObject.DestroyImmediate (mPathList [i]);
		}
		if (pathList == null)
			return;
		for (int i = 0; i < pathList.Count; ++i) {
			UISprite sprPath = GameObject.Instantiate (pathTemplate) as UISprite;
			sprPath.transform.parent = transView;
			sprPath.transform.localScale = Vector3.one;
			sprPath.transform.localPosition = GetGridPos (pathList [i]);
			mPathList.Add (sprPath);
		}
	}
	#endregion

	private UISprite CreateLine()
	{
		UISprite spr = GameObject.Instantiate (lineTemplate) as UISprite;
		spr.gameObject.SetActive (true);
		spr.transform.parent = transView;
		spr.transform.localScale = Vector3.one;
		spr.height = 3;
		return spr;
	}

	private Vector3 GetGridPos(Coord co)
	{
		float posX = (co.xPos + 0.5f) * gridSize;
		float poxY = -(MapManager.Instant.MapHeight - co.yPos - 0.5f) * gridSize;
		return new Vector3 (posX, poxY, 0f);
	}

	#region UI CallBack
	public Action<Coord, GridType> hdlOnClickGrid;
	public Action hdlFindPath;
	public Action<string> hdlSaveData;
	public Action<string> hdlLoadData;
	private void OnBgClick(GameObject obj)
	{
		Camera camera = Camera.main;
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, float.MaxValue)) {
			Vector3 hitLocalPos = sprBg.transform.InverseTransformPoint (hit.point);
			int indexX = ((int)hitLocalPos.x - edgeSize) / gridSize;
			int indexY = (int)(-hitLocalPos.y - edgeSize) / gridSize;
			Coord co = new Coord ((uint)indexX, (uint)(MapManager.Instant.MapHeight - indexY - 1));
			if (hdlOnClickGrid != null)
				hdlOnClickGrid.Invoke (co, mSelectType);
		}
	}

	public void OnToggleChanged()
	{
		for (int i = 0; i < mTogList.Count; ++i) {
			UIToggle tog = mTogList [i];
			if (tog.value) {
				mSelectType = (GridType)Enum.Parse (typeof(GridType), tog.name);
			}
		}
	}

	private void OnFindPathClicked(GameObject obj)
	{
		if (hdlFindPath != null)
			hdlFindPath.Invoke ();
	}

	private void OnClearPathClicked(GameObject obj)
	{
		SetPath (null);
	}

	private void OnSaveData(GameObject go)
	{
		if (hdlSaveData != null)
			hdlSaveData.Invoke ("test");
	}

	private void OnLoadData(GameObject go)
	{
		if (hdlLoadData != null)
			hdlLoadData.Invoke ("test");
	}
	#endregion
}
