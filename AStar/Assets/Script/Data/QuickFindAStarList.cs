using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuickFindAStarList{

	private List<AStarManager.AStarNode> mList = new List<AStarManager.AStarNode>();
	private Dictionary<uint,AStarManager.AStarNode> mDic = new Dictionary<uint, AStarManager.AStarNode>();

	public int GetMinFLengthIndex()
	{
		int index = 0;
		for (int i = 0; i < mList.Count; ++i) {
			if (mList [i].fLength < mList[index].fLength) {
				index = i;
			}
		}
		return index;
	}

	public int Count
	{
		get{
			return mList.Count;
		}
	}

	public AStarManager.AStarNode this[int index]  
	{  
		get  
		{  
			return mList[index];  
		} 
	}  

	public AStarManager.AStarNode GetNodeByIndex(int index)
	{
		return mList [index];
	}

	public AStarManager.AStarNode GetNodeByKey(uint key)
	{
		if (mDic.ContainsKey (key)) {
			return mDic [key];
		}
		return null;
	}

	public void Add(AStarManager.AStarNode node)
	{
		mList.Add (node);
		mDic.Add (node.co.Key, node);
	}

	public void RemoveAt(int index)
	{
		AStarManager.AStarNode node = mList [index];
		mDic.Remove (node.co.Key);
		mList.RemoveAt (index);
	}

	public void Clear()
	{
		mList.Clear ();
		mDic.Clear ();
	}
}
