using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class BatchRenameAudioClips : EditorWindow
{
    [MenuItem("Tools/Batch Rename/Increment Digits in Selected AudioClips")]
    public static void IncrementDigitsInSelectedAudioClips()
    {
        var selected = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets);

        foreach (var obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            string directory = System.IO.Path.GetDirectoryName(path);
            string extension = System.IO.Path.GetExtension(path);

            // Replace all digits with incremented value
            string newName = Regex.Replace(fileName, @"\d+", match =>
            {
                int number = int.Parse(match.Value);
                return (number + 1).ToString();
            });

            string newPath = $"{directory}/{newName}{extension}";

            AssetDatabase.RenameAsset(path, newName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Batch rename complete.");
    }
}
