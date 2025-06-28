using UnityEngine;

[CreateAssetMenu(fileName = "newChordProgression", menuName = "Data/ChordProgression")]
public class D_ChordProgression : ScriptableObject
{
    public float chordDuration = 4;
    public D_Chord[] chords;
    public float averageRandomStrikesPerSecond;
    public bool hangOnFinalChord;
    public bool strikeChordAtStartOfChord;

    public string CurrentRoot(float progressionStartTime)
    {
        if (hangOnFinalChord && (Time.time - progressionStartTime) > chordDuration * chords.Length)
            return chords[chords.Length - 1].RootNote;

        return chords[Mathf.FloorToInt(((Time.time - progressionStartTime) / chordDuration) % chords.Length)].RootNote;
    }
    public string[] CurrentChordMembers(float progressionStartTime)
    {
        if (hangOnFinalChord && (Time.time - progressionStartTime) > chordDuration * chords.Length)
            return chords[chords.Length - 1].members;

        return chords[Mathf.FloorToInt(((Time.time - progressionStartTime) / chordDuration) % chords.Length)].members;
    }

    public AudioClip CurrentChord(float progressionStartTime)
    {
        if (hangOnFinalChord && (Time.time - progressionStartTime) > chordDuration * chords.Length)
            return chords[chords.Length - 1].chordClips[0];

        return chords[Mathf.FloorToInt(((Time.time - progressionStartTime) / chordDuration) % chords.Length)].chordClips[0];
    }
}
