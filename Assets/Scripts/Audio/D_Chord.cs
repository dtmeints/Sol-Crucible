using UnityEngine;

[CreateAssetMenu(fileName = "New Chord", menuName = "Data/Chord")]
public class D_Chord : ScriptableObject
{
    public string Name;
    public string RootNote;
    public string[] members;
    public AudioClip[] chordClips;
}