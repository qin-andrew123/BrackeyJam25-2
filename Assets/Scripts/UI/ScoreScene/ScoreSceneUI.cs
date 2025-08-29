using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject mScorePrefab;
    [SerializeField, Tooltip("The parent where you will in code put all the score prefabs")]
    private Transform mScorePrefabParent;
    [SerializeField] private GameObject mButtonPrefab;
    [SerializeField, Tooltip("The parent where you will in code put all the score prefabs")]
    private Transform mButtonPrefabParent;
    [SerializeField] private TextMeshProUGUI mScoreTitleText;
    [SerializeField] private TextMeshProUGUI mScoreDescriptionText;
    [SerializeField] private TextMeshProUGUI mScoreFlavorText;
    [SerializeField] private TextMeshProUGUI mScoreText;

    private void OnEnable()
    {
        LevelManager.OnScoreSceneLoad += PopulateScoreData;
    }
    private void OnDisable()
    {
        LevelManager.OnScoreSceneLoad -= PopulateScoreData;
    }
    private void PopulateScoreData(int finalScore, List<float> scoreList, float quotaNumber, bool bDidBeatLevel)
    {
        string titleText = bDidBeatLevel ? "CONGRATS!" : "OH NO!";
        mScoreTitleText.text = titleText;

        string descriptionText = bDidBeatLevel ? "You made the quota!" : "You didn't make the quota...";
        mScoreDescriptionText.text = descriptionText;

        string flavorText = bDidBeatLevel ? "Your family is safe for now..." : "The dog mob will be looking for you!";
        mScoreFlavorText.text = flavorText;

        mScoreText.text = $"SCORE: {finalScore}";

#if UNITY_EDITOR
        Assert.IsTrue(scoreList.Count > 0, "We are passing a 0 sized scorelist to the scorescene. this will cause a divide by 0!");
#endif

        float hypotheticalMaxScore = finalScore / scoreList.Count;
        // Totally failed
        if(hypotheticalMaxScore == 0.0f)
        {
            hypotheticalMaxScore = float.MaxValue;
        }

        for (int i = 0; i < scoreList.Count; i++)
        {
            GameObject newScoreItem = Instantiate(mScorePrefab, mScorePrefabParent);
            ScorePrefabUI scorePrefabUI = newScoreItem.GetComponent<ScorePrefabUI>();
            float sliderValue = Math.Clamp(scoreList[i] / hypotheticalMaxScore, 0.0f, 1.0f);
            scorePrefabUI.InitializeSliderValue(sliderValue, i + 1);
        }

        GameObject mainMenuButton = Instantiate(mButtonPrefab, mButtonPrefabParent);
        ScoreSceneButtonUI menuButton = mainMenuButton.GetComponent<ScoreSceneButtonUI>();
        menuButton.InitializeButton((int)LevelIndices.MAIN_MENU, "Main Menu");
        if (bDidBeatLevel)
        {
            GameObject nextLevelButton = Instantiate(mButtonPrefab, mButtonPrefabParent);
            ScoreSceneButtonUI nextLevel = nextLevelButton.GetComponent<ScoreSceneButtonUI>();
            nextLevel.InitializeButton((int)LevelIndices.BAKING_SCENE, "Continue");
        }

    }
}
