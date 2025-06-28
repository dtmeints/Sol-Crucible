using UnityEngine;

public class MusicSetter : MonoBehaviour
{
    [SerializeField] D_ChordProgression music;
    void Start()
    {
        AudioManager.Instance.SetMusic(music);  
    }
}
