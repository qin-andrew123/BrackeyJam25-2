using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSceneButtonUI : MonoBehaviour
{
    private Button mButton;
    private int mLevelToLoad = 0;
    private void Start()
    {
        mButton = gameObject.GetComponent<Button>();
    }
    private void OnDestroy()
    {
        mButton.onClick.RemoveListener(OnButtonClick);
    }

    public void InitializeButton(int levelToLoad, string buttonName)
    {
        mLevelToLoad = levelToLoad;
        TextMeshProUGUI buttonTextMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonTextMesh)
        {
            buttonTextMesh.text = buttonName;
        }
        if (!mButton)
        {
            mButton = gameObject.GetComponent<Button>();
        }
        mButton.onClick.AddListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        GlobalVariables.Instance.LoadScene(mLevelToLoad);
    }
}
