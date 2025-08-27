using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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

    public int DayNumber { get; private set; } = 0;
    public int QuotaNumber { get; set; } = 0;
    public List<IngredientDataSO> IngredientData => mIngredientDataSO;
    public List<string> IngredientNames => mIngredientDataNames;
    public Dictionary<IngredientFlavor, List<IngredientDataSO>> FlavorDictionary => mFlavorDictionary;
    public Dictionary<string, IngredientDataSO> NameDictionary => mNameDictionary;

    private RoundManager mRoundManager;
    private LevelManager mLevelManager;
    [SerializeField] private List<IngredientDataSO> mIngredientDataSO;
    private List<string> mIngredientDataNames = new List<string>();
    private Dictionary<IngredientFlavor, List<IngredientDataSO>> mFlavorDictionary = new Dictionary<IngredientFlavor, List<IngredientDataSO>>();
    private Dictionary<string, IngredientDataSO> mNameDictionary = new Dictionary<string, IngredientDataSO>();
    public void LoadScene(int levelNum)
    {
        mLevelManager.LoadScene(levelNum);
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

        mLevelManager = new LevelManager();

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
            Assert.IsFalse(mNameDictionary.ContainsKey(indexName), "Assertion: There is a duplicate name value");

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
                DayNumber++;
                mLevelManager.OnBakeSceneLoaded();
                break;
            case LevelIndices.SCORE_SCENE:
                mLevelManager.OnScoreSceneLoaded();
                break;
            default:
                break;
        }
    }
}
