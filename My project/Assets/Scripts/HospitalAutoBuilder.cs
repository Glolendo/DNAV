using System.Collections.Generic;
using UnityEngine;

public class HospitalAutoBuilder : MonoBehaviour
{
    public enum Side { None, North, South, East, West }

    [System.Serializable] public class RectDef { public int xMin, xMax, yMin, yMax; }
    [System.Serializable] public class DoorSpec { public Side side; public int rectIndex; [Range(0f, 1f)] public float t; }
    [System.Serializable] public class RoomEx { public string name; public List<RectDef> rects = new(); public List<DoorSpec> doors = new(); }

    [Header("Grid & Scale")]
    public int gridWidth = 15;
    public int gridHeight = 15;
    public float cellSize = 2.3f;

    [Header("Geometry")]
    public float wallThickness = 0.15f;
    public float wallHeight = 2.8f;
    public float doorWidth = 1.2f;
    public float doorHeight = 2.3f;
    public float floorThickness = 0.1f;

    [Header("Prefabs (optional)")]
    public GameObject wallPrefab;
    public GameObject doorFramePrefab;
    public Material floorMaterial;

#if UNITY_EDITOR
    [Header("Dev")]
    public bool autoBuildOnPlay = false;
#endif

    // ==========================================================
    // Hallway scheme:
    //   • East–West main corridor reserved at y = 7–8 (2 cells)
    //   • North–South main corridor reserved at x = 9–10 (2 cells)
    //   • 1-cell margins elsewhere unless you asked to touch border
    // ==========================================================

