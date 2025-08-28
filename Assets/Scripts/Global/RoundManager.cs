using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public enum IngredientEffect
    {
        NONE = -1,
        BASE_VALUE,
        MULTIPLIER,
        ADDITIVE,
        SUBTRACTIVE,
        DIVISION
    }
    public IngredientFlavor[] RoundRequiredIngredients => mRoundRequiredIngredients;
    public static event Action<int> OnRoundStart;
    public static event Action OnRoundEnd;
    public static event Action<int> OnTurnUsed;
    public static event Action OnRoundCleanup;

    [SerializeField] private RoundUI mRoundUI;
    [SerializeField] private FlavorPieChart mFlavorPieChart;
    [SerializeField] private float mRoundScoreQuota; // Quota chosen by designers/programmatically?
    private int mRoundNumber = 0;
    private int mTurnsPerRound = 0;
    private int mRoundsForLevel = 0;
    private IngredientFlavor[] mRoundRequiredIngredients = { IngredientFlavor.NONE, IngredientFlavor.NONE };
    //[SerializeField] private string mBiscuitFlavorQuota; // ?
    private float mLevelScore = 0.0f;
    private float mRoundScore = 0.0f;
    private IngredientEffect mLastEffect = IngredientEffect.ADDITIVE;
    private FlavorData mRoundFlavors = new FlavorData();

    public void HandleTurnNumberChange()
    {
        Debug.Log("Handling Turn Number change");
        mTurnsPerRound--;

        if (mTurnsPerRound == 0)
        {
            // End the round which means store the value of the biscuit and reset 
            EndRound();
        }
        else
        {
            OnTurnUsed?.Invoke(mTurnsPerRound);
        }
    }
    public void CalculateRoundScore(IngredientFlavor prevFlavor, IngredientFlavor currFlavor, float value)
    {
        if (prevFlavor == IngredientFlavor.NONE)
        {
            mRoundScore += value;
            mLastEffect = IngredientEffect.ADDITIVE;
            mRoundUI.UpdateScoreText(mRoundScore);
            return;
        }

        int[] synergisticIngredientFlavors = IngredientDataSO.GetSynergisticIngredientFlavors(prevFlavor);
        if ((IngredientFlavor)synergisticIngredientFlavors[0] == currFlavor || (IngredientFlavor)synergisticIngredientFlavors[1] == currFlavor)
        {
            if (mLastEffect == IngredientEffect.ADDITIVE || mLastEffect == IngredientEffect.MULTIPLIER)
            {
                mRoundScore *= value; // Multiplicative
                mLastEffect = IngredientEffect.MULTIPLIER;
            }
            else
            {
                mRoundScore += value; // Everything else is additive if the two are synergistic
                mLastEffect = IngredientEffect.ADDITIVE;
            }
        }
        else if ((IngredientFlavor)IngredientDataSO.GetAntagonizingIngredientFlavor(prevFlavor) == currFlavor)
        {
            if (mLastEffect == IngredientEffect.SUBTRACTIVE || mLastEffect == IngredientEffect.DIVISION)
            {
                mRoundScore /= value; // Divisive
                mLastEffect = IngredientEffect.DIVISION;
            }
            else
            {
                mRoundScore -= value; // Everything else is subtractive
                mLastEffect = IngredientEffect.SUBTRACTIVE;
            }
        }
        else
        {
            mRoundScore += value; // neutral. Additive
            mLastEffect = IngredientEffect.ADDITIVE;
        }

        mRoundUI.UpdateScoreText(mRoundScore);
    }

    public void CalculateRoundFlavor(FlavorData ingredientFlavor)
    {
        mRoundFlavors += ingredientFlavor;
        mFlavorPieChart.SetValues(mRoundFlavors);
    }

    public void AddLevelScore()
    {
        mLevelScore += mRoundScore;
    }

    public bool MetQuota()
    {
        return mLevelScore >= mRoundScoreQuota;
    }

    // Called when the level starts to mark the start of all rounds for the level
    private void InitiateRounds(int roundsPerLevel)
    {
        mRoundsForLevel = roundsPerLevel;
        mTurnsPerRound = GlobalVariables.Instance.TurnsPerRound;
        OnTurnUsed?.Invoke(mTurnsPerRound);
        mRoundNumber++;
        Debug.Log("Rounds: " + mRoundsForLevel);
        Debug.Log("Turns: " + mTurnsPerRound);

        // This is mainly for effects
        OnRoundStart?.Invoke(mRoundNumber);
    }
    private void StartNextRound()
    {
        if (mRoundNumber == mRoundsForLevel)
        {
            LevelManager.OnBakeSceneEnded();
            return;
        }

        mRoundNumber++;
        mTurnsPerRound = GlobalVariables.Instance.TurnsPerRound;
        OnTurnUsed?.Invoke(mTurnsPerRound);
        // This is mainly for effects
        OnRoundStart?.Invoke(mRoundNumber);
    }
    private void EndRound()
    {
        // This is mainly for UI Effect the invoke.
        OnRoundEnd?.Invoke();

        // Probably want a coroutine that wait for X seconds

        // Actual cleanup happens here
        OnRoundCleanup?.Invoke();

        // Start the next round
        StartNextRound();
    }


    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnEnable()
    {
        PlayerInput.TEMPSetRoundRequirements += InitiateRounds;
        LevelManager.OnBakeSceneLoad += InitiateRounds;
        IngredientManager.OnIngredientMixed += HandleTurnNumberChange;
    }
    private void OnDisable()
    {
        PlayerInput.TEMPSetRoundRequirements -= InitiateRounds;
        LevelManager.OnBakeSceneLoad -= InitiateRounds;
        IngredientManager.OnIngredientMixed -= HandleTurnNumberChange;
    }

    //private void SetRequiredItems()
    //{
    //    // TODO: add weighting or something else if we want that. For now we are just randomly picking
    //    int randomDifficulty = UnityEngine.Random.Range((int)(DifficultySetting.EASY), (int)(DifficultySetting.HARD));

    //    // TODO: We can also customize this to be better as well, random selection
    //    int starterFlavor = UnityEngine.Random.Range((int)IngredientFlavor.BLAND, (int)IngredientFlavor.TOTAL_FLAVORS - 1);
    //    int[] synergies = IngredientDataSO.GetSynergisticIngredientFlavors((IngredientFlavor)starterFlavor);
    //    int antagonist = IngredientDataSO.GetAntagonizingIngredientFlavor((IngredientFlavor)starterFlavor);

    //    mRoundRequiredIngredients[0] = (IngredientFlavor)starterFlavor;

    //    Debug.Log("Difficulty: " + (DifficultySetting)randomDifficulty);
    //    switch ((DifficultySetting)randomDifficulty)
    //    {
    //        case DifficultySetting.EASY:
    //            mRoundRequiredIngredients[1] = (IngredientFlavor)synergies[UnityEngine.Random.Range(0, 1)];
    //            break;
    //        case DifficultySetting.MEDIUM:
    //            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
    //            {
    //                mRoundRequiredIngredients[1] = (IngredientFlavor)antagonist;
    //            }
    //            else
    //            {
    //                mRoundRequiredIngredients[1] = (IngredientFlavor)synergies[UnityEngine.Random.Range(0, 1)];
    //            }
    //            break;
    //        case DifficultySetting.HARD:
    //            mRoundRequiredIngredients[1] = (IngredientFlavor)antagonist;
    //            break;
    //        default:
    //            Debug.Log("Improper Difficulty Value, something wrong happened");
    //            return;
    //    }

    //    OnSelectedIngredients?.Invoke(mRoundRequiredIngredients);
    //}
}
