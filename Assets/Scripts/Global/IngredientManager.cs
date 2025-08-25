using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    private void OnEnable()
    {
        Ingredient.OnIngredientClicked += HandleIngredientInteracted;
    }
    private void OnDestroy()
    {
        Ingredient.OnIngredientClicked -= HandleIngredientInteracted;
    }

    private void HandleIngredientInteracted(IngredientDataSO inputIngredient)
    {
        // TODO: more complex math calculations will happen in the future but we can for now just debug log our values:
        string ingredientName = inputIngredient.IngredientName;
        IngredientFlavor ingredientFlavor = inputIngredient.IngredientFlavorValue;
        IngredientEffect ingredientEffect = inputIngredient.IngredientEffectValue;
        float ingredientValue = inputIngredient.IngredientValue;
        int antagonistIngredient = IngredientDataSO.GetAntagonizingIngredientFlavor(ingredientFlavor);
        int[] synergisticIngredients = IngredientDataSO.GetSynergisticIngredientFlavors(ingredientFlavor);

        // This should never happen but if the count of syntergistic ingredients is != 2 we assert return;
        if (synergisticIngredients.Length != 2)
        {
            Debug.LogAssertion("Something has gone wrong. We have more than 2 synergistic ingredients");
            return;
        }

        if (ingredientFlavor == IngredientFlavor.NONE)
        {
            Debug.LogWarning("The ingredient interated with has a none flavor value. Is this correct?");
        }

        if (ingredientEffect == IngredientEffect.NONE)
        {
            Debug.LogWarning("The ingredient interated with has a none effect value. Is this correct?");
        }

        Debug.Log("Ingredient: " + ingredientName + " was interacted with! \n It is flavor: " + ingredientFlavor +
            "\n It is effect: " + ingredientEffect + "\n It has a value of: " + ingredientValue +
            "\n Its antagnoist ingredient is: " + (IngredientFlavor)(antagonistIngredient) +
            "\n Its synergistic ingredients are: {" + (IngredientFlavor)(synergisticIngredients[0]) + "," + (IngredientFlavor)(synergisticIngredients[1]) + "}");
    }

    // TODO: Hook up UI with this value
    private float mScore = 0;
}
