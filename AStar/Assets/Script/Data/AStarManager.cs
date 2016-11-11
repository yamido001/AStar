using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStarManager{
	private static AStarManager mInstant;
	public static AStarManager Instant
	{
		get {
			if (null == mInstant)
				mInstant = new AStarManager ();
			return mInstant;
		}
	}

	private class AStarNode
	{
		public AStarNode parent;
		public Coord co;
		public int gLength;
	}

	private List<AStarNode> mOpen = new List<AStarNode>();
	private List<AStarNode> mClose = new List<AStarNode>();

	public List<Coord> FindPath(Coord startCo, Coord endCo)
	{
		List<Coord> path = new List<Coord> ();

		mOpen.Clear ();
		mClose.Clear ();

		AStarNode startNode = new AStarNode ();
		startNode.co = startCo;
		mOpen.Add (startNode);
	
		AStarNode endNode = null;

		while (mOpen.Count > 0) {

			AStarNode nearNode = null;
			int nearNodeFinally = int.MaxValue;
			int index = -1;
			for (int i = 0; i < mOpen.Count; ++i) {
				AStarNode node = mOpen [i];
				if (!MapManager.Instant.CanPass (node.co)) {
					continue;
				}

				int FLength = GetHLength (node.co, endCo);
				if (node.parent != null) {
					node.gLength = node.parent.gLength + GetGLength (node.parent.co, node.co);
					FLength += node.gLength;
				}
				if (FLength >= nearNodeFinally) {
					continue;
				}

				nearNode = node;
				nearNodeFinally = FLength;
				index = i;
			}

			if (nearNode == null) {
				break;
			}

			if (nearNode.co.Equals(endCo)) {
				endNode = nearNode;
				break;
			}

			if (nearNode.co.xPos == 4 && nearNode.co.yPos == 1) {
				int testAAA = 12;
				++testAAA;
			}

			Array dirArray = Enum.GetValues (typeof(MapDirection));
			for (int i = 0; i < dirArray.Length; ++i) {
				MapDirection dir = (MapDirection)dirArray.GetValue (i);
				if (!MapManager.Instant.HasDirectionPos (dir, nearNode.co)) {
					continue;
				}
				AStarNode childNode = new AStarNode ();
				childNode.co = MapManager.Instant.GetDirectionCoord (dir, nearNode.co);
				childNode.parent = nearNode;
				childNode.gLength = nearNode.gLength + GetGLength (nearNode.co, childNode.co);

				for (int j = 0; j < mOpen.Count; ++j) {
					AStarNode openNode = mOpen [j];
					if (!openNode.co.Equals(childNode.co))
						continue;
					if (openNode.gLength + GetHLength (openNode.co, endCo) > childNode.gLength + GetHLength (childNode.co, endCo)) {
						openNode.gLength = childNode.gLength;
						openNode.parent = nearNode;
					}
				}

				bool isInClose = false;
				for (int j = 0; j < mClose.Count; ++j) {
					AStarNode closeNode = mClose [j];
					if (closeNode.co.Equals (childNode.co)) {
						isInClose = true;
						break;
					}
				}
				if (isInClose)
					continue;

				mOpen.Add (childNode);

			}
			
			mOpen.RemoveAt(index);
			mClose.Add (nearNode);

			mOpen.Sort(delegate(AStarNode x, AStarNode y) {
				return (x.gLength + GetHLength (x.co, endCo)).CompareTo(y.gLength + GetHLength (y.co, endCo));
				});
		}

		if (null == endNode)
			return null;

		while (endNode.parent != null && !endNode.parent.co.Equals(startCo)) {
			endNode = endNode.parent;
			path.Add (endNode.co);
		}
		path.Reverse();
		return path;
	}

	private int GetHLength(Coord fromCo, Coord endCo)
	{
		return Mathf.Abs ((int)fromCo.xPos - (int)endCo.xPos) + Mathf.Abs ((int)fromCo.yPos - (int)endCo.yPos);
	}

	private int GetGLength(Coord fromCo, Coord endCo)
	{
		return (fromCo.xPos == endCo.xPos || fromCo.yPos == endCo.yPos) ? 10 : 14;
	}
}
