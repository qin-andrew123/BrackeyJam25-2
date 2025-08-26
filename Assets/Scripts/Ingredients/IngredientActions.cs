using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class IngredientActions : MonoBehaviour
{
    [SerializeField] private IngredientManager mIngredientManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckCombine()
    {
        Debug.Log("Trying to Combine Ingredients");

        Ingredient ingredient1 = mIngredientManager.mIngredientClicked;
        if (!ingredient1)
        {
            Debug.LogWarning("Cannot enter combine mode without selecting an ingredient initially");
            mIngredientManager.bIsCombining = false;
            return;
        }

        if (!mIngredientManager.bIsCombining)
        {
            mIngredientManager.bIsCombining = true;
            Debug.Log("Entered Combine Mode. Press 'combine' again to pick the second ingredient!");
            return;
        }

        Ingredient ingredient2 = mIngredientManager.mCombineCandidate;
        if (!ingredient2)
        {
            Debug.LogWarning("Cannot combine without a second selection");
            mIngredientManager.bIsCombining = false;
            return;
        }

        List<IngredientPair> combinablePairs = ingredient1.GetIngredientData().CombinableIngredients;
        foreach(IngredientPair pair in combinablePairs)
        {
            if(pair.ingredient == ingredient2.GetIngredientData())
            {
                Debug.Log("Combined Ingredients " + ingredient1 + " and " + ingredient2);
                Combine(pair.result);
                mIngredientManager.bIsCombining = false;
                return;
            }
        }

        mIngredientManager.bIsCombining = false;
        Debug.Log("Ingredients Not Combinable. Player did not select correct ones!");
    }

    private void Combine(IngredientDataSO result)
    {
        Debug.Log("Combining");
        mIngredientManager.Combine(result);

    }

    public void Mix()
    {
        Ingredient ingredient = mIngredientManager.mIngredientClicked;
        if(!ingredient.GetIngredientUsed())
        {
            mIngredientManager.AddToMixingBowl(ingredient);
        }
    }
}
