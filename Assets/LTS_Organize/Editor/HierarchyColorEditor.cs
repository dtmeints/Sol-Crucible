// Assets/Editor/HierarchyColorEditor.cs
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[InitializeOnLoad]
internal static class HierarchyColorEditor
{
    #region Constants
    private const float SolidFrac = .5f, VPad = 1f, GutterW = 32f, Pad = 7f, Cell = 24f;
    private const int StripDepth = 100, ArrowDepth = -1500, IconsPerRow = 10;
    #endregion

    #region Caches
    private static readonly Dictionary<string, Texture2D> IconCache = new();
    private static readonly Dictionary<(Color, int), Texture2D> GradCache = new(new GradKeyComparer());
    #endregion

    #region Reflection
    private static readonly Type SceneHierarchyType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchy");
    private static readonly PropertyInfo LastHierarchyProp = SceneHierarchyType.GetProperty("lastInteractedHierarchy",
        BindingFlags.Public | BindingFlags.Static);
    private static readonly MethodInfo SetExpanded = SceneHierarchyType.GetMethod("SetExpanded",
        BindingFlags.Public | BindingFlags.Instance);
    private static readonly MethodInfo IsExpanded = SceneHierarchyType.GetMethod("IsExpanded",
        BindingFlags.Public | BindingFlags.Instance);
    private delegate void SetIconDel(UnityEngine.Object o, Texture2D t);
    private static readonly SetIconDel SetIcon =
        (SetIconDel)Delegate.CreateDelegate(typeof(SetIconDel),
            typeof(EditorGUIUtility).GetMethod("SetIconForObject",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!);
    #endregion

    #region GUI
    private static readonly GUIContent GC = new();
    private static GUIStyle arrowStyle;
    private static readonly Func<Rect> VisibleRect = BuildVisibleGetter();
    private static readonly Color HoverPro = new(1, 1, 1, .02f);
    private static readonly Color HoverPersonal = new(0, 0, 0, .05f);
    #endregion

    #region Init
    static HierarchyColorEditor()
    {
        MergeSavedPalettes();
        EditorApplication.hierarchyWindowItemOnGUI += RowGUI;
    }
    #endregion

    #region Palette Persistence
    private const string PalKey = "PCustomPalettes";
    [Serializable] private sealed class Wrapper<T> { public List<T> items = new(); }
    private static void MergeSavedPalettes()
    {
        var json = EditorPrefs.GetString(PalKey, string.Empty);
        if (string.IsNullOrEmpty(json)) return;
        var extras = JsonUtility.FromJson<Wrapper<Color[]>>(json).items;
        foreach (var p in extras)
            if (!((List<Color[]>)PaletteData.HierarchyPalettes).Contains(p))
                ((List<Color[]>)PaletteData.HierarchyPalettes).Add(p);
    }
    #endregion

    #region Row GUI
    private static void RowGUI(int id, Rect row)
    {
        var go = EditorUtility.InstanceIDToObject(id) as GameObject;
        if (go == null) return;
        if (Event.current.type == EventType.Repaint)
        {
            var clip = VisibleRect();
            if (row.yMax < clip.yMin || row.yMin > clip.yMax) return;
        }

        var gid = GlobalObjectId.GetGlobalObjectIdSlow(go);
        var selected = Array.IndexOf(Selection.instanceIDs, id) >= 0;
        
        HierarchyColorStore.instance.TryGetColor(gid, out var tint);
        var rowTint = tint == default ? (selected ? PaletteData.DefaultHierarchySelectedBlue : PaletteData.DefaultHierarchyGrey) : tint;
        var pad = selected ? 0f : VPad;
        DrawStrip(row, rowTint, pad);

        var e = Event.current;
        var hovered = e.type == EventType.Repaint && row.Contains(e.mousePosition);
        if (hovered || selected)
        {
            var clip = VisibleRect();
            var col = EditorGUIUtility.isProSkin ? HoverPro : HoverPersonal;
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(new Rect(clip.xMin, row.y, clip.width, row.height), col, Color.clear);
            Handles.EndGUI();
        }
        
        if (go.transform.childCount > 0)
        {
            bool hasVisibleChildren = false;
            foreach (Transform child in go.transform) {
                if (child.gameObject.hideFlags is not HideFlags.HideInHierarchy and not HideFlags.HideAndDontSave) {
                    hasVisibleChildren = true;
                    break;
                }
            }

            if (hasVisibleChildren) {
                arrowStyle ??= new GUIStyle(EditorStyles.foldout) { fixedWidth = 16, fixedHeight = 16 };
                var fr = new Rect(row.x - 14, row.y + (row.height - 16) * .5f, 16, 16);
                var d = GUI.depth;
                GUI.depth = ArrowDepth;
                var exp = GUI.Toggle(fr, RowExpanded(id), GUIContent.none, arrowStyle);
                GUI.depth = d;
                if (exp != RowExpanded(id)) SetRowExpanded(id, exp);
            }
        }

        var iconTex = EditorGUIUtility.ObjectContent(go, typeof(GameObject)).image as Texture2D;
        if (iconTex != null) GUI.DrawTexture(new Rect(row.x, row.y + pad, 16, 16), iconTex, ScaleMode.ScaleToFit, true);

        GC.text = go.name; GC.image = null;
        var save = GUI.color; GUI.color = Contrast(rowTint);
        GUI.Label(new Rect(row.x + 18, row.y + pad, row.width - 18, row.height - pad), GC, EditorStyles.label);
        GUI.color = save;

        if (e.type == EventType.MouseDown && e.alt && e.button == 0 && row.Contains(e.mousePosition))
        {
            PalettePopup.Show(go, gid, GUIUtility.GUIToScreenPoint(e.mousePosition));
            e.Use();
        }
    }
    #endregion
    
    #region Strip & Gradient
    private static void DrawStrip(Rect row, Color col, float pad)
    {
        var clip = VisibleRect();
        var x0 = clip.xMin + GutterW;
        var w = Mathf.RoundToInt(clip.xMax - x0);
        var solid = Mathf.RoundToInt(w * SolidFrac);
        var fade = w - solid;
        var h = row.height - pad;

        var d = GUI.depth; GUI.depth = StripDepth;
        Handles.BeginGUI();
        Handles.DrawSolidRectangleWithOutline(new Rect(x0, row.y + pad, solid, h), col, Color.clear);
        Handles.EndGUI(); GUI.depth = d;

        if (fade > 0)
            GUI.DrawTexture(new Rect(x0 + solid, row.y + pad, fade, h), Gradient(col, fade));
    }

    private static Texture2D Gradient(Color c, int w)
    {
        if (GradCache.TryGetValue((c, w), out var t)) return t;
        var tex = new Texture2D(w, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.DontSave, filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
        var bg = PaletteData.DefaultHierarchyGrey;
        for (var i = 0; i < w; i++) tex.SetPixel(i, 0, Color.Lerp(c, bg, i / (float)(w - 1)));
        tex.Apply();
        return GradCache[(c, w)] = tex;
    }
    #endregion

    #region Hierarchy Helpers
    private static object GetHierarchy() => LastHierarchyProp?.GetValue(null);
    private static bool RowExpanded(int id) => GetHierarchy() is { } h && IsExpanded != null && (bool)IsExpanded.Invoke(h, new object[] { id, true });
    private static void SetRowExpanded(int id, bool exp) { var h = GetHierarchy(); if (h != null && SetExpanded != null) SetExpanded.Invoke(h, new object[] { id, exp, true }); }
    #endregion

    #region VisibleRect Reflection
    private static Func<Rect> BuildVisibleGetter()
    {
        var t = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
        var p = t.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), p!.GetGetMethod(true)!);
    }
    #endregion

