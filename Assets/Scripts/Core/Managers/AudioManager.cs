using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<AudioManager>();
            return instance;
        }
    }
    [Header("SETTINGS")]
    [SerializeField] float maxSpeedIntensity;
    [SerializeField] float minVolume = .5f, maxVolume = 1.0f;
    [SerializeField] float bassVolume = .5f;
    [SerializeField] D_ChordProgression chordProgression;

    [Header("REFERENCES")]
    [SerializeField] AudioSource[] bassAudioSources;
    [SerializeField] AudioClip[] collisionSounds;
    [SerializeField] AudioClip[] bassClips;
    [SerializeField] D_ChordProgression victoryChords;

    [Header("RESOURCES")]
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
    private string[] bassPitches = new string[] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
    public Dictionary<string, AudioClip[]> clipsByPitch;
    public Dictionary<string, AudioClip> bassClipsByPitch;

    [Header("IN-GAME")]
    public string currentBassPitch;
    public AudioSource CurrentBassAudioSource => bassAudioSources[currentBassAudioSourceIndex];
    public int currentBassAudioSourceIndex;
    public float progressionStartTime;
    List<Coroutine> bassFadeCoroutines = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        clipsByPitch = new();
        foreach (string pitch in pitchNames)
            clipsByPitch[pitch] = GetClipsOfPitch(pitch);

        bassClipsByPitch = new();
        foreach (string pitch in bassPitches)
            bassClipsByPitch[pitch] = GetBassClipOfPitch(pitch);
    }

    private void Start()
    {
        currentBassPitch = chordProgression.CurrentRoot(progressionStartTime);
        currentBassAudioSourceIndex = 0;
        CurrentBassAudioSource.clip = bassClipsByPitch[currentBassPitch];
        CurrentBassAudioSource.volume = bassVolume;
        CurrentBassAudioSource.Play();
        progressionStartTime = Time.time;
    }

    private void Update()
    {
        if (chordProgression.CurrentRoot(progressionStartTime) != currentBassPitch || CurrentBassAudioSource.volume <= .1f)
        {
            PlayNextBassPitch();
        }
        if (Random.value < Time.deltaTime * chordProgression.averageRandomStrikesPerSecond)
        {
            PlayCollisionSound(UnityEngine.Random.Range(0, 3), Vector3.zero);
        }
    }

    public void PlayCollisionSound(float collisionSpeed, Vector3 collisionPoint)
    {
        float intensity = Mathf.Clamp01(collisionSpeed / maxSpeedIntensity);
        float volume = Mathf.Lerp(minVolume, maxVolume, intensity);

        string[] currentChordMembers = chordProgression.CurrentChordMembers(progressionStartTime);
        int randomMemberIndex = Random.Range(0, currentChordMembers.Length);
        string chosenChordMember = currentChordMembers[randomMemberIndex];

        //if there are no sounds, try again.
        if (clipsByPitch[chosenChordMember] == null || clipsByPitch[chosenChordMember].Length == 0)
        {
            PlayCollisionSound(collisionSpeed, collisionPoint);
            return;
        }
        AudioClip clip = clipsByPitch[chosenChordMember][Random.Range(0, clipsByPitch[chosenChordMember].Length)];
        AudioSource.PlayClipAtPoint(clip, collisionPoint, volume);
    }

    public void PlayPianoChord()
    {
        AudioClip chord = chordProgression.CurrentChord(progressionStartTime);
        if (chord != null)
            AudioSource.PlayClipAtPoint(chord, Vector3.zero);
    }

    public void SetMusic(D_ChordProgression progression)
    {
        this.chordProgression = progression;
        progressionStartTime = Time.time;
        foreach (Coroutine co in bassFadeCoroutines)
            if (co != null)
                StopCoroutine(co);
        bassFadeCoroutines.Clear();

        PlayNextBassPitch();
    }

    private void PlayNextBassPitch()
    {
        currentBassPitch = chordProgression.CurrentRoot(progressionStartTime);
        bassFadeCoroutines.Add(StartCoroutine(Co_FadeOut(bassAudioSources[currentBassAudioSourceIndex], chordProgression.chordDuration / 8f)));
        currentBassAudioSourceIndex += 1;

        if (currentBassAudioSourceIndex >= bassAudioSources.Length)
            currentBassAudioSourceIndex = 0;

        CurrentBassAudioSource.clip = bassClipsByPitch[currentBassPitch];
        CurrentBassAudioSource.volume = bassVolume;
        CurrentBassAudioSource.Play();
        if (chordProgression.strikeChordAtStartOfChord)
            PlayPianoChord();
    }

    public void PlayVictory() => SetMusic(victoryChords);

    private AudioClip[] GetClipsOfPitch(string pitch)
    {
        List<AudioClip> matchingClips = new List<AudioClip>();
        foreach (var clip in collisionSounds)
        {
            if (clip.name[..pitch.Length] == pitch)
            {
                matchingClips.Add(clip);
            }
        }
        return matchingClips.ToArray();
    }

    private AudioClip GetBassClipOfPitch(string pitch)
    {
        foreach (var clip in bassClips)
            if (clip.name[..pitch.Length] == pitch && clip.name[pitch.Length] != '#')
                return clip;
         
        return null;
    }

    private IEnumerator Co_FadeOut(AudioSource source, float duration)
    {
        float remainingDuration = duration;
        float startingVolume = source.volume;
        while (remainingDuration > 0)
        {
            source.volume = Mathf.Lerp(0, startingVolume, remainingDuration / duration);
            remainingDuration -= Time.deltaTime;
            yield return null;
        }
        source.Stop();
    }
}
