using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimeStatistics{

	private int mLastTime = 0;
	private Dictionary<string, int> mAllTime = new Dictionary<string, int> ();
	public void FrameTest(string key)
	{
		if (!mAllTime.ContainsKey (key))
			mAllTime.Add (key, 0);
		int delat = System.Environment.TickCount - mLastTime;
		mAllTime [key] += delat;
		mLastTime = System.Environment.TickCount;
	}

	public void BeginFrameTest()
	{
		mLastTime = System.Environment.TickCount;
	}

	public void PrintFrameTest()
	{
		mAllTime = (from entry in mAllTime
			orderby entry.Value descending
			select entry).ToDictionary (pair => pair.Key, pair => pair.Value);
		foreach (var item in mAllTime) {
			Debug.LogError (item.Key + "  " + item.Value);
		}
	}
}
