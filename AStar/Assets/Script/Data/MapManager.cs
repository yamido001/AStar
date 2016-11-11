using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapManager{

	private static MapManager mInstant;
	public static MapManager Instant
	{
		get {
			if (null == mInstant)
				mInstant = new MapManager ();
			return mInstant;
		}
	}

	private bool[,] mMapPassData;

	public MapManager(){
		mMapPassData = new bool[MapWidth,MapHeight];
		ClearData ();
	}


	public uint MapWidth
	{
		get {
			return 10;
		}
	}

	public uint MapHeight
	{
		get {
			return 10;
		}
	}

	public void ClearData()
	{
		for (uint i = 0; i < MapWidth; ++i) {
			for (uint j = 0; j < MapHeight; ++j) {
				mMapPassData [i,j] = true;
			}
		}
	}

	public void SetPassable(Coord co, bool canPass)
	{
		if (co.xPos >= MapWidth || co.yPos >= MapHeight)
			return;
		mMapPassData [co.xPos, co.yPos] = canPass;
	}

	public bool CanPass(Coord co)
	{
		if (co.xPos >= MapWidth || co.yPos >= MapHeight)
			return false;
		return mMapPassData [co.xPos, co.yPos];
	}

	public List<Coord> GetBlockList()
	{
		List<Coord> ret = new List<Coord> ();
		for (int i = 0; i < MapWidth; ++i) {
			for (int j = 0; j < MapHeight; ++j) {
				if (!mMapPassData [i, j]) {
					ret.Add (new Coord ((uint)i, (uint)j));
				}
			}
		}
		return ret;
	}

	public Coord GetDirectionCoord(MapDirection mapDir, Coord co)
	{
		int posX;
		int posY;
		GetDirectionPos(out posX, out posY, co, mapDir);
		return new Coord ((uint)posX, (uint)posY);
	}

	public bool HasDirectionPos(MapDirection mapDir, Coord co)
	{
		int posX;
		int posY;
		GetDirectionPos(out posX, out posY, co, mapDir);
		return posX >= 0 && posX < MapWidth && posY >= 0 && posY < MapHeight;
	}

	private void GetDirectionPos(out int x, out int y, Coord co, MapDirection mapDir)
	{
		x = (int)co.xPos;
		y = (int)co.yPos;
		switch (mapDir) 
		{
		case MapDirection.Left:
			x = (int)co.xPos - 1;
			break;
		case MapDirection.Right:
			x = (int)co.xPos + 1;
			break;
		case MapDirection.Top:
			y = (int)co.yPos + 1;
			break;
		case MapDirection.Bottom:
			y = (int)co.yPos - 1;
			break;
		case MapDirection.RightTop:
			y = (int)co.yPos + 1;
			x = (int)co.xPos + 1;
			break;
		case MapDirection.LeftTop:
			y = (int)co.yPos + 1;
			x = (int)co.xPos - 1;
			break;
		case MapDirection.RightBottom:
			y = (int)co.yPos - 1;
			x = (int)co.xPos + 1;
			break;
		case MapDirection.LeftBottom:
			y = (int)co.yPos - 1;
			x = (int)co.xPos - 1;
			break;
		default:
			break;
		}
	}
}
