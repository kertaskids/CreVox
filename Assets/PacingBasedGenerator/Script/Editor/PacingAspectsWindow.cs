using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PBGWindow {
	[InitializeOnLoad]
	public class PacingAspectsWindow : EditorWindow {
		public static PacingAspectsWindow Instance { 
			get;
			private set;
		}
		public static bool IsOpen { get { return Instance != null; } }

		private Object _level;

		private AnimationCurve _originalThreat;
		private AnimationCurve _originalImpetus;
		private AnimationCurve _originalTempo;
		private AnimationCurve _intendedThreat;
		private AnimationCurve _intendedImpetus;
		private AnimationCurve _intendedTempo;

		private bool _isFoldoutOriginal;
		private bool _isFoldoutIntended;
		private string _messageForOriginal;
		private string _messageForIntended;

		private Vector2 _originalScrollPosition = Vector2.zero;
		private Vector2 _intendedScrollPosition = Vector2.zero;

		// [Change later] change with room structure
		private string[] _originalRooms;
		private string[] _intendedRooms;
		// [Change later] embed this in the structure
		private bool[] _isFoldoutOriginalRooms;
		private bool[] _isFoldoutIntendedRooms;

		void Awake(){
			Initialize ();	
		}

		void Initialize(){
			_isFoldoutOriginal = false;
			_isFoldoutIntended = false;
			_originalThreat = AnimationCurve.Linear(0, 0, 1, 1);
			_originalTempo = AnimationCurve.EaseInOut(0, 0, 1, 1);
			_originalImpetus = AnimationCurve.Linear(0, 0, 1, 1);
			_intendedThreat = new AnimationCurve();
			_intendedTempo = new AnimationCurve();
			_intendedImpetus = new AnimationCurve();

			_originalRooms = new string[] {"Room 1", "Room 2", "Room 3", "Room 4", "Room 5"};
			_intendedRooms = new string[] {"Room 1", "Room 2", "Room 3", "Room 4", "Room 5"};
			_isFoldoutOriginalRooms = new bool[] { false, false, false, false, false };
			_isFoldoutIntendedRooms = new bool[] { false, false, false, false, false };
			Debug.Log (_originalRooms.Length);
		}

		void OnGUI(){
			LevelPickerLayout();
			OriginalLevelLayout();
			IntendedLevelLayout ();
			GUILayout.Button("Apply", GUILayout.Height(24));
		}

		void LevelPickerLayout(){
			EditorGUILayout.BeginHorizontal("Box");
			_level = EditorGUILayout.ObjectField("Level", _level, typeof(Object), true);
			EditorGUILayout.EndVertical();
		}

		void OriginalLevelLayout(){
			EditorGUILayout.BeginVertical("Box");
			_isFoldoutOriginal = EditorGUILayout.Foldout(_isFoldoutOriginal, "Original Level");
			if (_isFoldoutOriginal) {
				EditorGUI.indentLevel++;
				EditorGUILayout.CurveField("Threat", _originalThreat);
				EditorGUILayout.CurveField("Impetus", _originalImpetus);
				EditorGUILayout.CurveField("Tempo", _originalTempo);
				EditorGUI.indentLevel--;

				EditorGUILayout.BeginHorizontal("Box");
				_originalScrollPosition = EditorGUILayout.BeginScrollView(_originalScrollPosition, GUILayout.Height(200));
				for (int i = 0, j = 0; i < _isFoldoutOriginalRooms.Length; i++, j++) {
					_isFoldoutOriginalRooms[i] = EditorGUILayout.Foldout (_isFoldoutOriginalRooms[i], _originalRooms[j]);
					if(_isFoldoutOriginalRooms[i]){
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
					}
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical ();
		}

		void IntendedLevelLayout(){
			EditorGUILayout.BeginVertical("Box");
			_isFoldoutIntended = EditorGUILayout.Foldout (_isFoldoutIntended, "Intended Level");
			if (_isFoldoutIntended) {
				EditorGUI.indentLevel++;
				EditorGUILayout.CurveField ("Threat", _intendedThreat);
				EditorGUILayout.CurveField ("Impetus", _intendedImpetus);
				EditorGUILayout.CurveField ("Tempo", _intendedTempo);
				EditorGUI.indentLevel--;

				EditorGUILayout.BeginHorizontal("Box");
				_intendedScrollPosition = EditorGUILayout.BeginScrollView(_intendedScrollPosition, GUILayout.Height(200));
				for (int i = 0, j = 0; i < _isFoldoutIntendedRooms.Length; i++, j++) {
					_isFoldoutIntendedRooms[i] = EditorGUILayout.Foldout (_isFoldoutIntendedRooms[i], _intendedRooms[j]);
					if(_isFoldoutIntendedRooms[i]){
						EditorGUILayout.BeginVertical("HelpBox");
						string pacingInformation = " Threat: x, " + "Impetus: x, " + "Tension: x";
						GUILayout.Label("Pacing Aspects");
						GUILayout.Label(pacingInformation);
						EditorGUILayout.Space();
						ConstraintsLayout();
						EditorGUILayout.EndVertical();	
					}
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical ();
		}
			
		void ConstraintsLayout(){
			float tempLabelWidth = EditorGUIUtility.labelWidth;
			string constraintsLabel;
			GUILayout.Label("Constraints");

			// Room Constraints
			constraintsLabel = "Room";
			GUILayout.Label(constraintsLabel);
			EditorGUILayout.BeginVertical("Box");
			SingleConstraintLayout("Number of tiles", 25, 100);
			EditorGUILayout.EndVertical();
			GUILayout.Space(5);
			// Enemy Instance Constraints
			constraintsLabel = "Enemy Instances";
			GUILayout.Label(constraintsLabel);
			EditorGUILayout.BeginVertical("Box");
			SingleConstraintLayout("Total number", 0, 10);
			SingleConstraintLayout("Number of types", 0, 10);
			EditorGUILayout.EndVertical();
			GUILayout.Space(5);
			// Enemy Instance Constraints
			constraintsLabel = "Enemy's Attributes";
			GUILayout.Label(constraintsLabel);
			EditorGUILayout.BeginVertical("Box");
			SingleConstraintLayout("HP", 50, 100);
			SingleConstraintLayout("Speed", 1, 5);
			SingleConstraintLayout("Damage", 25, 50);
			SingleConstraintLayout("Attack Frequency", 1, 5);
			EditorGUILayout.EndVertical();
		}

		void SingleConstraintLayout(string label, int minValue, int maxValue){
			float tempLabelWidth = EditorGUIUtility.labelWidth;

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(" "+label, GUILayout.Width((Screen.width/4) + Screen.width/40));
			GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			EditorGUIUtility.labelWidth = 50;
			EditorGUILayout.DelayedIntField("Min", minValue, GUILayout.Width(Screen.width/4));
			EditorGUILayout.DelayedIntField("Max", maxValue, GUILayout.Width(Screen.width/4));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = tempLabelWidth;
		}
	}
}
