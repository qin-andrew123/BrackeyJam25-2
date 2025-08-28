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
    public static AudioManager Instance;
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

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.SetAudioLevelState(mAudioLevelState);
            if (mBGM == null)
            {
                Instance.mMenuMusicSource.Stop();
            }
            else if (!Instance.mMenuMusicSource.isPlaying)
            {
                Instance.mMenuMusicSource.Play();
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        StartMusic();
        SetAudioLevelState(mAudioLevelState);
        mAudioPlayer = GetComponent<AudioPlayer>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }

    void StartMusic()
    {
        mMenuMusicSource = transform.AddComponent<AudioSource>();
        mMenuMusicSource.resource = mBGM.mSound[0];
        mMenuMusicSource.outputAudioMixerGroup = MusicMixer;
        mMenuMusicSource.loop = true;

        mGameMusicSource = transform.AddComponent<AudioSource>();
        mGameMusicSource.resource = mBGM.mSound[1];
        mGameMusicSource.outputAudioMixerGroup = MusicMixer;
        mGameMusicSource.loop = true;

        mMenuMusicSource.Play();
        mGameMusicSource.Play();
    }

    public void SetAudioLevelState(AudioLevelState audioLevelState)
    {
        mAudioLevelState = audioLevelState;

        switch (audioLevelState)
        {
            case AudioLevelState.Menu:
                mGameMusicSource.mute = true;
                mMenuMusicSource.mute = false;
                break;
            case AudioLevelState.Game:
                mGameMusicSource.mute = false;
                mMenuMusicSource.mute = true;
                break;
        }
    }

    public AudioSource PlayAudioEvent(AudioEvent audioEvent)
    {
        return mAudioPlayer.PlayAudioEvent(audioEvent);
    }
}
