using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public VisualElement ui;
    public Button QuitButton;
    [SerializeField] UIDocument PauseMenuObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenuObj.gameObject.SetActive(false);
    }

    private void Awake()
    {
        ui = PauseMenuObj.GetComponent<UIDocument>().rootVisualElement;

    }

    private void OnEnable()
    {
        QuitButton = ui.Q<Button>("Quit");
        QuitButton.clicked += OnQuitButtonClicked;
    }

        // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Update");
            PauseMenuObj.gameObject.SetActive(!PauseMenuObj.gameObject.activeSelf);
        }
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
