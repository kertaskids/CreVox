using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PBGWindow {
	[InitializeOnLoad]
	public class LevelCandidateWindow : EditorWindow {
		public static LevelCandidateWindow Instance { 
			get;
			private set;
		}
		public static bool IsOpen { get { return Instance != null; } }
	
		public enum SortedBy {
			None,
			Threat,
			Impetus,
			Tempo
		};
		private SortedBy _sortedBy;

		private AnimationCurve _levelCandidateThreat;
		private AnimationCurve _levelCandidateImpetus;
		private AnimationCurve _levelCandidateTempo;

		private Vector2 _levelCandidateScrollPosition = Vector2.zero;
		private string[] _levelCandidates;	
		private bool[] _isFoldoutLevelCandidate;

		void Awake(){
			Initialize();
			_sortedBy = SortedBy.None;
		}

		void Initialize(){
			_levelCandidateThreat = AnimationCurve.Linear(0, 0, 1, 1);
			_levelCandidateImpetus = AnimationCurve.Linear(0, 0, 1, 1);
			_levelCandidateTempo = AnimationCurve.Linear(0, 0, 1, 1);
			_levelCandidates = new string[] { "Candidate 1", "Candidate 2", "Candidate 3", "Candidate 4", "Candidate 5" };
			_isFoldoutLevelCandidate = new bool[] { false, false, false, false, false };
		}

		void OnGUI(){
			LevelCandidateLayout();
		}

		void LevelCandidateLayout(){
			GUILayout.Space(5);
			GUILayout.Label("Level Candidates");
			EditorGUILayout.BeginVertical("Box");
			_levelCandidateScrollPosition = EditorGUILayout.BeginScrollView (_levelCandidateScrollPosition, GUILayout.Height(300));
			for (int i = 0, j = 0; i < _isFoldoutLevelCandidate.Length; i++, j++) {
				_isFoldoutLevelCandidate[i] = EditorGUILayout.Foldout(_isFoldoutLevelCandidate[i], _levelCandidates[j]);
				if (_isFoldoutLevelCandidate [i]) {
					EditorGUI.indentLevel++;
					EditorGUILayout.CurveField ("Threat", _levelCandidateThreat);
					EditorGUILayout.CurveField("Impetus", _levelCandidateImpetus);
					EditorGUILayout.CurveField("Tempo", _levelCandidateTempo);
					EditorGUILayout.BeginVertical("HelpBox");
					string pacingInformation = 
						"Pacing Aspects" +
						"\n Threat: x, " + "Impetus: x, " + "Tension: x";
					GUILayout.Label(pacingInformation);
					string roomInformation = 
						"Room" +
						"\n Number of enemies: x" +
						"\n Number of tiles of player paths: x" +
						"\n Number of tiles in player paths that has enemy: x" +
						"\n Number of empty tiles: x" +
						"\n Number of enemy tiles: x" +
						"\n Number of passable tiles: x" +
						"\n Number of total tiles: x" +
						"\n Number of exit connections: x" +
						"\n Tag: Normal Room";
					GUILayout.Label(roomInformation);
					string enemyInformation = 
						"Enemy" +
						"\n Number of enemy: x" +
						"\n Number of enemy types: x" +
						"\n" +
						"\n<All others information regarding to pacing variables>";
					GUILayout.Label(enemyInformation);
					EditorGUILayout.EndVertical();	
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			_sortedBy = (SortedBy)EditorGUILayout.EnumPopup("Sorted By", _sortedBy);
			EditorGUILayout.HelpBox ("Total number of candidates: x." +
				"\nGenerated in x seconds.", MessageType.Info);
			GUILayout.Button("Apply", GUILayout.Height (24));
		}
	}
}