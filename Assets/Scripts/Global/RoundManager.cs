using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static event Action<IngredientFlavor[]> OnSelectedIngredients;
    enum DifficultySetting
    {
        EASY,
        MEDIUM,
        HARD
    }

    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnEnable()
    {
        PlayerInput.TEMPSetRoundRequirements += SetRequiredItems;
    }
    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnDisable()
    {
        PlayerInput.TEMPSetRoundRequirements -= SetRequiredItems;
    }

    private void SetRequiredItems()
    {
        // TODO: add weighting or somethign else if we want that. For now we are just randomly picking
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

    private IngredientFlavor[] mRoundRequiredIngredients = { IngredientFlavor.NONE, IngredientFlavor.NONE };
    public IngredientFlavor[] RoundRequiredIngredients => mRoundRequiredIngredients;
}
