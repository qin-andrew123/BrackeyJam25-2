using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "Scriptable Objects/AudioEvent")]
public class AudioEvent : ScriptableObject
{
    [Tooltip("Sounds to play (picked randomly)")]
    public AudioClip[] mSound;
    [Tooltip("Should the sound play spatialized on the GameObject")]
    public bool bIsSpatial;
    [Tooltip("Is the sound a loop or one-shot")]
    public bool bIsLoop;
    [Tooltip("Volume, on a scale from 0-1")]
    public float mVolumeScale = 1f;

    [Header("Spatialized Sound Settings")]
    public float mSpatialBlend;
    public float mMaxDistance;

    public void Play2DSound()
    {
        AudioManager.Instance.PlayAudioEvent(this);
    }
}
