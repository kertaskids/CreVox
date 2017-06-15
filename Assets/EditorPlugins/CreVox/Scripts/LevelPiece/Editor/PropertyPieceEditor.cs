﻿using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CreVox
{
	[CustomEditor(typeof(PropertyPiece))]
	public class PropertyPieceEditor : LevelPieceEditor
	{
		PropertyPiece pp;

		void OnEnable()
		{
			pp = (PropertyPiece)target;
			if (pp.eventRange.Length != 6)
				pp.eventRange = new LevelPiece.EventRange[6];
		}
		#region InspectorGUI
		public override void OnInspectorGUI ()
		{
			Color def = GUI.color;
			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.LabelField ("Event", EditorStyles.boldLabel);
			using (var h = new EditorGUILayout.HorizontalScope ("Box")) {
				pp.eventRange[5] = (LevelPiece.EventRange)EditorGUILayout.EnumPopup ("Event Range", pp.eventRange[5]);
			}
			EditorGUILayout.Separator ();

			EditorGUILayout.LabelField ("Modified Component", EditorStyles.boldLabel);
			for (int i = 0; i < pp.PProperties.Length; i++) {
				if (pp.PProperties [i].tComponent != FocalComponent.Unused)
					GUI.color = (pp.PProperties [i].tObject == null) ? Color.red : Color.green;
				using (var v = new EditorGUILayout.VerticalScope (EditorStyles.helpBox)) {
					GUI.color = def;
					EditorGUI.BeginChangeCheck ();
					pp.PProperties [i].tComponent = (FocalComponent)EditorGUILayout.EnumPopup (
						"Type", pp.PProperties [i].tComponent);
					if (EditorGUI.EndChangeCheck ()) {
						//清除既有att參數
						pp.PProperties [i].tObject = null;
					}
					//判斷tComponent類型並指定相應處理
					switch (pp.PProperties [i].tComponent) {
					case FocalComponent.AddLootActor:
						DrawInsAddLootActor (i);
						break;

					case FocalComponent.EnemySpawner:
						DrawInsEnemySpawner (i);
						break;

					case FocalComponent.Unknown:
						DrawInsUnknown (i);
						break;

					default:
						EditorGUILayout.HelpBox ("↗ Select a componennt Type...", MessageType.None, true);
						break;
					}
				}
			}
			if (EditorGUI.EndChangeCheck ())
				EditorUtility.SetDirty (pp);
		}
		private void DrawInsAddLootActor (int _index)
		{
			pp.PProperties [_index].tObject = EditorGUILayout.ObjectField (
				"Target", pp.PProperties [_index].tObject, typeof(AddLootActor), true);
			if (pp.PProperties [_index].tObject != null) {
				AddLootActor obj = (AddLootActor)pp.PProperties [_index].tObject;
				EditorGUILayout.LabelField ("Modifiable Field : ",
					"Loot ID　(" + obj.m_lootID.ToString () + ")",
					EditorStyles.miniLabel);
			} else {
				DrawInsDragFirst ();
			}
		}
		private void DrawInsEnemySpawner (int _index)
		{
			pp.PProperties [_index].tObject = EditorGUILayout.ObjectField (
				"Target", pp.PProperties [_index].tObject, typeof(EnemySpawner), true);
			if (pp.PProperties [_index].tObject != null) {
				EnemySpawner obj = (EnemySpawner)pp.PProperties [_index].tObject;
				EditorGUILayout.LabelField ("Modifiable Field : ",
					"Enemy Type　(" + obj.m_enemyType.ToString () + ")\n" +
					"Spawner Data\n" +
					"　├─ Total Qty　(" + obj.m_spawnerData.m_totalQty.ToString () + ")\n" +
					"　├─ Max Live Qty　(" + obj.m_spawnerData.m_maxLiveQty.ToString () + ")\n" +
					"　├─ Spwn Count Per Time　(" + obj.m_spawnerData.m_spwnCountPerTime.ToString () + ")\n" +
					"　├─ Random Spawn X　(" + obj.m_spawnerData.m_randomSpawn.x.ToString () + ")\n" +
					"　└─ Random Spawn Y　(" + obj.m_spawnerData.m_randomSpawn.y.ToString () + ")",
					EditorStyles.miniLabel,
					GUILayout.Height (12 * 7));
				if(!Application.isPlaying)
				((EnemySpawner)pp.PProperties [_index].tObject).m_isStart = false;
			} else {
				DrawInsDragFirst ();
			}
		}
		private void DrawInsUnknown (int _index)
		{
			pp.PProperties [_index].tObject = EditorGUILayout.ObjectField (
				"Target", pp.PProperties [_index].tObject, typeof(object), true);
			if (pp.PProperties [_index].tObject != null) {
				EditorGUILayout.HelpBox ("實現夢想請洽二七...", MessageType.Warning, true);
			} else {
				DrawInsDragFirst ();
			}
		}
		private void DrawInsDragFirst()
		{
			EditorGUILayout.HelpBox ("↗ Drag a component into object field...", MessageType.None, true);
		}
		#endregion

		#region EditorGUI
        public override void OnEditorGUI(ref BlockItem item)
        {
			PropertyPiece pp = (PropertyPiece)target;
			Color def = GUI.contentColor;
			EditorGUI.BeginChangeCheck ();
			for (int i = 0; i < pp.PProperties.Length; i++) {
				EditorGUI.BeginDisabledGroup (pp.PProperties [i].tComponent == FocalComponent.Unused);
				using (var v = new EditorGUILayout.VerticalScope ("box")) {
					EditorGUILayout.LabelField (pp.PProperties [i].tComponent.ToString (), EditorStyles.boldLabel);
					switch (pp.PProperties [i].tComponent) {
					case FocalComponent.AddLootActor:
						if (pp.PProperties [i].tObject != null) {
							AddLootActor obj = (AddLootActor)pp.PProperties [i].tObject;
							EditorGUI.BeginChangeCheck ();
							obj.m_lootID = EditorGUILayout.IntField ("Loot ID", obj.m_lootID);
							if (EditorGUI.EndChangeCheck ()) {
								string _code = pp.PProperties [i].tComponent + ";" +
									obj.m_lootID.ToString ();
								item.attributes [i] = _code;
							}
						}
						break;

					case FocalComponent.EnemySpawner:
						if (pp.PProperties [i].tObject != null) {
							EnemySpawner obj = (EnemySpawner)pp.PProperties [i].tObject;
//							EditorGUI.BeginChangeCheck ();
							obj.m_enemyType = (EnemyType)EditorGUILayout.EnumPopup ("Enemy Type", obj.m_enemyType);
							EditorGUILayout.LabelField ("Spawner Data");
							EditorGUI.indentLevel++;
							obj.m_spawnerData.m_totalQty = EditorGUILayout.IntField ("Total Qty", obj.m_spawnerData.m_totalQty);
							obj.m_spawnerData.m_maxLiveQty = EditorGUILayout.IntField ("Max Live Qty", obj.m_spawnerData.m_maxLiveQty);
							obj.m_spawnerData.m_spwnCountPerTime = EditorGUILayout.IntField ("Spwn Count Per Time", obj.m_spawnerData.m_spwnCountPerTime);
							obj.m_spawnerData.m_randomSpawn = EditorGUILayout.Vector2Field ("Random Spawn", obj.m_spawnerData.m_randomSpawn);
							EditorGUI.indentLevel--;
//							if (EditorGUI.EndChangeCheck ()) {
								string _code = pp.PProperties [i].tComponent + ";" +
								               obj.m_enemyType.ToString () + ";" +
								               obj.m_spawnerData.m_totalQty.ToString () + ";" +
								               obj.m_spawnerData.m_maxLiveQty.ToString () + ";" +
								               obj.m_spawnerData.m_spwnCountPerTime.ToString () + ";" +
								               obj.m_spawnerData.m_randomSpawn.x.ToString () + "," + obj.m_spawnerData.m_randomSpawn.y.ToString ();
								item.attributes [i] = _code;
//							}
						}
						break;

					case FocalComponent.Unknown:
						if (pp.PProperties [i].tObject != null) {

						}
						break;

					default:
						if (item.attributes [i].Length > 0)
							item.attributes [i] = "";
						break;
					}
					EditorGUILayout.LabelField (item.attributes [i], EditorStyles.miniTextField);
				}
				EditorGUI.EndDisabledGroup ();
			}
			if (EditorGUI.EndChangeCheck ())
				EditorUtility.SetDirty (pp);
        }
		#endregion
    }
}
