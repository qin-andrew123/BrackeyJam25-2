using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour 
{
    public VisualElement ui;

    public Button NewGameButton;
    public Button OptionsButton;
    public Button QuitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

    }

    private void OnEnable()
    {
        NewGameButton = ui.Q<Button>("NewGame");
        NewGameButton.clicked += OnNewGameButtonClicked;

        OptionsButton = ui.Q<Button>("Options");
        OptionsButton.clicked += OnOptionsButtonClicked;

        QuitButton = ui.Q<Button>("Quit");
        QuitButton.clicked += OnQuitButtonClicked;
    }
    private void OnNewGameButtonClicked()
    {

    }
    private void OnOptionsButtonClicked()
    {
        //Place Options Transition Here
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
