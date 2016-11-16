using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AnalyticsEngine : ScriptableObject {

	private static Dictionary<string, int> counters;
	private static string[] counterNames;
	private static List<string> DataRows;
	private static bool Initialized = false;
	private static StreamWriter outputStream;
	private static string filename;
	// Use this for initialization
	void Start () {
	
	}
	
	public static void Initialize(string[] counterNameList) {
		if(!Initialized) {
			DirectoryInfo dir = System.IO.Directory.CreateDirectory(Application.dataPath + "/data/" + System.DateTime.Now.ToString("yy-MMM-dd"));
			filename = Application.dataPath + "/data/" + dir.Name + "/" + System.DateTime.Now.ToString("hh_mm_ss");
			StreamWriter outputStream = new StreamWriter(filename, true);
			string topRow = "";
			counterNames = counterNameList;
			counters = new Dictionary<string, int>();
			foreach(string counter in counterNameList) {
				counters.Add(counter, 0);
				topRow += counter;
				topRow += "\t";
			}
			outputStream.WriteLine(topRow);
			outputStream.Close();
			Initialized = true;
		}
	}

	public static void Increment(string counterName) {
		counters[counterName] += 1;
	}

	public static void PrintRow() {
		string output = "";
		foreach(string counter in counterNames) {
			output = string.Concat(output, counters[counter].ToString(), "\t");
			counters[counter] = 0;
		}
		Debug.Log("printing row");
		Debug.Log(output);
		Debug.Log(outputStream);
		outputStream = new StreamWriter(filename, true);
		outputStream.WriteLine(output);
		outputStream.Close();
	}

	static void OnApplicationQuit() {
		Debug.Log("printing analytics data");

		foreach(string dataPoint in DataRows) {
			outputStream.WriteLine(dataPoint);
		}

		outputStream.Close();
	}
}
