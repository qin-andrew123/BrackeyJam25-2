using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("Audio that plays on start")]
    private AudioEvent[] mStartupSounds;
    private List<AudioSource> mSources = new();

    private void Awake()
    {
        AudioSource audioSource = transform.AddComponent<AudioSource>();
        mSources.Add(audioSource);

        foreach (AudioEvent audioEvent in mStartupSounds)
        {
            PlayAudioEvent(audioEvent);
        }
    }

    private void Start()
    {
        mSources[0].outputAudioMixerGroup = AudioManager.Instance.SFXMixer;
    }

    public AudioSource PlayAudioEvent(AudioEvent audioEvent)
    {
        if (audioEvent == null || audioEvent.mSound == null || audioEvent.mSound.Length == 0) { return null; }
        int soundIndex = Random.Range(0, audioEvent.mSound.Length);

        if (audioEvent.bIsSpatial)
        {
            AudioSource audioSource = transform.AddComponent<AudioSource>();
            mSources.Add(audioSource);
            audioSource.resource = audioEvent.mSound[soundIndex];
            //audioSource.outputAudioMixerGroup = AudioManager.Instance.SFXMixer;
            audioSource.spatialBlend = audioEvent.mSpatialBlend;
            audioSource.volume = audioEvent.mVolumeScale;
            audioSource.loop = audioEvent.bIsLoop;
            audioSource.spatialize = true;
            audioSource.maxDistance = audioEvent.mMaxDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.Play();

            return audioSource;
        }
        else if (audioEvent.bIsLoop)
        {
            return FindAndPlaySource(audioEvent, soundIndex);
        }
        else
        {
            mSources[0].PlayOneShot(audioEvent.mSound[soundIndex], audioEvent.mVolumeScale);
            return null;
        }
    }

    // Searches for an available audio source to play from
    // If there are none, creates a new one
    private AudioSource FindAndPlaySource(AudioEvent audioEvent, int soundIndex)
    {
        int availSource = -1;
        for (int i = 0; i < mSources.Count; ++i)
        {
            if (!mSources[i].isPlaying || mSources[i].clip == null)
            {
                availSource = i;
                break;
            }
        }

        if (availSource == -1)
        {
            AudioSource audioSource = transform.AddComponent<AudioSource>();
            mSources.Add(audioSource);
            audioSource.resource = audioEvent.mSound[soundIndex];
            audioSource.volume = audioEvent.mVolumeScale;
            audioSource.loop = audioEvent.bIsLoop;
            audioSource.outputAudioMixerGroup = AudioManager.Instance.SFXMixer;
            audioSource.Play();
            return audioSource;
        }
        else
        {
            mSources[availSource].resource = audioEvent.mSound[soundIndex];
            mSources[availSource].volume = audioEvent.mVolumeScale;
            mSources[availSource].loop = audioEvent.bIsLoop;
            mSources[availSource].Play();
            return mSources[availSource];
        }
    }
}
