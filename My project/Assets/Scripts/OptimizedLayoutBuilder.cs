using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptimizedLayoutBuilder : MonoBehaviour
{
    [Header("Source Layout JSON (from LayoutExporter)")]
    public TextAsset layoutJson;

    [Header("Wall Settings")]
    public GameObject wallPrefab;    // optional: leave null to use Cube primitives
    public float wallThickness = 0.15f;
    public float wallHeight = 2.8f;
    public float mergeTolerance = 0.05f;     // max gap to merge
    public float minSegmentLength = 0.3f;    // scrap tiny slivers

    [Header("Build Options")]
    public bool clearExistingChildren = true;
    public bool snapToGrid = true;

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

    private enum Orientation { Horizontal, Vertical }

    private class WallSegment
    {
        public Orientation orientation;
        public float lineCoord; // z for horizontal, x for vertical
        public float start;     // x for horizontal, z for vertical
        public float end;
    }

    [ContextMenu("Rebuild Optimized Layout")]
    public void Rebuild()
    {
        if (layoutJson == null)
        {
            Debug.LogError("OptimizedLayoutBuilder: No layoutJson assigned.");
            return;
        }

        // Clear old generated walls
        if (clearExistingChildren)
        {
#if UNITY_EDITOR
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
#else
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
#endif
        }

        // Parse JSON
        var data = JsonUtility.FromJson<LayoutDump>(layoutJson.text);
        if (data == null || data.groups == null || data.groups.Count == 0)
        {
            Debug.LogError("OptimizedLayoutBuilder: Failed to parse layout JSON.");
            return;
        }

        float grid = data.gridSize > 0 ? data.gridSize : 0.5f;

        // Collect raw wall segments
        var rawSegments = new List<WallSegment>();

        foreach (var g in data.groups)
        {
            if (!string.Equals(g.groupName, "Walls", StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var it in g.items)
            {
                // Skip non-wall containers (like room parents)
                if (!it.name.ToLower().Contains("wall"))
                    continue;

                // Determine orientation by rotation
                float ryNorm = Mathf.Repeat(it.ry, 360f);
                Orientation? orient = null;

                if (Mathf.Abs(ryNorm - 0f) < 5f || Mathf.Abs(ryNorm - 180f) < 5f)
                    orient = Orientation.Horizontal;
                else if (Mathf.Abs(ryNorm - 90f) < 5f || Mathf.Abs(ryNorm - 270f) < 5f)
                    orient = Orientation.Vertical;

                if (!orient.HasValue)
                    continue; // ignore weird angles

                if (orient.Value == Orientation.Horizontal)
                {
                    float halfLen = it.sx * 0.5f;
                    float start = it.px - halfLen;
                    float end = it.px + halfLen;
                    float z = it.pz;

                    if (snapToGrid)
                    {
                        z = Mathf.Round(z / grid) * grid;
                        start = Mathf.Round(start / grid) * grid;
                        end = Mathf.Round(end / grid) * grid;
                    }

                    if (Mathf.Abs(end - start) >= minSegmentLength)
                    {
                        rawSegments.Add(new WallSegment
                        {
                            orientation = Orientation.Horizontal,
                            lineCoord = z,
                            start = Mathf.Min(start, end),
                            end = Mathf.Max(start, end)
                        });
                    }
                }
                else // Vertical
                {
                    float halfLen = it.sz * 0.5f;
                    float start = it.pz - halfLen;
                    float end = it.pz + halfLen;
                    float x = it.px;

                    if (snapToGrid)
                    {
                        x = Mathf.Round(x / grid) * grid;
                        start = Mathf.Round(start / grid) * grid;
                        end = Mathf.Round(end / grid) * grid;
                    }

                    if (Mathf.Abs(end - start) >= minSegmentLength)
                    {
                        rawSegments.Add(new WallSegment
                        {
                            orientation = Orientation.Vertical,
                            lineCoord = x,
                            start = Mathf.Min(start, end),
                            end = Mathf.Max(start, end)
                        });
                    }
                }
            }
        }

        if (rawSegments.Count == 0)
        {
            Debug.LogWarning("OptimizedLayoutBuilder: No wall segments found.");
            return;
        }

        // Group by orientation + line (z or x) and merge overlapping segments
        var merged = MergeSegments(rawSegments, grid);

        // Instantiate walls
        foreach (var seg in merged)
        {
            float length = seg.end - seg.start;
            if (length < minSegmentLength) continue;

            Vector3 pos;
            Vector3 scale;
            Quaternion rot;

            if (seg.orientation == Orientation.Horizontal)
            {
                float midX = (seg.start + seg.end) * 0.5f;
                pos = new Vector3(midX, wallHeight * 0.5f, seg.lineCoord);
                scale = new Vector3(length, wallHeight, wallThickness);
                rot = Quaternion.identity;
            }
            else // Vertical
            {
                float midZ = (seg.start + seg.end) * 0.5f;
                pos = new Vector3(seg.lineCoord, wallHeight * 0.5f, midZ);
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

            // Make sure it has a collider
            var col = go.GetComponent<Collider>();
            if (!col) go.AddComponent<BoxCollider>();
        }

        Debug.Log($"OptimizedLayoutBuilder: Built {merged.Count} merged walls from {rawSegments.Count} raw segments.");
    }

    private List<WallSegment> MergeSegments(List<WallSegment> input, float grid)
    {
        var result = new List<WallSegment>();

        // Group by orientation, then by lineCoord bucket
        var groups = input
            .GroupBy(s => s.orientation)
            .SelectMany(og =>
            {
                return og.GroupBy(s => Mathf.Round(s.lineCoord / grid) * grid,
                                  s => s,
                                  (line, segs) => new { orientation = og.Key, line, segs = segs.ToList() });
            });

        foreach (var g in groups)
        {
            var segs = g.segs;
            // sort by start
            segs.Sort((a, b) => a.start.CompareTo(b.start));

            float curStart = segs[0].start;
            float curEnd = segs[0].end;

            for (int i = 1; i < segs.Count; i++)
            {
                var s = segs[i];
                if (s.start <= curEnd + mergeTolerance)
                {
                    // overlaps or touches -> extend
                    curEnd = Mathf.Max(curEnd, s.end);
                }
                else
                {
                    // flush previous
                    if (curEnd - curStart >= minSegmentLength)
                    {
                        result.Add(new WallSegment
                        {
                            orientation = g.orientation,
                            lineCoord = g.line,
                            start = curStart,
                            end = curEnd
                        });
                    }
                    curStart = s.start;
                    curEnd = s.end;
                }
            }

            // flush last
            if (curEnd - curStart >= minSegmentLength)
            {
                result.Add(new WallSegment
                {
                    orientation = g.orientation,
                    lineCoord = g.line,
                    start = curStart,
                    end = curEnd
                });
            }
        }

        return result;
    }
}
