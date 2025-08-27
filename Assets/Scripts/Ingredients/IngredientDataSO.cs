using System.Collections.Generic;
using UnityEngine;

public enum IngredientFlavor
{
    NONE = -1,
    BLAND = 0,
    BITTER,
    SWEET,
    SALTY,
    SOUR,
    UMAMI,

    // KEEP THIS AT THE END. If we ever add more flavors, we need to update this value
    TOTAL_FLAVORS = 6,
}


public enum IngredientEffect
{
    NONE = -1,
    BASE_VALUE,
    MULTIPLIER,
    ADDITIVE,
}


[CreateAssetMenu(fileName = "IngredientDataSO", menuName = "Scriptable Objects/IngredientDataSO")]
public class IngredientDataSO : ScriptableObject
{
    // Returns antagonizing flavor
    public static int GetAntagonizingIngredientFlavor(IngredientFlavor inputFlavor)
    {
        if (inputFlavor == IngredientFlavor.NONE)
        {
            return -1;
        }

        int offsetValue = (int)(IngredientFlavor.TOTAL_FLAVORS) / 2;
        return ((int)(inputFlavor) + offsetValue) % (int)(IngredientFlavor.TOTAL_FLAVORS);
    }

    // Returns the two synergistic ingredient flavors
    public static int[] GetSynergisticIngredientFlavors(IngredientFlavor inputFlavor)
    {
        int[] result = { -1, -1 };

        if (inputFlavor == IngredientFlavor.NONE)
        {
            return result;
        }
        int thisIndex = (int)(inputFlavor);

        int leftIndex = thisIndex - 1;
        if (leftIndex < 0)
        {
            // Could technically set this to UMAMI, but in case we want to add more
            leftIndex = (int)(IngredientFlavor.TOTAL_FLAVORS) - 1;
        }
        result[0] = leftIndex;

        int rightIndex = thisIndex + 1;
        if (rightIndex >= (int)(IngredientFlavor.TOTAL_FLAVORS))
        {
            rightIndex = 0;
        }
        result[1] = rightIndex;

        return result;
    }

    [SerializeField] private string mIngredientName = "NONE";
    public string IngredientName => mIngredientName;
    [SerializeField] private IngredientFlavor mIngredientFlavor = IngredientFlavor.NONE;
    public IngredientFlavor IngredientFlavorValue => mIngredientFlavor;
    [SerializeField] private IngredientEffect mIngredientEffect = IngredientEffect.NONE;
    public IngredientEffect IngredientEffectValue => mIngredientEffect;
    [SerializeField] private float mIngredientValue = 1.0f;
    public float IngredientValue => mIngredientValue;
}
