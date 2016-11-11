using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class MapSaveData{
	public MapSaveCoord startCo;
	public MapSaveCoord endCo;
	public List<MapSaveCoord> blockList = new List<MapSaveCoord>();

	public void SaveDataCheck()
	{
		if (startCo == null) {
			startCo = GetEmptyCoord();
		}
		if (endCo == null) {
			endCo = GetEmptyCoord();
		}
	}

	public void LoadDataCheck()
	{
		if (IsEmptyCoord (startCo))
			startCo = null;
		if (IsEmptyCoord (endCo))
			endCo = null;
	}

	private bool IsEmptyCoord(MapSaveCoord co)
	{
		return co.xPos == uint.MaxValue && co.yPos == uint.MaxValue;
	}

	private MapSaveCoord GetEmptyCoord()
	{
		MapSaveCoord co = new MapSaveCoord ();
		co.xPos = uint.MaxValue;
		co.yPos = uint.MaxValue;
		return co;
	}
}

[Serializable]
public class MapSaveCoord
{
	public uint xPos;
	public uint yPos;
}