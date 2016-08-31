﻿using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CreVox
{

	[CustomEditor (typeof(World))]
	public class WorldEditor : Editor
	{
		World world;
		Dictionary<WorldPos, Chunk> dirtyChunks = new Dictionary<WorldPos, Chunk> ();
		int cx = 1;
		int cy = 1;
		int cz = 1;

		string rularTag = "VoxelEditorBase";
		WorldPos workpos;
		//VoxelLayer------
		private int fixY = 0;
		private bool showPointer = true;
		//----------------

		public enum EditMode
		{
			View,
			Voxel,
			VoxelLayer,
			Object,
		}

		private EditMode selectedEditMode;
		private EditMode currentEditMode;

		private PaletteItem _itemSelected;
		private Texture2D _itemPreview;
		private LevelPiece _pieceSelected;

		public void OnEnable ()
		{
			world = (World)target;
			SubscribeEvents ();
		}

		private void OnDisable ()
		{
			UnsubscribeEvents ();
		}

		void OnSceneGUI ()
		{
			DrawModeGUI ();
			ModeHandler ();
			EventHandler ();
		}

		public override void OnInspectorGUI ()
		{
			//修改成適應視窗寬度
			float lw = 60;
			float w = (Screen.width - 20 - lw) / 3 - 8;
			EditorGUIUtility.labelWidth = 20;

			GUILayout.BeginVertical (EditorStyles.helpBox, GUILayout.Width (Screen.width - 20));
			EditorGUILayout.LabelField ("Chunk setting", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Count", GUILayout.Width (lw));
			cx = EditorGUILayout.IntField ("X", cx, GUILayout.Width (w));
			cy = EditorGUILayout.IntField ("Y", cy, GUILayout.Width (w));
			cz = EditorGUILayout.IntField ("Z", cz, GUILayout.Width (w));
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Init")) {
				world.Reset ();
				world.Init (cx, cy, cz);
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox, GUILayout.Width (Screen.width - 20));
			EditorGUILayout.LabelField ("Save & Load", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Save")) {
				world.workFile = Serialization.GetSaveLocation ();
				Serialization.SaveWorld (world,world.workFile);
			}
			if (GUILayout.Button ("Load")) {
				world.workFile = Serialization.GetLoadLocation ();
				Save save = Serialization.LoadWorld (world.workFile);
				if (save != null)
					world.BuildWorld (save);
				SceneView.RepaintAll ();
			}
			if (GUILayout.Button ("Test")) {
				Serialization.SaveWorld (world, "Assets/Resources/testmap.bytes");
				world.Reset ();
				EditorApplication.isPlaying = true;
			}
			GUILayout.EndHorizontal ();
			EditorGUILayout.LabelField (world.workFile);
			GUILayout.EndVertical ();

			DrawPieceSelectedGUI ();

			if (GUI.changed) {
				EditorUtility.SetDirty (world);
			}
		}

		private void UpdateCurrentPieceInstance (PaletteItem item, Texture2D preview)
		{
			_itemSelected = item;
			_itemPreview = preview;
			_pieceSelected = (LevelPiece)item.GetComponent<LevelPiece> ();
			Repaint ();
		}

		private void SubscribeEvents ()
		{
			PaletteWindow.ItemSelectedEvent += new PaletteWindow.itemSelectedDelegate (UpdateCurrentPieceInstance);
		}

		private void UnsubscribeEvents ()
		{
			PaletteWindow.ItemSelectedEvent -= new PaletteWindow.itemSelectedDelegate (UpdateCurrentPieceInstance);
		}

		private void DrawPieceSelectedGUI ()
		{
			GUILayout.BeginVertical (EditorStyles.helpBox, GUILayout.Width (Screen.width - 20));
			EditorGUILayout.LabelField ("Piece Selected", EditorStyles.boldLabel);
			if (_pieceSelected == null) {
				EditorGUILayout.HelpBox ("No piece selected!", MessageType.Info);
			} else {
				EditorGUILayout.BeginVertical ("box");
				EditorGUILayout.LabelField (new GUIContent (_itemPreview), GUILayout.Height (40));
				EditorGUILayout.LabelField (_itemSelected.itemName);
				EditorGUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
		}

		private void DrawModeGUI ()
		{
			List<EditMode> modes = EditorUtils.GetListFromEnum<EditMode> ();
			List<string> modeLabels = new List<string> ();
			foreach (EditMode mode in modes) {
				modeLabels.Add (mode.ToString ());
			}
			float ButtonW = 90;

			Handles.BeginGUI ();
			GUILayout.BeginArea (new Rect (10f, 10f, modeLabels.Count * ButtonW, 60f), "", "Box"); //根據選項數量決定寬度
			selectedEditMode = (EditMode)GUILayout.Toolbar ((int)currentEditMode, modeLabels.ToArray (), GUILayout.ExpandHeight (true));
			EditorGUILayout.BeginHorizontal ();
			world.editDis = EditorGUILayout.Slider ("Edibable Distance", world.editDis, 90f, 1000f);
			EditorGUILayout.EndHorizontal ();
			GUILayout.EndArea ();

			//VoxelLayer------
			GUILayout.BeginArea (new Rect (10f, 75f, ButtonW * 2 + 10, 65f));
			DrawLayerModeGUI ();
			GUILayout.EndArea ();
			//----------------
			Handles.EndGUI ();
		}

		//VoxelLayer------
		private void DrawLayerModeGUI ()
		{
			if (selectedEditMode == EditMode.VoxelLayer) {
				GUI.color = new Color (world.YColor.r, world.YColor.g, world.YColor.b, 1.0f);
				EditorGUILayout.BeginHorizontal ("Box", GUILayout.Width (90));
				GUI.color = Color.white;
				EditorGUILayout.BeginVertical ();
				if (GUILayout.Button ("▲", GUILayout.Width (85))) {
					fixY++;
					world.ChangeEditY (fixY);
					fixY = world.editY;
				}
				EditorGUILayout.LabelField ("","Layer : "+ world.editY, "TextField", GUILayout.Width (85));
				if (GUILayout.Button ("▼", GUILayout.Width (85))) {
					fixY--;
					world.ChangeEditY (fixY);
					fixY = world.editY;
				}
				EditorGUILayout.EndVertical ();

				if (GUILayout.Button (showPointer ? "Hide\n Pointer" : "Show\n Pointer", GUILayout.ExpandHeight (true))) {
					showPointer = !showPointer;
				}
				EditorGUILayout.EndHorizontal ();
				
				world.pointer = showPointer ? true : false;
			} else
				world.pointer = false;
		}
		//----------------

		private void ModeHandler ()
		{
			switch (selectedEditMode) {
			case EditMode.Voxel:
			case EditMode.VoxelLayer: //VoxelLayer------
			case EditMode.Object:
				Tools.current = Tool.None;
                
				break;
			case EditMode.View:
			default:
				Tools.current = Tool.View;
                    //Tools.viewTool = ViewTool.Orbit;
				break;
			}
			if (selectedEditMode != currentEditMode) {
				currentEditMode = selectedEditMode;
				Repaint ();
			}
		}

		private void EventHandler ()
		{
			HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
			int button = Event.current.button;
		
			if (!Event.current.alt) {
				//BoxCursor------
				switch (currentEditMode) {
				case EditMode.Voxel:
				case EditMode.VoxelLayer: 
					world.useBox = true;
					break;

				case EditMode.Object:
				case EditMode.View:
				default:
					world.useBox = false;
					break;
				}
				//---------------
				switch (currentEditMode) {
				case EditMode.Voxel:
					if (button == 0)
						DrawMarker (false);
					else if (button <= 1) {
						DrawMarker (true);
					}
					if (Event.current.type == EventType.MouseDown /*|| Event.current.type == EventType.MouseDrag*/) {
						if (button == 0)
							Paint (false);
						else if (button == 1) {
							Paint (true);
							Tools.viewTool = ViewTool.None;
							Event.current.Use ();
						}
					}
					if (Event.current.type == EventType.MouseUp) {
						UpdateDirtyChunks ();
					}               
					break;

				//VoxelLayer------
				case EditMode.VoxelLayer: 
					DrawLayerMarker ();

					if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
						if (button == 0)
							PaintLayer (false);
						else if (button == 1) {
							PaintLayer (true);
							Tools.viewTool = ViewTool.None;
							Event.current.Use ();
						}
					}
					if (Event.current.shift) {
						if (Event.current.type == EventType.ScrollWheel) {
							if (Event.current.delta.y < 0)
								fixY++;
							if (Event.current.delta.y > 0)
								fixY--;
							world.ChangeEditY (fixY);
							fixY = world.editY;
							Event.current.Use ();
						}
					}

					if (Event.current.type == EventType.MouseUp) {
						UpdateDirtyChunks ();
					}
					break;
				//----------------

				case EditMode.Object:
					if (Event.current.type == EventType.MouseDown) {
						if (button == 0)
							PaintPieces (false);
						else if (button == 1) {
							PaintPieces (true);
							Tools.viewTool = ViewTool.None;
							Event.current.Use ();
						}
					}
					DrawGridMarker ();
                
					break;

				case EditMode.View:
				default:
					break;
				}
			}
		}

		private void DrawMarker (bool isErase)
		{
			//update = true;
			RaycastHit hit;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			if (Physics.Raycast (worldRay, out hit, world.editDis, 1 << LayerMask.NameToLayer ("Editor")) && !isErase) {
				WorldPos pos = EditTerrain.GetBlockPos (hit, isErase ? false : true);
				float x = pos.x * Block.w;
				float y = pos.y * Block.h;
				float z = pos.z * Block.d;
				//float x = Mathf.FloorToInt((hitInfo.point.x + Block.hw) / Block.w) * Block.w;

				//BoxCursor------
				//Handles.CubeCap(0, new Vector3(x, y, z), Quaternion.identity, 2f);
				if (hit.collider.gameObject.tag == rularTag) {
					hit.normal = Vector3.zero;
				}
				BoxCursorUtils.UpdateBox (world.box, new Vector3 (x, y, z), hit.normal);
				//---------------
				SceneView.RepaintAll ();
			} else {
				world.useBox = false;
				SceneView.RepaintAll ();
			}
		}

		//VoxelLayer------
		private void DrawLayerMarker ()
		{
			//update = true;
			RaycastHit hit;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			if (Physics.Raycast (worldRay, out hit, world.editDis, 1 << LayerMask.NameToLayer ("EditorLevel"))) {
				hit.point += new Vector3 (0f, -Block.h, 0f);
				WorldPos pos = EditTerrain.GetBlockPos (hit, true);
				float x = pos.x * Block.w;
				float y = pos.y * Block.h;
				float z = pos.z * Block.d;
				//BoxCursor------
				//Handles.CubeCap(0, new Vector3(x, y, z), Quaternion.identity, 2f);
				world.useBox = true;
				BoxCursorUtils.UpdateBox (world.box, new Vector3 (x, y, z), Vector3.zero);
				SceneView.RepaintAll ();
				//---------------
			} else {
				world.useBox = false;
				SceneView.RepaintAll ();
			}
		}
		//----------------

		private void DrawGridMarker ()
		{
			if (_pieceSelected == null)
				return;
			
			RaycastHit hit;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

			if (Physics.Raycast (worldRay, out hit, world.editDis, 1 << LayerMask.NameToLayer ("Editor"))) {
				if (hit.normal.y <= 0)
					return;
//				if (hit.collider.gameObject.tag == rularTag) return; 

				WorldPos pos = EditTerrain.GetBlockPos (hit, true);
				WorldPos gPos = EditTerrain.GetGridPos (hit.point);
				gPos.y = 0;
				Handles.color = Color.white;
				Handles.RectangleCap (0, new Vector3 (pos.x * Block.w, pos.y * Block.h -1, pos.z * Block.d), Quaternion.Euler (90, 0, 0), Block.hw);
				float x = pos.x * Block.w + gPos.x - 1;
				float y = pos.y * Block.h + gPos.y - 1;
				float z = pos.z * Block.d + gPos.z - 1;

				LevelPiece.PivotType pivot = (_pieceSelected.isStair) ? LevelPiece.PivotType.Edge : _pieceSelected.pivot;
				if (CheckPlaceable ((int)gPos.x, (int)gPos.z, pivot)) {
					Handles.color = Color.red;
					Handles.RectangleCap (0, new Vector3 (x, y, z), Quaternion.Euler (90, 0, 0), 0.5f);
					Handles.color = Color.white;
				}
				SceneView.RepaintAll ();
			}
		}

		private void Paint (bool isErase)
		{
			RaycastHit gHit;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			bool isHit = Physics.Raycast (worldRay, out gHit, world.editDis, 1 << LayerMask.NameToLayer ("Editor"));
			WorldPos pos;

			if (isHit) {
				pos = EditTerrain.GetBlockPos (gHit, isErase ? false : true);

				world.SetBlock (pos.x, pos.y, pos.z, isErase ? new BlockAir () : new Block ());
				Chunk chunk = world.GetChunk (pos.x, pos.y, pos.z);

				if (chunk) {
					if (!dirtyChunks.ContainsKey (pos))
						dirtyChunks.Add (pos, chunk);
					chunk.UpdateMeshFilter ();
					SceneView.RepaintAll ();
				}
			}
		}

		//VoxelLayer------
		private void PaintLayer (bool isErase)
		{
			RaycastHit gHit;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			bool isHit = Physics.Raycast (worldRay, out gHit, world.editDis, 1 << LayerMask.NameToLayer ("EditorLevel"));
			WorldPos pos;

			if (isHit) {
				gHit.point = gHit.point + new Vector3 (0f, -Block.h, 0f);
				pos = EditTerrain.GetBlockPos (gHit, true);

				world.SetBlock (pos.x, pos.y, pos.z, isErase ? new BlockAir () : new Block ());
				Chunk chunk = world.GetChunk (pos.x, pos.y, pos.z);

				if (chunk) {
					if (!dirtyChunks.ContainsKey (pos))
						dirtyChunks.Add (pos, chunk);
					chunk.UpdateMeshFilter ();
					SceneView.RepaintAll ();
				}
			}
		}
		//----------------

		private void PaintPieces (bool isErase)
		{
			if (_pieceSelected == null)
				return;
			RaycastHit gHit;
			bool canPlace = false;
			Ray worldRay = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			bool isHit = Physics.Raycast (worldRay, out gHit, world.editDis, 1 << LayerMask.NameToLayer ("Editor"));

			if (isHit) {
				if (gHit.normal.y <= 0)
					return;
//				if (gHit.collider.gameObject.tag == rularTag)
//					return; 
				WorldPos bPos = EditTerrain.GetBlockPos (gHit, true);
				WorldPos gPos = EditTerrain.GetGridPos (gHit.point);
				gPos.y = 0;
				int gx = gPos.x;
				int gz = gPos.z;
				if (_pieceSelected.isStair) {
					if (CheckPlaceable (gx, gz, LevelPiece.PivotType.Edge)) {
						if (gPos.x == 0 && gPos.z == 1)
							bPos.x -= 1;
						if (gPos.x == 2 && gPos.z == 1)
							bPos.x += 1;
						if (gPos.x == 1 && gPos.z == 0)
							bPos.z -= 1;
						if (gPos.x == 1 && gPos.z == 2)
							bPos.z += 1;
						bPos.y -= 1;
						canPlace = true;
					}
				}
				if (CheckPlaceable (gx, gz, _pieceSelected.pivot)) {
					canPlace = true;
				}

				if (canPlace) {
					world.PlacePiece (bPos, gPos, isErase ? null : _pieceSelected);
					SceneView.RepaintAll ();
				}
			}
		}

		private void UpdateDirtyChunks ()
		{
			foreach (KeyValuePair<WorldPos, Chunk> c in dirtyChunks) {
				c.Value.UodateMeshCollider ();
			}
			dirtyChunks.Clear ();
		}

		private bool CheckPlaceable (int x, int z, LevelPiece.PivotType pType)
		{
			if (pType == LevelPiece.PivotType.Grid)
				return true;
			else if (pType == LevelPiece.PivotType.Center && (x * z) == 1)
				return true;
			else if (pType == LevelPiece.PivotType.Vertex && (x + z) % 2 == 0 && x * z != 1)
				return true;
			else if (pType == LevelPiece.PivotType.Edge && (x + z) % 2 == 1)
				return true;

			return false;
		}
	}
}