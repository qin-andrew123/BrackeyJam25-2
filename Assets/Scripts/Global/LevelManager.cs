using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{
    public static event Action<int> OnBakeSceneLoad;
    public static event Action OnBakeSceneEnd;
    public static event Action<int, List<float>, float, bool> OnScoreSceneLoad;
    public static event Action OnScoreSceneEnd;
    public static event Action OnMainMenuLoad;
    public static event Action OnGameOver;

    public static void OnBakeSceneLoaded(int roundsPerLevel)
    {
        Debug.Log("loading baking scene. rounds: " + roundsPerLevel);
        GlobalVariables.Instance.ModifyQuota();
        OnBakeSceneLoad?.Invoke(roundsPerLevel);
    }
    public static void OnBakeSceneEnded()
    {
        // On bake scene end for UI to show that we have finished the level.
        OnBakeSceneEnd?.Invoke();
        LoadScene((int)LevelIndices.SCORE_SCENE);
    }
    public static void OnScoreSceneLoaded(int finalScore, List<float> individualBiscuitScores, float quotaNumber, bool bDidBeatLevel)
    {
        // Some sort of effects can go here
        OnScoreSceneLoad?.Invoke(finalScore, individualBiscuitScores, quotaNumber, bDidBeatLevel);
    }
    public static void OnScoreSceneEnded()
    {
        OnScoreSceneEnd?.Invoke();

    }
    public static void OnMainMenuLoaded()
    {
        OnMainMenuLoad?.Invoke();
    }
    public static void LoadScene(int levelNum)
    {
        LevelIndices levelIndex = (LevelIndices)levelNum;
        switch (levelIndex)
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
