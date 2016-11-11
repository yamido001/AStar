using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMediator : MonoBehaviour {
	MapView view;
	private Coord mStartCo;
	private bool mHasStartCo = false;
	private Coord mEndCo;
	private bool mHasEndCo = false;

	public void Init()
	{
		view = gameObject.GetComponent<MapView> ();
		view.hdlFindPath = OnFindPathClicked;
		view.hdlOnClickGrid = OnClickGird;
		view.hdlSaveData = OnSaveDataClicked;
		view.hdlLoadData = OnLoadDataClicked;
		view.Init ();
	}

	public void ClearData()
	{
		view.ClearData ();
		mHasEndCo = false;
		mHasStartCo = false;
	}

	private void OnClickGird(Coord co, MapView.GridType type)
	{
		MapView.GridType beforeType = MapView.GridType.Empty;
		if (mHasStartCo && co.Equals (mStartCo))
			beforeType = MapView.GridType.StartPoint;
		else if (mHasEndCo && co.Equals (mEndCo))
			beforeType = MapView.GridType.EndPoint;
		else if (!MapManager.Instant.CanPass (co))
			beforeType = MapView.GridType.Block;
		
		switch (beforeType) {
		case MapView.GridType.Empty:
			OnSetGridType (co, type);
			break;
		case MapView.GridType.Block:
			if(type == MapView.GridType.Empty)
				OnSetGridType (co, type);
			break;
		case MapView.GridType.EndPoint:
			if (type == MapView.GridType.Block)
				mHasEndCo = false;
			else
				OnSetGridType (co, type);
			break;
		case MapView.GridType.StartPoint:
			if (type == MapView.GridType.Block)
				mHasStartCo = false;
			else
				OnSetGridType (co, type);
			break;
		default:
			break;
		}
	}

	private void OnSetGridType(Coord co, MapView.GridType type)
	{
		switch (type) {
		case MapView.GridType.Empty:
			MapManager.Instant.SetPassable (co, true);
			break;
		case MapView.GridType.Block:
			MapManager.Instant.SetPassable (co, false);
			break;
		case MapView.GridType.StartPoint:
			mHasStartCo = true;
			mStartCo = co;
			break;
		case MapView.GridType.EndPoint:
			mHasEndCo = true;
			mEndCo = co;
			break;
		default:
			break;
		}

		view.RefreshBlock ();
		if (mHasStartCo)
			view.SetStartPoint (mStartCo);
		else
			view.HideStartPoint ();
		if (mHasEndCo)
			view.SetEndPoint (mEndCo);
		else
			view.HideEndPoint ();
	}

	void OnFindPathClicked()
	{
		if (!mHasStartCo || !mHasEndCo)
			return;
		view.SetPath (AStarManager.Instant.FindPath (mStartCo, mEndCo));
	}

	void OnSaveDataClicked(string fileName)
	{
		MapSaveData saveData = new MapSaveData ();
		if (mHasStartCo) {
			saveData.startCo = new MapSaveCoord ();
			saveData.startCo.xPos = mStartCo.xPos;
			saveData.startCo.yPos = mStartCo.yPos;
		}
		if (mHasEndCo) {
			saveData.endCo = new MapSaveCoord ();
			saveData.endCo.xPos = mEndCo.xPos;
			saveData.endCo.yPos = mEndCo.yPos;
		}
		List<Coord> blockList = MapManager.Instant.GetBlockList ();
		for (int i = 0; i < blockList.Count; ++i) {
			MapSaveCoord saveCo = new MapSaveCoord ();
			saveCo.xPos = blockList [i].xPos;
			saveCo.yPos = blockList [i].yPos;
			saveData.blockList.Add (saveCo);
		}
		MapDataBase.SaveData (saveData, fileName);
	}

	void OnLoadDataClicked(string fileName)
	{
		ClearData ();
		MapSaveData saveData = MapDataBase.LoadData (fileName);
		if (saveData == null)
			return;
		MapManager.Instant.ClearData ();
		for (int i = 0; i < saveData.blockList.Count; ++i) {
			MapSaveCoord saveCo = saveData.blockList [i];
			MapManager.Instant.SetPassable (new Coord (saveCo.xPos, saveCo.yPos), false);
		}
		if (saveData.startCo != null) {
			OnClickGird (new Coord (saveData.startCo.xPos, saveData.startCo.yPos), MapView.GridType.StartPoint);
		}
		if (saveData.endCo != null) {
			OnClickGird (new Coord (saveData.endCo.xPos, saveData.endCo.yPos), MapView.GridType.EndPoint);
		}
		view.RefreshBlock ();
	}
}
