// Assets/Editor/LayoutExporter.cs
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class LayoutExporter : EditorWindow
{
    [Serializable] public class XForm
    {
        public string name;
        public string path;          // Hierarchy path from root parent
        public float px, py, pz;     // position (world)
        public float ry;             // rotation Y (world) – we assume top-down layout
        public float sx, sy, sz;     // local scale
        public string prefabHint;    // optional: original prefab name (if any)
        public string tag;
        public int layer;
    }

    [Serializable] public class Group
    {
        public string groupName;     // "Walls", "Doors", etc.
        public List<XForm> items = new List<XForm>();
    }

    [Serializable] public class LayoutDump
    {
        public string sceneName;
        public string exportedAt;    // ISO time
        public float gridSize = 0.5f;
        public float wallThicknessDefault = 0.2f;
        public List<Group> groups = new List<Group>();
        public string notes = "Positions are world-space; rotationY only is captured for 2D layout.";
    }

    Transform wallsRoot, doorsRoot, extraRoot;
    float gridSize = 0.5f;
    float wallThicknessDefault = 0.2f;

    [MenuItem("Tools/Layout Exporter")]
    static void Open() => GetWindow<LayoutExporter>("Layout Exporter");

    void OnGUI()
    {
        GUILayout.Label("Roots", EditorStyles.boldLabel);
        wallsRoot = (Transform)EditorGUILayout.ObjectField("Walls Root", wallsRoot, typeof(Transform), true);
        doorsRoot = (Transform)EditorGUILayout.ObjectField("Doors Root", doorsRoot, typeof(Transform), true);
        extraRoot = (Transform)EditorGUILayout.ObjectField("Extra Root (optional)", extraRoot, typeof(Transform), true);

        GUILayout.Space(6);
        gridSize = EditorGUILayout.FloatField("Grid Size (snap)", gridSize);
        wallThicknessDefault = EditorGUILayout.FloatField("Default Wall Thickness", wallThicknessDefault);

        GUILayout.Space(10);
        if (GUILayout.Button("Export Layout JSON"))
            Export();
    }

    void Export()
    {
        var dump = new LayoutDump
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            exportedAt = DateTime.UtcNow.ToString("o"),
            gridSize = gridSize,
            wallThicknessDefault = wallThicknessDefault,
            groups = new List<Group>()
        };

        void CaptureGroup(string name, Transform root)
        {
            if (root == null) return;
            var g = new Group { groupName = name };
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t == root) continue;
                var wPos = t.position;
                var wRot = t.rotation.eulerAngles;
                var s = t.localScale;

                var xf = new XForm
                {
                    name = t.name,
                    path = GetHierarchyPath(t, root),
                    px = wPos.x, py = wPos.y, pz = wPos.z,
                    ry = wRot.y,
                    sx = s.x, sy = s.y, sz = s.z,
                    tag = t.tag,
                    layer = t.gameObject.layer,
                    prefabHint = GetPrefabSourceName(t)
                };
                g.items.Add(xf);
            }
            dump.groups.Add(g);
        }

        CaptureGroup("Walls", wallsRoot);
        CaptureGroup("Doors", doorsRoot);
        if (extraRoot != null) CaptureGroup(extraRoot.name, extraRoot);

        // Save
        var folder = "Assets/Exports";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var path = EditorUtility.SaveFilePanel("Save Layout JSON", folder, "layout_dump.json", "json");
        if (string.IsNullOrEmpty(path)) return;

        var json = JsonUtility.ToJson(dump, prettyPrint: true);
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
        Debug.Log($"Layout exported:\n{path}");
    }

    static string GetHierarchyPath(Transform t, Transform stopAt)
    {
        var stack = new Stack<string>();
        var cur = t;
        while (cur != null && cur != stopAt)
        {
            stack.Push(cur.name);
            cur = cur.parent;
        }
        return string.Join("/", stack);
    }

  static string GetPrefabSourceName(Transform t)
{
#if UNITY_2021_3_OR_NEWER
    var src = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject);
    return src ? src.name : "";
#else
    var src = PrefabUtility.GetPrefabParent(t.gameObject) as GameObject;
    return src ? src.name : "";
#endif
}

}
