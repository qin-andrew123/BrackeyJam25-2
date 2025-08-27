using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{
    public static event Action OnBakeSceneLoad;
    public static event Action OnBakeSceneEnd;
    public static event Action OnScoreSceneLoad;
    public static event Action OnScoreSceneEnd;
    public static event Action OnMainMenuLoad;
    public static event Action OnGameOver;

    public void OnBakeSceneLoaded()
    {
        OnBakeSceneLoad?.Invoke();
    }
    public void OnBakeSceneEnded()
    {
        OnBakeSceneEnd?.Invoke();
    }
    public void OnScoreSceneLoaded()
    {
        OnScoreSceneLoad?.Invoke();
    }
    public void OnScoreSceneEnded()
    {
        OnScoreSceneEnd?.Invoke();
    }
    public void OnMainMenuLoaded()
    {
        OnMainMenuLoad?.Invoke();
    }
    public void LoadScene(int levelNum)
    {
        LevelIndices levelIndex = (LevelIndices)levelNum;
        switch(levelIndex)
        {
            case LevelIndices.MAIN_MENU:
            case LevelIndices.BAKING_SCENE:
            case LevelIndices.SCORE_SCENE:
                Debug.Log("Loading Level: " + levelIndex);
                SceneManager.LoadScene(levelNum);
                break;
            default:
                Debug.LogAssertion("INVALID LEVEL INDEX");
                break;
        }
    }
    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }
}
