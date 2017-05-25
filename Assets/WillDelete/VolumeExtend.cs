﻿using CreVox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CrevoxExtend {
	public enum ConnectionInfoType {
		StartingNode,
		Connection
	}
	public class ConnectionInfo {
		public WorldPos position;
		public Quaternion rotation;
		public ConnectionInfoType type;
		public string connectionName;
		public bool used;
		// Just for connection.
		public ConnectionInfo(WorldPos position, Quaternion rotation, ConnectionInfoType type, string name = "") {
			this.position = position;
			this.rotation = rotation;
			this.type = type;
			this.connectionName = name;
			used = false;
		}
		public ConnectionInfo(ConnectionInfo clone) {
			this.position = clone.position;
			this.rotation = clone.rotation;
			this.type = clone.type;
			this.connectionName = clone.connectionName;
			this.used = clone.used;
		}
		public ConnectionInfo Clone() {
			return new ConnectionInfo(this);
		}
		public bool Compare(ConnectionInfo obj) {
			return this.position.Compare(obj.position) && this.rotation == obj.rotation && this.type == obj.type && this.used == obj.used;
		}
		public WorldPos RelativePosition( float degree) {
			int absoluteDegree = ((int) (degree + this.rotation.eulerAngles.y) % 360);
			 return DirectionOffset[absoluteDegree / 90];
		}
		// Constant array.
		public static WorldPos[] DirectionOffset = new WorldPos[] {
			new WorldPos(0, 0, 1),
			new WorldPos(1, 0, 0),
			new WorldPos(0, 0, -1),
			new WorldPos(-1, 0, 0)
		};
	}
	public class VolumeExtend : MonoBehaviour {
		private List<ConnectionInfo> _connectionInfos;
		public List<ConnectionInfo> ConnectionInfos {
			get { return _connectionInfos; }
			set { _connectionInfos = value; }
		}
	}
}