using UnityEditor;
using UnityEngine;
using PBGWindow;

namespace PBGSystem {
	public class PacingWindow : EditorWindow{
		[MenuItem("Pacing Generator/Pacing Aspects")]
		public static void ShowPacingWindow(){
			EditorWindow window = EditorWindow.GetWindow <PBGWindow.PacingAspectsWindow> ("Pacing Aspects", true);
			window.position = new Rect (50, 50, 400, 640);
		}

		[MenuItem("Pacing Generator/Level Candidate")]
		public static void ShowCandidateWindow(){
			EditorWindow window = EditorWindow.GetWindow <PBGWindow.LevelCandidateWindow> ("Level Candidates", true);
			window.position = new Rect (50, 50, 400, 640);
		}

		[MenuItem("Pacing Generator/Data Exporter")]
		public static void ShowDataExporter(){
			EditorWindow window = EditorWindow.GetWindow <PBGWindow.DataExporterWindow> ("Data Exporter", true);
			window.position = new Rect (50, 50, 400, 640);
		}
	}	
}