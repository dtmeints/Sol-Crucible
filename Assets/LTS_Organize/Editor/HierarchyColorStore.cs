// Assets/Editor/HierarchyColorStore.cs
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
[FilePath("Library/HierarchyColorStore.asset", FilePathAttribute.Location.ProjectFolder)]
internal class HierarchyColorStore : ScriptableSingleton<HierarchyColorStore>
{
    [Serializable] struct Entry { public string id; public Color c; public string icon; }
    [SerializeField] List<Entry> entries = new();

    public bool TryGetColor(GlobalObjectId gid, out Color color)
    {
        var key = gid.ToString();
        foreach (var e in entries)
            if (e.id == key && e.c.a > 0f) { color = e.c; return true; }
        color = default; return false;
    }

    public void SetColor(GlobalObjectId gid, Color col)
    {
        var key = gid.ToString();
        for (var i = 0; i < entries.Count; ++i)
            if (entries[i].id == key) { entries[i] = new Entry { id = key, c = col, icon = entries[i].icon }; Save(true); return; }
        entries.Add(new Entry { id = key, c = col }); Save(true);
    }

    public void RemoveColor(GlobalObjectId gid)
    {
        var key = gid.ToString(); entries.RemoveAll(e => e.id == key); Save(true);
    }

    public bool TryGetIcon(GlobalObjectId gid, out string iconName)
    {
        var key = gid.ToString();
        foreach (var e in entries)
            if (e.id == key) { iconName = e.icon; return !string.IsNullOrEmpty(iconName); }
        iconName = null; return false;
    }

    public void SetIcon(GlobalObjectId gid, string iconName)
    {
        var key = gid.ToString();
        for (var i = 0; i < entries.Count; ++i)
            if (entries[i].id == key) { entries[i] = new Entry { id = key, c = entries[i].c, icon = iconName }; Save(true); return; }
        entries.Add(new Entry { id = key, c = default, icon = iconName }); Save(true);
    }
}
