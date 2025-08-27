using System;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static event Action<int> OnRoundStart;
    public static event Action OnRoundEnd;
    private int mRoundNumber = 0;

    private void InitiateNextRound()
    {
        mRoundNumber++;
        OnRoundStart?.Invoke(mRoundNumber);
    }
    private void EndRound()
    {
        OnRoundEnd?.Invoke();
    }

    public enum IngredientEffect
    {
        NONE = -1,
        BASE_VALUE,
        MULTIPLIER,
        ADDITIVE,
        SUBTRACTIVE,
        DIVISION
    }

    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnEnable()
    {
        LevelManager.OnBakeSceneLoad += InitiateNextRound;
    }
    private void OnDisable()
    {
        LevelManager.OnBakeSceneLoad -= InitiateNextRound;
    }

    private void SetRequiredItems()
    {
        // TODO: add weighting or something else if we want that. For now we are just randomly picking
        int randomDifficulty = UnityEngine.Random.Range((int)(DifficultySetting.EASY), (int)(DifficultySetting.HARD));

        // TODO: We can also customize this to be better as well, random selection
        int starterFlavor = UnityEngine.Random.Range((int)IngredientFlavor.BLAND, (int)IngredientFlavor.TOTAL_FLAVORS - 1);
        int[] synergies = IngredientDataSO.GetSynergisticIngredientFlavors((IngredientFlavor)starterFlavor);
        int antagonist = IngredientDataSO.GetAntagonizingIngredientFlavor((IngredientFlavor)starterFlavor);

        mRoundRequiredIngredients[0] = (IngredientFlavor)starterFlavor;

        Debug.Log("Difficulty: " + (DifficultySetting)randomDifficulty);
        switch ((DifficultySetting)randomDifficulty)
        {
            case DifficultySetting.EASY:
                mRoundRequiredIngredients[1] = (IngredientFlavor)synergies[UnityEngine.Random.Range(0, 1)];
                break;
            case DifficultySetting.MEDIUM:
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                {
                    mRoundRequiredIngredients[1] = (IngredientFlavor)antagonist;
                }
                else
                {
                    mRoundRequiredIngredients[1] = (IngredientFlavor)synergies[UnityEngine.Random.Range(0, 1)];
                }
                break;
            case DifficultySetting.HARD:
                mRoundRequiredIngredients[1] = (IngredientFlavor)antagonist;
                break;
            default:
                Debug.Log("Improper Difficulty Value, something wrong happened");
                return;
        }

        OnSelectedIngredients?.Invoke(mRoundRequiredIngredients);
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
            if(mLastEffect == IngredientEffect.ADDITIVE || mLastEffect == IngredientEffect.MULTIPLIER)
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
        else if((IngredientFlavor)IngredientDataSO.GetAntagonizingIngredientFlavor(prevFlavor) == currFlavor)
        {
            if(mLastEffect == IngredientEffect.SUBTRACTIVE || mLastEffect == IngredientEffect.DIVISION)
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

    public void AddLevelScore()
    {
        mLevelScore += mRoundScore;
    }

    public bool MetQuota()
    {
        return mLevelScore >= mRoundScoreQuota;
    }

    private IngredientFlavor[] mRoundRequiredIngredients = { IngredientFlavor.NONE, IngredientFlavor.NONE };
    public IngredientFlavor[] RoundRequiredIngredients => mRoundRequiredIngredients;

    [SerializeField] private float mRoundScoreQuota; // Quota chosen by designers/programmatically?
    //[SerializeField] private string mBiscuitFlavorQuota; // ?
    private float mLevelScore = 0.0f;
    private float mRoundScore = 0.0f;
    private IngredientEffect mLastEffect = IngredientEffect.ADDITIVE;
    [SerializeField] private RoundUI mRoundUI;
}