    [Header("Rooms (inclusive grid coords)")]
    public List<RoomEx> roomsEx = new()
    {
        // ----- Bottom-left cluster -----
        new RoomEx {
            name="Main Lobby (101)",
            rects=new(){ new RectDef{ xMin=1, xMax=3, yMin=1, yMax=4 } },
            doors=new(){
                new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f }, // front
                new DoorSpec{ side=Side.North, rectIndex=0, t=0.33f }, // back-left
                new DoorSpec{ side=Side.North, rectIndex=0, t=0.67f }  // back-right
            }
        },
        new RoomEx {
            name="Consult Room (102)",
            rects=new(){ new RectDef{ xMin=1, xMax=2, yMin=5, yMax=6 } },
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Office (103)",
            rects=new(){ new RectDef{ xMin=3, xMax=4, yMin=5, yMax=6 } },
            doors=new(){ new DoorSpec{ side=Side.West, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Radiology Check-in (104)",
            rects=new(){ new RectDef{ xMin=4, xMax=5, yMin=1, yMax=4 } },
            doors=new(){ new DoorSpec{ side=Side.East, rectIndex=0, t=0.60f } } // right-side door
        },
        new RoomEx {
            name="Room 105",
            rects=new(){ new RectDef{ xMin=6, xMax=6, yMin=2, yMax=3 } },
            doors=new(){ new DoorSpec{ side=Side.North, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="X-Ray Room (106)",
            rects=new(){ new RectDef{ xMin=7, xMax=8, yMin=3, yMax=4 } },
            doors=new(){ new DoorSpec{ side=Side.North, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 107",
            rects=new(){ new RectDef{ xMin=6, xMax=7, yMin=5, yMax=6 } },
            doors=new(){ new DoorSpec{ side=Side.North, rectIndex=0, t=0.50f } }
        },

        // ----- Mid-left + upper-left -----
        new RoomEx {
            name="Room 108",
            rects=new(){ new RectDef{ xMin=1, xMax=3, yMin=9, yMax=10 } }, // above EW corridor
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Pathology Lab (109–120)",
            rects=new(){ new RectDef{ xMin=1, xMax=4, yMin=13, yMax=15 } }, // touching top & left borders
            doors=new(){
                new DoorSpec{ side=Side.South, rectIndex=0, t=0.30f },
                new DoorSpec{ side=Side.East,  rectIndex=0, t=0.40f } // extra east-side door
            }
        },
        new RoomEx {
            name="Room 110",
            rects=new(){ new RectDef{ xMin=5, xMax=8, yMin=9, yMax=10 } },
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.60f } }
        },
        new RoomEx {
            name="Room 111",
            rects=new(){ new RectDef{ xMin=7, xMax=8, yMin=6, yMax=8 } }, // shifted left to avoid NS corridor
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },

        // ----- Lower-right around bottom edge -----
        new RoomEx {
            name="Room 112",
            rects=new(){ new RectDef{ xMin=11, xMax=12, yMin=3, yMax=4 } }, // moved off NS corridor
            doors=new(){ new DoorSpec{ side=Side.North, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Financial Office (113)",
            rects=new(){ new RectDef{ xMin=12, xMax=14, yMin=1, yMax=2 } }, // touches bottom, inset from right
            doors=new(){ new DoorSpec{ side=Side.East, rectIndex=0, t=0.55f } } // door on right
        },
        new RoomEx {
            name="Room 114",
            rects=new(){ new RectDef{ xMin=11, xMax=12, yMin=5, yMax=6 } }, // moved off NS corridor
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 115",
            rects=new(){ new RectDef{ xMin=12, xMax=13, yMin=5, yMax=6 } }, // below EW corridor (no overlap)
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 116",
            rects=new(){ new RectDef{ xMin=13, xMax=14, yMin=9, yMax=10 } }, // right of NS corridor
            doors=new(){ new DoorSpec{ side=Side.West, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 117",
            rects=new(){ new RectDef{ xMin=12, xMax=13, yMin=9, yMax=10 } }, // left of 116
            doors=new(){ new DoorSpec{ side=Side.East, rectIndex=0, t=0.50f } }
        },

        // ----- Upper mid / top rows -----
        new RoomEx {
            name="Room 118",
            rects=new(){ new RectDef{ xMin=6, xMax=8, yMin=11, yMax=13 } }, // shifted left of NS corridor
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 119",
            rects=new(){ new RectDef{ xMin=5, xMax=7, yMin=13, yMax=14 } },
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 120",
            rects=new(){ new RectDef{ xMin=7, xMax=8, yMin=14, yMax=15 } }, // moved left to avoid x=9
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },
        new RoomEx {
            name="Room 121",
            rects=new(){ new RectDef{ xMin=12, xMax=13, yMin=13, yMax=14 } },
            doors=new(){ new DoorSpec{ side=Side.South, rectIndex=0, t=0.50f } }
        },

        // ----- Pharmacy top-right (with right-side gap) -----
        new RoomEx {
            name="Pharmacy (122)",
            rects=new(){ new RectDef{ xMin=13, xMax=14, yMin=13, yMax=13 } }, // leaves x=15 as walking space
            doors=new(){ new DoorSpec{ side=Side.North, rectIndex=0, t=0.50f } }
        }
    };

    // ---------- Build pipeline ----------
    void Reset() { gameObject.name = "Hospital_Auto_Builder"; }

    void Start()
    {
#if UNITY_EDITOR
        if (autoBuildOnPlay) Build();
#endif
    }

    [ContextMenu("Build Layout")]
    public void Build()
    {
        transform.position = Vector3.zero;

        // clear old build
        var kill = new List<GameObject>();
        foreach (Transform c in transform) kill.Add(c.gameObject);
        foreach (var g in kill) DestroyImmediate(g);

        BuildFloor();
        BuildRoomsEx();
        BuildOuterBoundaryWithAutoDoors();
    }

    void BuildFloor()
    {
        float w = gridWidth * cellSize;
        float h = gridHeight * cellSize;

        var f = GameObject.CreatePrimitive(PrimitiveType.Cube);
        f.name = "Floor";
        f.transform.SetParent(transform);
        f.transform.localScale = new Vector3(w, floorThickness, h);

        // fixed alignment that solved your earlier offset
        f.transform.localPosition = new Vector3(
            (gridWidth * cellSize) * 0.5f + (cellSize * 0.5f),
            -floorThickness * 0.5f,
            (gridHeight * cellSize) * 0.5f + (cellSize * 0.5f)
        );

        if (floorMaterial) f.GetComponent<MeshRenderer>().material = floorMaterial;
    }

    void BuildRoomsEx()
    {
        foreach (var r in roomsEx)
        {
            if (r.rects == null || r.rects.Count == 0) continue;
            var root = new GameObject(r.name);
            root.transform.SetParent(transform, false);

            for (int i = 0; i < r.rects.Count; i++)
            {
                var rc = r.rects[i];
                Vector3 min = CellToWorld(rc.xMin, rc.yMin);
                Vector3 max = CellToWorld(rc.xMax + 1, rc.yMax + 1);

                List<float> s = new(), n = new(), w = new(), e = new();
                if (r.doors != null)
                {
                    foreach (var d in r.doors)
                    {
                        if (d.rectIndex != i) continue;
                        switch (d.side)
                        {
                            case Side.South: s.Add(Mathf.Clamp01(d.t)); break;
                            case Side.North: n.Add(Mathf.Clamp01(d.t)); break;
                            case Side.West:  w.Add(Mathf.Clamp01(d.t)); break;
                            case Side.East:  e.Add(Mathf.Clamp01(d.t)); break;
                        }
                    }
                }

                BuildWallRun(root.transform, Side.South, min, max, s);
                BuildWallRun(root.transform, Side.North, min, max, n);
                BuildWallRun(root.transform, Side.West,  min, max, w);
                BuildWallRun(root.transform, Side.East,  min, max, e);
            }
        }
    }

    void BuildOuterBoundaryWithAutoDoors()
    {
        Vector3 minB = CellToWorld(1, 1);
        Vector3 maxB = CellToWorld(gridWidth + 1, gridHeight + 1);
        var south = new List<float>(); var north = new List<float>(); var west = new List<float>(); var east = new List<float>();

        foreach (var r in roomsEx)
        {
            if (r.rects == null || r.doors == null) continue;
            for (int i = 0; i < r.rects.Count; i++)
            {
                var rc = r.rects[i];
                Vector3 min = CellToWorld(rc.xMin, rc.yMin);
                Vector3 max = CellToWorld(rc.xMax + 1, rc.yMax + 1);

                foreach (var d in r.doors)
                {
                    if (d.rectIndex != i) continue;
                    DoorPose(min, max, d.side, d.t, out var p, out _);

                    if (d.side == Side.South && rc.yMin == 1) south.Add(Mathf.InverseLerp(minB.x, maxB.x, p.x));
                    if (d.side == Side.North && rc.yMax == gridHeight) north.Add(Mathf.InverseLerp(minB.x, maxB.x, p.x));
                    if (d.side == Side.West  && rc.xMin == 1) west.Add(Mathf.InverseLerp(minB.z, maxB.z, p.z));
                    if (d.side == Side.East  && rc.xMax == gridWidth) east.Add(Mathf.InverseLerp(minB.z, maxB.z, p.z));
                }
            }
        }

        var root = new GameObject("OuterBoundary");
        root.transform.SetParent(transform, false);
        BuildWallRun(root.transform, Side.South, minB, maxB, south);
        BuildWallRun(root.transform, Side.North, minB, maxB, north);
        BuildWallRun(root.transform, Side.West,  minB, maxB, west);
        BuildWallRun(root.transform, Side.East,  minB, maxB, east);
    }

    // ----- wall helpers -----
    void BuildWallRun(Transform parent, Side side, Vector3 min, Vector3 max, List<float> doors01)
    {
        float length = (side == Side.North || side == Side.South) ? (max.x - min.x) : (max.z - min.z);

        if (doors01 == null || doors01.Count == 0)
        {
            CreateWall(parent, side, min, max, 0f, length);
            return;
        }

        doors01.Sort();
        float halfGap = Mathf.Min(doorWidth, length - wallThickness) * 0.5f;
        float cursor = -length * 0.5f;

        foreach (var t in doors01)
        {
            float center = Mathf.Lerp(-length * 0.5f, length * 0.5f, t);
            float a = Mathf.Max(center - halfGap, -length * 0.5f);
            float b = Mathf.Min(center + halfGap,  length * 0.5f);

            float segLen = (a - cursor);
            if (segLen > wallThickness * 0.5f)
            {
                float segMid = cursor + segLen * 0.5f;
                CreateWall(parent, side, min, max, segMid, segLen);
            }
            cursor = b;
        }

        float tail = (length * 0.5f - cursor);
        if (tail > wallThickness * 0.5f)
        {
            float segMid = cursor + tail * 0.5f;
            CreateWall(parent, side, min, max, segMid, tail);
        }
    }

    void CreateWall(Transform parent, Side side, Vector3 min, Vector3 max, float offsetAlong, float length)
    {
        GameObject wall = wallPrefab ? Instantiate(wallPrefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.SetParent(parent, false);

        Vector3 pos, scale;
        if (side == Side.South || side == Side.North)
        {
            float z = (side == Side.South) ? min.z : max.z;
            pos = new Vector3((min.x + max.x) * 0.5f + offsetAlong, wallHeight * 0.5f, z);
            scale = new Vector3(length, wallHeight, wallThickness);
        }
        else
        {
            float x = (side == Side.West) ? min.x : max.x;
            pos = new Vector3(x, wallHeight * 0.5f, (min.z + max.z) * 0.5f + offsetAlong);
            scale = new Vector3(wallThickness, wallHeight, length);
        }

        wall.transform.localPosition = pos;
        wall.transform.localScale = scale;
    }

    // ----- math helpers -----
    Vector3 CellToWorld(int x, int y)
    {
        float X = (x - 0.5f) * cellSize;
        float Z = (y - 0.5f) * cellSize;
        return new Vector3(X, 0f, Z);
    }

    void DoorPose(Vector3 min, Vector3 max, Side s, float t, out Vector3 p, out Quaternion r)
    {
        p = Vector3.zero; r = Quaternion.identity;

        if (s == Side.South) { float x = Mathf.Lerp(min.x, max.x, t); p = new Vector3(x, 0, min.z); r = Quaternion.Euler(0, 0, 0); }
        else if (s == Side.North) { float x = Mathf.Lerp(min.x, max.x, t); p = new Vector3(x, 0, max.z); r = Quaternion.Euler(0, 180, 0); }
        else if (s == Side.West)  { float z = Mathf.Lerp(min.z, max.z, t); p = new Vector3(min.x, 0, z); r = Quaternion.Euler(0, 90, 0); }
        else if (s == Side.East)  { float z = Mathf.Lerp(min.z, max.z, t); p = new Vector3(max.x, 0, z); r = Quaternion.Euler(0, -90, 0); }
    }
}
