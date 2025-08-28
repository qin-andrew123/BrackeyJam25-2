using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientActions : MonoBehaviour
{
    [SerializeField] private IngredientManager mIngredientManager;
    public void CheckCombine()
    {
        Debug.Log("Trying to Combine Ingredients");

        if (mIngredientManager.GetClickedIngredients().Count < 2)
        {
            Debug.LogWarning("Cannot enter combine mode without selecting at least two ingredients!");
            return;
        }

        List<IngredientPair> combinablePairs = mIngredientManager.GetCombinableIngredients();
        List<Ingredient> clickedIngredients = mIngredientManager.GetClickedIngredients();
        foreach (IngredientPair pair in combinablePairs)
        {
            bool bCanCombine = true;
            List<IngredientDataSO> ingredients = pair.ingredientPrefabs;
            foreach (Ingredient ingredient in clickedIngredients)
            {
                if (!ingredients.Contains(ingredient.GetIngredientData()))
                {
                    bCanCombine = false;
                    break;
                }
            }
            if (bCanCombine)
            {
                Debug.Log("Combining");
                mIngredientManager.Combine(pair.resultPrefab);

                return;
            }
        }

        Debug.Log("Ingredients Not Combinable. Player did not select correct ones!");
    }

    public void Mix()
    {
        if(mIngredientManager.GetClickedIngredients().Count != 1)
        {
            Debug.LogWarning("Error: You can only mix one ingredient at a time!");
            return;
        }
        Ingredient ingredient = mIngredientManager.GetClickedIngredients()[0];
        if (ingredient.GetIngredientUsed())
        {
            Debug.LogWarning("Cannot mix. Ingredient already in mixing bowl");
            return;
        }

        mIngredientManager.AddToMixingBowl(ingredient);
    }
}
