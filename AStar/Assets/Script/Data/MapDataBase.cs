using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;

public class MapDataBase{

	public static void SaveData(MapSaveData mapData, string fileName)
	{
		mapData.SaveDataCheck ();
		string json =  JsonUtility.ToJson (mapData);
		SaveFile (fileName, json);
	}

	public static MapSaveData LoadData(string fileName)
	{
		string content = LoadFile (fileName);
		if (string.IsNullOrEmpty (content))
			return null;
		MapSaveData saveData = JsonUtility.FromJson<MapSaveData> (content);
		saveData.LoadDataCheck ();
		return saveData;
	}

	private static void SaveFile(string fileName, string content)
	{
		string filePath = GetFilePath (fileName);
		if (File.Exists (filePath))
			File.Delete (filePath);
		File.WriteAllText (filePath, content);
	}

	private static string LoadFile(string fileName)
	{
		string filePath = GetFilePath (fileName);
		if (!File.Exists (filePath))
			return string.Empty;
		return File.ReadAllText (filePath);
	}

	private static string GetFilePath(string fileName)
	{
		return Path.Combine (Application.persistentDataPath, fileName + ".txt");
	}
}
