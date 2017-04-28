﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreVox;
using MissionGrammarSystem;
using System;
using System.Linq;

namespace CrevoxExtend {
	public class VolumeDataTransform {
		private static List<Guid> _alphabetIDs = new List<Guid>();
		private static Dictionary<Guid, List<VolumeData>> _refrenceTable = new Dictionary<Guid, List<VolumeData>>();

		private static List<List<VolumeData>> sameVolumeDatas = new List<List<VolumeData>>();
		public static List<Guid> AlphabetIDs {
			get { return _alphabetIDs; }
			set { _alphabetIDs = value; }
		}
		public static Dictionary<Guid, List<VolumeData>> RefrenceTable {
			get { return _refrenceTable; }
			set { _refrenceTable = value; }
		}
		public static List<List<VolumeData>> SameVolumeDatas {
			get { return sameVolumeDatas; }
			set { sameVolumeDatas = value; }
		}
		public static void InitialTable() {
			_refrenceTable = new Dictionary<Guid, List<VolumeData>>();
			for (int i = 0; i < _alphabetIDs.Count; i++) {
				_refrenceTable[_alphabetIDs[i]] = sameVolumeDatas[i];
			}
		}
		private static List<CreVoxNode> _usedNode;
		// Generate the volume data that refer graph grammar.
		public static void Generate() {
			_usedNode = new List<CreVoxNode>();
			// Get root.
			CreVoxNode root = CreVoxAttach.RootNode;
			// Initial root.
			Volume volume = CrevoxOperation.InitialVolume(SelectData(_refrenceTable[root.AlphabetID]));
			_usedNode.Add(root);
			if (GenerateRecursion(root, volume)) {
				Debug.Log("Successful.");
				// Update volume manager and scene.
				CrevoxOperation.RefreshVolume();
			} else {
				Debug.Log("Error");
			}
		}
		// Dfs generate.
		private static bool GenerateRecursion(CreVoxNode node, Volume volumeOrigin) {
			foreach (var child in node.Children) {
				if (_usedNode.Exists(n => n.SymbolID == child.SymbolID)) {
					continue;
				}
				Volume volume = null;
				// Find the suitable vdata by random ordering.
				foreach (var vdata in _refrenceTable[child.AlphabetID].OrderBy( x=> UnityEngine.Random.value)) {
					volume = CrevoxOperation.CreateVolumeObject(vdata);
					if(volume.GetComponent<VolumeExtend>().ConnectionInfos.Count - 1 >= child.Children.Count) {
						Debug.Log("Find the count is " + volume.GetComponent<VolumeExtend>().ConnectionInfos.Count);
						break;
					} else {
						// Cannot connect, so delete it.
						MonoBehaviour.DestroyImmediate(volume.gameObject);
						volume = null;
					}
				}
				// No vdata have enough connection.
				if(volume == null) {
					Debug.Log("There is no vdata that have enough connection. It means this graph  doesn't match with vdata.");
					return false;
				}
				// Combine.
				if (CrevoxOperation.CombineVolumeObject(volumeOrigin, volume)) {
					_usedNode.Add(child);
					if (!GenerateRecursion(child, volume)) {
						return false;
					}
				} else {
					MonoBehaviour.DestroyImmediate(volume.gameObject);
					return false;
				}
			}
			return true;
		}
		// [TEST] Will delete.
		public static void RandomGenerate(int count) {
			List<Volume> vols = new List<Volume>();
			Volume volume = CrevoxOperation.InitialVolume(SelectData(sameVolumeDatas[UnityEngine.Random.Range(0, sameVolumeDatas.Count)]));
			vols.Add(volume);
			while (--count > 0) {
				volume = CrevoxOperation.AddAndCombineVolume(volume, SelectData(sameVolumeDatas[UnityEngine.Random.Range(0, sameVolumeDatas.Count)]));
				if (volume != null) {
					vols.Add(volume);
				} else {
					volume = vols[UnityEngine.Random.Range(0, vols.Count)];
				}
			}
			CrevoxOperation.RefreshVolume();
		}
		// Random select from multi vDatas.
		private static VolumeData SelectData(List<VolumeData> sameRoomData) {
				return sameRoomData[(int)UnityEngine.Random.Range(0, sameRoomData.Count)];
		}

	}
}
