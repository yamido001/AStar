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

	public AStarManager()
	{
		Init ();
	}

	public class AStarNode
	{
		public AStarNode parent;
		public Coord co;
		public int gLength;
		public int fLength;
		public int hLength;
	}

//	private List<AStarNode> mOpen = new List<AStarNode>();
	private QuickFindAStarList mOpen = new QuickFindAStarList();
	private HashSet<uint> mClose = new HashSet<uint>();
	private MapDirection[] mAllDirections = null;
	private TimeStatistics mTimeDebug = new TimeStatistics();

	private void Init()
	{
		Array dirArray = Enum.GetValues (typeof(MapDirection));
		mAllDirections = new MapDirection[dirArray.Length];
		for (int i = 0; i < mAllDirections.Length; ++i) {
			mAllDirections [i] = (MapDirection)dirArray.GetValue (i);
		}
	}

	public List<Coord> FindPath(Coord startCo, Coord endCo)
	{
		List<Coord> path = new List<Coord> ();

		mOpen.Clear ();
		mClose.Clear ();

		AStarNode startNode = new AStarNode ();
		startNode.co = startCo;
		startNode.hLength = GetHLength (startNode.co, endCo);
		startNode.gLength = 0;
		startNode.fLength = startNode.hLength;
		mOpen.Add (startNode);
	
		AStarNode endNode = null;
		mTimeDebug.BeginFrameTest ();
		while (mOpen.Count > 0) {
			mTimeDebug.FrameTest ("JustStart");
			int index = mOpen.GetMinFLengthIndex ();

			AStarNode nearNode = mOpen [index];
			if (null == endNode || endNode.hLength > nearNode.hLength)
				endNode = nearNode;
			//寻路正常，找到完全的路径
			if (nearNode.co.Equals(endCo)) {
				endNode = nearNode.parent;
				break;
			}
			mTimeDebug.FrameTest ("FindNearNode");
			for (int i = 0; i < mAllDirections.Length; ++i) {
				mTimeDebug.FrameTest ("ChildStart");
				if (!MapManager.Instant.HasDirectionPos (mAllDirections[i], nearNode.co)) {
					mTimeDebug.FrameTest ("Child node error pos");
					continue;
				}
				
				Coord childNodeCo = MapManager.Instant.GetDirectionCoord (mAllDirections[i], nearNode.co);
				if (MapManager.Instant.IsBlock (childNodeCo)) {
					mTimeDebug.FrameTest ("Child node in block");
					continue;
				}
				if (!MapManager.Instant.CanPass (nearNode.co, childNodeCo)) {
					mTimeDebug.FrameTest ("Child node can not pass");
					continue;
				}

				int childNodeHLength = GetHLength (childNodeCo, endCo);
				int childNodeGLength = nearNode.gLength + GetGLength (nearNode.co, childNodeCo);
				int childNodeFLength = childNodeGLength + childNodeHLength;
				mTimeDebug.FrameTest ("Child node correct");

				AStarNode nodeInOpen = mOpen.GetNodeByKey (childNodeCo.Key);
				if (null != nodeInOpen && nodeInOpen.fLength > childNodeFLength) {
					nodeInOpen.gLength = childNodeGLength;
					nodeInOpen.fLength = childNodeFLength;
					nodeInOpen.parent = nearNode;
				}
				if (null != nodeInOpen) {
					mTimeDebug.FrameTest ("Child node isInOpen");
					continue;
				}
				mTimeDebug.FrameTest ("Child node isInOpen");
					
				if (mClose.Contains (childNodeCo.Key)) {
					mTimeDebug.FrameTest ("Child node isInClose");
					continue;
				}
				mTimeDebug.FrameTest ("Child node isInClose");

				AStarNode childNode = new AStarNode ();
				childNode.parent = nearNode;
				childNode.co = childNodeCo;
				childNode.fLength = childNodeFLength;
				childNode.gLength = childNodeGLength;
				childNode.hLength = childNodeHLength;
				mOpen.Add (childNode);
				mTimeDebug.FrameTest ("Child node create");
			}
			mTimeDebug.FrameTest ("FindNearNode finish");
			mOpen.RemoveAt(index);
			mClose.Add (nearNode.co.Key);
			mTimeDebug.FrameTest ("Open remove and close add");
		}

		//起点和终点相连接时，返回空
		if (null == endNode)
			return null;
		mTimeDebug.FrameTest ("Begin get path");
		do {
			path.Add (endNode.co);
			endNode = endNode.parent;
		} while(!endNode.co.Equals (startCo));
//		while (endNode.parent != null && !endNode.parent.co.Equals(startCo)) {
//				endNode = endNode.parent;
//				path.Add (endNode.co);
//		}
		path.Reverse();
		mTimeDebug.FrameTest ("Finish get path");
		mTimeDebug.PrintFrameTest ();
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


