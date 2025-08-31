using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioLevelState
{
    Menu,
    Game
}
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioEvent mBGM;
    private AudioPlayer mAudioPlayer;
    [SerializeField]
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup SFXMixer;

    private AudioSource mMenuMusicSource;
    private AudioSource mGameMusicSource;

    [SerializeField]
    private AudioLevelState mAudioLevelState = AudioLevelState.Menu;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // If no instance exists, set this as the instance.
            Instance = this;
            // Optional: Prevent this object from being destroyed when loading new scenes.
            DontDestroyOnLoad(this.gameObject);

            mAudioPlayer = GetComponent<AudioPlayer>();

        }
    }

    private void Start()
    {
    }

    public AudioSource PlayAudioEvent(AudioEvent audioEvent)
    {
        return mAudioPlayer.PlayAudioEvent(audioEvent);
    }

}
