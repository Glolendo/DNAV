using System;
using System.Collections.Generic;
using UnityEngine;

public class LayoutBuilderSimple : MonoBehaviour
{
    [Header("Source Layout JSON (from LayoutExporter)")]
    public TextAsset layoutJson;

    [Header("Wall Settings")]
    public GameObject wallPrefab;   // optional – if null, will use Cube
    public float wallThickness = 0.15f;
    public float wallHeight = 2.8f;
    public bool snapToGrid = true;
    public float gridSize = 0.5f;   // we’ll use 0.5 like you chose

    [Header("Build Options")]
    public bool clearExistingChildren = true;

    [Serializable]
    public class XForm
    {
        public string name;
        public string path;
        public float px, py, pz;
        public float ry;
        public float sx, sy, sz;
        public string prefabHint;
        public string tag;
        public int layer;
    }

    [Serializable]
    public class Group
    {
        public string groupName;
        public List<XForm> items;
    }

    [Serializable]
    public class LayoutDump
    {
        public string sceneName;
        public string exportedAt;
        public float gridSize;
        public float wallThicknessDefault;
        public List<Group> groups;
        public string notes;
    }

    [ContextMenu("Rebuild Layout (Simple)")]
    public void Rebuild()
    {
        if (layoutJson == null)
        {
            Debug.LogError("LayoutBuilderSimple: No layoutJson assigned.");
            return;
        }

        if (clearExistingChildren)
        {
#if UNITY_EDITOR
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
#else
            foreach (Transform child in transform)
                Destroy(child.gameObject);
#endif
        }

        var data = JsonUtility.FromJson<LayoutDump>(layoutJson.text);
        if (data == null || data.groups == null || data.groups.Count == 0)
        {
            Debug.LogError("LayoutBuilderSimple: Failed to parse layout JSON.");
            return;
        }

        if (data.gridSize > 0) gridSize = data.gridSize;

        int wallCount = 0;

        foreach (var g in data.groups)
        {
            if (!string.Equals(g.groupName, "Walls", StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var it in g.items)
            {
              // Treat anything with a decent X or Z size as a wall.
    // This skips tiny parents/markers but includes all real wall cubes.
    float maxDim = Mathf.Max(Mathf.Abs(it.sx), Mathf.Abs(it.sz));
    if (maxDim < 0.2f) continue;   // ignore tiny transforms

    BuildWallFromXForm(it);
    wallCount++;
            }
        }

        Debug.Log($"LayoutBuilderSimple: Rebuilt {wallCount} walls.");
    }

    void BuildWallFromXForm(XForm it)
    {
        // Determine orientation from scale (more robust than rotation)
        bool horizontal = it.sx >= it.sz;
        float length = horizontal ? Mathf.Abs(it.sx) : Mathf.Abs(it.sz);
if (length < 0.2f) return; // ignore microscopic pieces


        // Original endpoints
        
        float centerX = it.px;
        float centerZ = it.pz;

        // Snap center + endpoints to grid if desired
        if (snapToGrid && gridSize > 0)
        {
            // compute endpoints, snap them, then recompute center+length
            if (horizontal)
            {
                float half = length * 0.5f;
                float startX = centerX - half;
                float endX = centerX + half;

                startX = Mathf.Round(startX / gridSize) * gridSize;
                endX = Mathf.Round(endX / gridSize) * gridSize;

                length = Mathf.Max(gridSize, Mathf.Abs(endX - startX));
                centerX = (startX + endX) * 0.5f;
            }
            else
            {
                float half = length * 0.5f;
                float startZ = centerZ - half;
                float endZ = centerZ + half;

                startZ = Mathf.Round(startZ / gridSize) * gridSize;
                endZ = Mathf.Round(endZ / gridSize) * gridSize;

                length = Mathf.Max(gridSize, Mathf.Abs(endZ - startZ));
                centerZ = (startZ + endZ) * 0.5f;
            }

            centerX = Mathf.Round(centerX / gridSize) * gridSize;
            centerZ = Mathf.Round(centerZ / gridSize) * gridSize;
        }

        Vector3 pos;
        Vector3 scale;
        Quaternion rot;

        if (horizontal)
        {
            pos = new Vector3(centerX, wallHeight * 0.5f, centerZ);
            scale = new Vector3(length, wallHeight, wallThickness);
            rot = Quaternion.identity;
        }
        else
        {
            pos = new Vector3(centerX, wallHeight * 0.5f, centerZ);
            scale = new Vector3(wallThickness, wallHeight, length);
            rot = Quaternion.identity;
        }

        GameObject go;
        if (wallPrefab != null)
        {
#if UNITY_EDITOR
            go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(wallPrefab);
#else
            go = Instantiate(wallPrefab);
#endif
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        go.name = "Wall";
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.localScale = scale;

        // Collider safety
        var col = go.GetComponent<Collider>();
        if (!col) go.AddComponent<BoxCollider>();
    }
}