    #region Colour Helpers
    private static Color Contrast(Color c) =>
        (.299f * c.r + .587f * c.g + .114f * c.b) > .6f ? Color.black : Color.white;
    private static Texture2D Icon(string name)
    {
        if (IconCache.TryGetValue(name, out var t)) return t;
        t = EditorGUIUtility.FindTexture(EditorGUIUtility.isProSkin ? "d_" + name : name)
            ?? EditorGUIUtility.FindTexture(EditorGUIUtility.isProSkin ? name : "d_" + name);
        return IconCache[name] = t;
    }
    #endregion

    #region GradientKeyComparer
    private readonly struct GradKeyComparer : IEqualityComparer<(Color, int)>
    {
        public bool Equals((Color, int) a, (Color, int) b) =>
            Mathf.Approximately(a.Item1.r, b.Item1.r) &&
            Mathf.Approximately(a.Item1.g, b.Item1.g) &&
            Mathf.Approximately(a.Item1.b, b.Item1.b) &&
            Mathf.Approximately(a.Item1.a, b.Item1.a) && a.Item2 == b.Item2;
        public int GetHashCode((Color, int) k) => k.Item1.GetHashCode() ^ k.Item2;
    }
    #endregion

    #region PalettePopup
    private sealed class PalettePopup : PopupWindowContent
    {
        private readonly GameObject go;
        private readonly GlobalObjectId gid;
        private bool drag;
        private Vector2 ds, ws;
        private readonly List<Rect> hot = new();
        private static GUIStyle XStyle;
        public PalettePopup(GameObject g, GlobalObjectId id) { go = g; gid = id; }
        public static void Show(GameObject g, GlobalObjectId id, Vector2 scr) =>
            PopupWindow.Show(new Rect(scr, Vector2.zero), new PalettePopup(g, id));
        public override Vector2 GetWindowSize()
        {
            var rows = 1 + (PaletteData.HierarchyPalettes.Count + 1) / 2 +
                       (PaletteData.IconNames.Count + IconsPerRow - 1) / IconsPerRow;
            return new Vector2(Pad * 2 + Cell * IconsPerRow, Pad * (rows + 1) + Cell * rows);
        }
        public override void OnGUI(Rect _)
        {
            XStyle ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
            hot.Clear(); var y = Pad; var e = Event.current;

            var clr = new Rect(Pad, y, Cell - 2, Cell - 2);
            GUI.Box(clr, GUIContent.none); GUI.Label(clr, "X", XStyle);
            if (GUI.Button(clr, GUIContent.none, GUIStyle.none))
            { HierarchyColorStore.instance.SetColor(gid, PaletteData.DefaultHierarchyGrey); SetIcon(go, null); editorWindow.Close(); return; }
            hot.Add(clr); y += Cell + Pad;

            for (var p = 0; p < PaletteData.HierarchyPalettes.Count; p += 2)
            {
                var a = PaletteData.HierarchyPalettes[p];
                var b = (p + 1 < PaletteData.HierarchyPalettes.Count) ? PaletteData.HierarchyPalettes[p + 1] : null;
                for (var i = 0; i < a.Length; i++)
                {
                    var rA = new Rect(Pad + i * Cell, y, Cell - 2, Cell - 2);
                    EditorGUI.DrawRect(rA, a[i]);
                    if (GUI.Button(rA, GUIContent.none, GUIStyle.none))
                    { HierarchyColorStore.instance.SetColor(gid, a[i]); editorWindow.Close(); return; }
                    hot.Add(rA);
                    if (b != null)
                    {
                        var rB = new Rect(Pad + (i + 5) * Cell, y, Cell - 2, Cell - 2);
                        EditorGUI.DrawRect(rB, b[i]);
                        if (GUI.Button(rB, GUIContent.none, GUIStyle.none))
                        { HierarchyColorStore.instance.SetColor(gid, b[i]); editorWindow.Close(); return; }
                        hot.Add(rB);
                    }
                }
                y += Cell + Pad;
            }

            for (var i = 0; i < PaletteData.IconNames.Count; i++)
            {
                var row = i / IconsPerRow; var col = i % IconsPerRow;
                var tex = LoadIcon(PaletteData.IconNames[i]); if (tex == null) continue;
                var r = new Rect(Pad + col * Cell, y + row * (Cell + Pad), Cell - 2, Cell - 2);
                if (GUI.Button(r, tex, GUIStyle.none)) { SetIcon(go, tex); editorWindow.Close(); return; }
                hot.Add(r);
            }

            switch (e.type)
            {
                case EventType.MouseDown when e.button == 0 && !hot.Exists(r => r.Contains(e.mousePosition)):
                    drag = true; ds = GUIUtility.GUIToScreenPoint(e.mousePosition); ws = editorWindow.position.position; break;
                case EventType.MouseDrag when drag:
                    var now = GUIUtility.GUIToScreenPoint(e.mousePosition);
                    editorWindow.position = new Rect(ws + (now - ds), editorWindow.position.size); break;
                case EventType.MouseUp: drag = false; break;
            }
        }
    }

    private static Texture2D LoadIcon(string name)
    {
        if (IconCache.TryGetValue(name, out var tex)) return tex;

        // 1️⃣  exact texture asset (dark / personal skins)
        string skinPrefix = EditorGUIUtility.isProSkin ? "d_" : "";
        tex = EditorGUIUtility.FindTexture(skinPrefix + name) ??
               EditorGUIUtility.FindTexture(name);

        // 2️⃣  built-in virtual icon
        tex ??= EditorGUIUtility.IconContent(name).image as Texture2D;

        // 3️⃣  try without the trailing " Icon" (Unity sometimes omits it)
        if (tex == null && name.EndsWith(" Icon"))
            tex = EditorGUIUtility.IconContent(name[..^5]).image as Texture2D;

        return IconCache[name] = tex;        // may still be null (skip quietly)
    }
    #endregion
}
