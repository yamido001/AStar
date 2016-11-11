using UnityEngine;
using System.Collections;

public struct Coord 
{
	public uint xPos;
	public uint yPos;

	public Coord(uint x, uint y)
	{
		xPos = x;
		yPos = y;
	}

	public bool Equals(Coord co)
	{
		return xPos == co.xPos && yPos == co.yPos;
	}

	public override string ToString ()
	{
		return string.Format ("({0},{1}]", xPos, yPos);
	}

	public uint Key
	{
		get {
			return yPos * MapManager.Instant.MapWidth + xPos;
		}
	}
}

public enum MapDirection
{
	Left,
	Right,
	Top,
	Bottom,
	RightTop,
	LeftTop,
	RightBottom,
	LeftBottom,
}
