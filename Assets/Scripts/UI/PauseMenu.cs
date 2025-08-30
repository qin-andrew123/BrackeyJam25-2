using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    //public VisualElement ui;
    public Button QuitButton;
    //[SerializeField] UIDocument PauseMenuObj;
    [SerializeField] private GameObject mPauseMenu;
    [SerializeField] private TutorialUI mTutorialMenu;
    private bool bIsPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mPauseMenu.SetActive(false);
    }

    private void Awake()
    {
        //ui = PauseMenuObj.GetComponent<UIDocument>().rootVisualElement;

    }

    private void OnEnable()
    {
        //QuitButton = ui.Q<Button>("Quit");
        //QuitButton.clicked += OnQuitButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            ShowPause();
        }
    }

    public void ShowPause()
    {
        Debug.Log("Pause");
        bIsPaused = !bIsPaused;
        mPauseMenu.SetActive(bIsPaused);
    }

    public void ShowTutorial()
    {
        mTutorialMenu.gameObject.SetActive(true);
        mTutorialMenu.OpenTutorial();
        ShowPause();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
