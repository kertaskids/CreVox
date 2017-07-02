using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PBGWindow {
	[InitializeOnLoad]
	public class DataExporterWindow : EditorWindow {
		public static DataExporterWindow Instance { 
			get;
			private set;
		}
		public static bool IsOpen { get { return Instance != null; } }

		void Awake(){}
		void Initialize(){}
		void OnGUI(){
			EditorGUILayout.HelpBox("Info", MessageType.Info);
			if (GUILayout.Button ("Export Data", GUILayout.Height(24))) {
				ExportData();
			}
		}

		void ExportData(){
			if(EditorUtility.DisplayDialog("Exporting Data", "The data will be exported to a CSV file. Are you sure?", "Yes", "Cancel")) {
				EditorUtility.SaveFilePanel("Choose Directory", "", "", "");	
			}

		}
	}
}