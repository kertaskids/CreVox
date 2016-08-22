﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SelectionBase]
public class World : MonoBehaviour {
    public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
    public int chunkX = 1;
    public int chunkY = 1;
    public int chunkZ = 1;
    public GameObject chunkPrefab;
    public GameObject ruler;
	public MeshCollider mColl;

	//VoxelLayer------
	public GameObject layerRuler; 
	public BoxCollider bColl;
	public int editY;
	public bool pointer;
	public Color YColor;
	//----------------

    public void Init()
    {
        CreateRuler();
		CreateLevelRuler();
        CreateChunks();
    }

    void CreateRuler()
    {
        ruler = new GameObject("Ruler");
        ruler.layer = LayerMask.NameToLayer("Editor");
        ruler.transform.parent = transform;
		mColl = ruler.AddComponent<MeshCollider>();
		mColl.hideFlags = HideFlags.HideInHierarchy;

        MeshData meshData = new MeshData();
        float x = -Block.hw;
        float y = -Block.hh;
        float z = -Block.hd;
        float w = chunkX * Chunk.chunkSize * Block.w+x;
        float d = chunkZ * Chunk.chunkSize * Block.d+z;
        meshData.useRenderDataForCol = true;
        meshData.AddVertex(new Vector3(x, 0, z));
        meshData.AddVertex(new Vector3(x, 0, d));
        meshData.AddVertex(new Vector3(w, 0, d));
        meshData.AddVertex(new Vector3(w, 0, z));
        meshData.AddQuadTriangles();

		mColl.sharedMesh = null;
        Mesh cmesh = new Mesh();
        cmesh.vertices = meshData.colVertices.ToArray();
        cmesh.triangles = meshData.colTriangles.ToArray();
        cmesh.RecalculateNormals();

		mColl.sharedMesh = cmesh;
    }

    void CreateChunks()
    {
        for (int x = 0; x < chunkX; x++)
        {
            for (int y = 0; y < chunkY; y++)
            {
                for (int z = 0; z < chunkZ; z++)
                {
                    CreateChunk(x * 16, y * 16, z * 16);
                    GetChunk(x * 16, y * 16, z * 16).Init();
                }
            }
        }
    }

    public void CreateChunk(int x, int y, int z)
    {
        WorldPos worldPos = new WorldPos(x, y, z);

        //Instantiate the chunk at the coordinates using the chunk prefab
        GameObject newChunkObject = Instantiate(
                        chunkPrefab, new Vector3(x * Block.w, y * Block.h, z * Block.d),
                        Quaternion.Euler(Vector3.zero)
                    ) as GameObject;
        newChunkObject.transform.parent = transform;
		newChunkObject.name = "Chunk("+x/16+","+y/16+","+z/16+")" ;

        Chunk newChunk = newChunkObject.GetComponent<Chunk>();

        newChunk.pos = worldPos;
        newChunk.world = this;

        //Add it to the chunks dictionary with the position as the key
        chunks.Add(worldPos, newChunk);

        //Add the following:
        for (int xi = 0; xi < 16; xi++)
        {
            for (int yi = 0; yi < 16; yi++)
            {
                for (int zi = 0; zi < 16; zi++)
                {
                    SetBlock(x + xi, y + yi, z + zi, new BlockAir());
                }
            }
        }
    }

    public void DestroyChunk(int x, int y, int z)
    {
        Chunk chunk = null;
        if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
        {
            Object.Destroy(chunk.gameObject);
            chunks.Remove(new WorldPos(x, y, z));
        }
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        WorldPos pos = new WorldPos();
        float multiple = Chunk.chunkSize;
        pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;
        Chunk containerChunk = null;
        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;
    }
    public Block GetBlock(int x, int y, int z)
    {
        Chunk containerChunk = GetChunk(x, y, z);
        if (containerChunk != null)
        {
            Block block = containerChunk.GetBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }
        else
        {
            //return new BlockAir();
            return null;
        }

    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        Chunk chunk = GetChunk(x, y, z);

        if (chunk != null)
        {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;
        }
	}

	void OnDrawGizmos()
	{
		float x = -Block.hw;
		float z = -Block.hd;
		float w = chunkX * Chunk.chunkSize * Block.w + x;
		float d = chunkZ * Chunk.chunkSize * Block.d + z;
		Vector3 v1 = new Vector3 (x, -Block.hh, z);
		Vector3 v2 = new Vector3 (x, -Block.hh, d);
		Vector3 v3 = new Vector3 (w, -Block.hh, d);
		Vector3 v4 = new Vector3 (w, -Block.hh, z);
		Gizmos.DrawLine (v1, v2);
		Gizmos.DrawLine (v2, v3);
		Gizmos.DrawLine (v3, v4);
		Gizmos.DrawLine (v4, v1);
		//VoxelLayer------
		Gizmos.color = YColor;
		if (pointer) {
				DrawGizmoLayer (editY);
		}
		Gizmos.DrawWireCube(
			transform.position + new Vector3(
				chunkX * 16f * Block.hw - Block.hw,
				chunkY * 16f * Block.hh - Block.hh, 
				chunkZ * 16f * Block.hd - Block.hd),
			new Vector3(
				chunkX * 16f * Block.w, 
				chunkY * 16f * Block.h, 
				chunkZ * 16f * Block.d)
		);
		//----------------
	}

	//VoxelLayer------
	void CreateLevelRuler()
	{
		YColor = new Color ((20 + (editY % 10) * 20) / 255f, (200 - Mathf.Abs ((editY % 10) - 5) * 20) / 255f, (200 - (editY % 10) * 20) / 255f, 0.4f);
		layerRuler = new GameObject ("LevelRuler");
		layerRuler.layer = LayerMask.NameToLayer ("EditorLevel");
		layerRuler.transform.parent = transform;
		bColl = layerRuler.AddComponent<BoxCollider> ();
		bColl.size = new Vector3 (chunkX * 16f * Block.w, 0f, chunkZ * 16f * Block.d);
		bColl.hideFlags = HideFlags.HideInHierarchy;
		ChangeEditY (0);
	}

	public void DrawGizmoLayer(int _y)
	{
		Gizmos.color = YColor;
		Gizmos.DrawCube (
			transform.position + new Vector3(
				chunkX * 16f * Block.hw - Block.hw,
				editY * Block.h, 
				chunkZ * 16f * Block.hd - Block.hd),
			new Vector3(
				chunkX * 16f * Block.w, 
				Block.h+0.1f, 
				chunkZ * 16f * Block.d)
		);
	}

	public void ChangeEditY(int _y) {
		editY = _y;
		YColor = new Color (
			(20  + (editY % 10) * 20) / 255f, 
			(200 - Mathf.Abs ((editY % 10) - 5) * 20) / 255f, 
			(200 - (editY % 10) * 20) / 255f, 
			0.4f
		);
		bColl.center = transform.position + new Vector3 (
			chunkX * 16f * Block.hw - Block.hw , 
			editY * Block.h, 
			chunkZ * 16f * Block.hd - Block.hd
		);
	}
	//----------------
}
