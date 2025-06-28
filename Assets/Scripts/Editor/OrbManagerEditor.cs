using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrbManager))]
public class OrbManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OrbManager manager = (OrbManager)target;

        if (GUILayout.Button("Refresh"))
        {
            manager.Refresh();
        }
    }
}
