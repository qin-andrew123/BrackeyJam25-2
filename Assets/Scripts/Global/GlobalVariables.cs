using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelIndices
{
    MAIN_MENU = 0,
    BAKING_SCENE = 1,
    SCORE_SCENE = 2,
}
public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }
    public static event Action OnQuotaChange;
    public int DayNumber { get; private set; } = 0;
    public float QuotaNumber => mQuotaNumber;
    public TicketConstraint CurrentTicketConstraint { get; set; } = new TicketConstraint();
    public List<IngredientDataSO> IngredientData => mIngredientDataSO;
    public List<string> IngredientNames => mIngredientDataNames;
    public Dictionary<IngredientFlavor, List<IngredientDataSO>> FlavorDictionary => mFlavorDictionary;
    public Dictionary<string, IngredientDataSO> NameDictionary => mNameDictionary;
    public int TurnsPerRound => mTurnsPerRound;
    public int RoundsPerLevel => mRoundsPerLevel;
    public List<float> BiscuitValues { get; private set; } = new List<float>();
    public bool MetRoundFlavorRequirement { get; set; }
    public bool MetRoundNameRequirement { get; set; }


    [SerializeField] private List<float> mQuotaValues;
    [SerializeField] private List<IngredientDataSO> mIngredientDataSO;
    [SerializeField] private int mTurnsPerRound = 5;
    [SerializeField] private int mRoundsPerLevel = 5;

    private float mQuotaNumber = 0;
    private int mQuotaIndex = 0;
    private List<string> mIngredientDataNames = new List<string>();
    private Dictionary<IngredientFlavor, List<IngredientDataSO>> mFlavorDictionary = new Dictionary<IngredientFlavor, List<IngredientDataSO>>();
    private Dictionary<string, IngredientDataSO> mNameDictionary = new Dictionary<string, IngredientDataSO>();

    public void ModifyQuota()
    {
        mQuotaIndex = Mathf.Clamp(mQuotaIndex, 0, mQuotaValues.Count - 1);
        mQuotaNumber = mQuotaValues[mQuotaIndex];

        OnQuotaChange?.Invoke();
        mQuotaIndex++;
    }
    public void LoadScene(int levelNum)
    {
        LevelManager.LoadScene(levelNum);
    }
    public void AddBiscuitValue(int value)
    {
        BiscuitValues.Add(value);
    }
    public int TallyFinalLevelScore()
    {
        int result = 0;
        foreach (int score in BiscuitValues)
        {
            result += score;
        }

        return result;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (mIngredientDataSO.Count == 0)
        {
            Debug.LogError("ERROR: The list of ingredient data scriptable objects in TicketManager is empty");
            return;
        }

        foreach (IngredientDataSO ingredientData in mIngredientDataSO)
        {
            mIngredientDataNames.Add(ingredientData.IngredientName);

            IngredientFlavor indexFlavor = ingredientData.IngredientFlavorValue;
            if (!mFlavorDictionary.ContainsKey(indexFlavor))
            {
                mFlavorDictionary[indexFlavor] = new List<IngredientDataSO> { ingredientData };
            }
            else
            {
                mFlavorDictionary[indexFlavor].Add(ingredientData);
            }

            string indexName = ingredientData.IngredientName;

            mNameDictionary[indexName] = ingredientData;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelIndices loadedLevelIndex = (LevelIndices)scene.buildIndex;

        switch (loadedLevelIndex)
        {
            case LevelIndices.MAIN_MENU:
                break;
            case LevelIndices.BAKING_SCENE:
                // Starting a new level so we are clearing our values
                BiscuitValues.Clear();
                DayNumber++;
                LevelManager.OnBakeSceneLoaded(mRoundsPerLevel);
                break;
            case LevelIndices.SCORE_SCENE:
                int finalScore = TallyFinalLevelScore();
                bool bDidBeatLevel = finalScore >= mQuotaNumber ? true : false;
                LevelManager.OnScoreSceneLoaded(finalScore, BiscuitValues, mQuotaNumber, bDidBeatLevel);
                break;
            default:
                break;
        }
    }
}
