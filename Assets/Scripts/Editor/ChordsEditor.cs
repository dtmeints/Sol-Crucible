using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(D_Chord))]
public class D_ChordEditor : Editor
{
    private const int columns = 12;

    private static readonly string[] pitchNames = new string[]
    {
        "G1", "G#1", "A1", "A#1", "B1",
        "C2", "C#2", "D2", "D#2", "E2", "F2", "F#2",
        "G2", "G#2", "A2", "A#2", "B2",
        "C3", "C#3", "D3", "D#3", "E3", "F3", "F#3",
        "G3", "G#3", "A3", "A#3", "B3",
        "C4", "C#4", "D4", "D#4", "E4", "F4", "F#4",
        "G4", "G#4", "A4", "A#4", "B4",
        "C5", "C#5", "D5", "D#5", "E5", "F5", "F#5",
        "G5", "G#5", "A5", "A#5", "B5"
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty nameProp = serializedObject.FindProperty("Name");
        SerializedProperty rootProp = serializedObject.FindProperty("RootNote");
        SerializedProperty membersProp = serializedObject.FindProperty("members");
        SerializedProperty clipsProp = serializedObject.FindProperty("chordClips");

        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(rootProp);

        GUILayout.Space(10);
        GUILayout.Label("Pitch Members", EditorStyles.boldLabel);

        // Convert current members to a hash set for easy lookup
        HashSet<string> current = new();
        for (int i = 0; i < membersProp.arraySize; i++)
            current.Add(membersProp.GetArrayElementAtIndex(i).stringValue);

        List<string> newMembers = new();
        int index = 0;
        while (index < pitchNames.Length)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < columns && index < pitchNames.Length; i++, index++)
            {
                string pitch = pitchNames[index];
                bool selected = current.Contains(pitch);
                bool nowSelected = GUILayout.Toggle(selected, pitch, GUILayout.Width(50));

                if (nowSelected)
                    newMembers.Add(pitch);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Only update if changed
        if (!AreArraysEqual(current, newMembers))
        {
            membersProp.ClearArray();
            for (int i = 0; i < newMembers.Count; i++)
            {
                membersProp.InsertArrayElementAtIndex(i);
                membersProp.GetArrayElementAtIndex(i).stringValue = newMembers[i];
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Chord Clips", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(clipsProp, true);

        // Apply and save
        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(target);
        }
    }

    private bool AreArraysEqual(HashSet<string> oldSet, List<string> newList)
    {
        if (oldSet.Count != newList.Count) return false;
        foreach (string item in newList)
            if (!oldSet.Contains(item))
                return false;
        return true;
    }
}
