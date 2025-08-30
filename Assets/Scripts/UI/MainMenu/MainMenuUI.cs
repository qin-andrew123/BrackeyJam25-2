using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonUI : MonoBehaviour
{
    [SerializeField] private Button mPlayButton;
    private void Start()
    {
        mPlayButton.onClick.AddListener(OnButtonClick);
    }
    private void OnDestroy()
    {
        mPlayButton.onClick.RemoveListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        if (!GlobalVariables.Instance.bDidWatchIntroCinematic)
        {
            GlobalVariables.Instance.LoadScene((int)LevelIndices.STORY_SCENE);
        }
        else
        {
            GlobalVariables.Instance.LoadScene((int)LevelIndices.BAKING_SCENE);
        }
    }
}
