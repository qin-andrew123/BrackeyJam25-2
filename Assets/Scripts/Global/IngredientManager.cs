using NUnit.Framework;
using System.Collections.Generic;
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

    public void AddToMixingBowl(Ingredient ingredient)
    {
        Debug.Log("Added " + ingredient + " to mixing bowl");
        mMixingBowlIngredients.Add(ingredient);
        ingredient.SetIngredientUsed(true);
        ingredient.gameObject.transform.position = mMixingBowlLocation.position;
    }

    public void Combine(IngredientDataSO data)
    {
        // instantiate new prefab and delete previous ingredients
        Transform resultPos = mIngredientClicked.transform;
        Destroy(mIngredientClicked.gameObject);
        Destroy(mCombineCandidate.gameObject);

        GameObject newIngredient = Instantiate(mIngredientPrefab);
        newIngredient.GetComponent<Ingredient>().SetIngredientData(data);
        newIngredient.transform.position = transform.position;
    }

    // TODO: Hook up UI with this value
    private float mScore = 0;
    public Ingredient mIngredientClicked = null;
    public Ingredient mCombineCandidate = null;
    public bool bIsCombining = false;
    [SerializeField] private List<Ingredient> mMixingBowlIngredients = new List<Ingredient>();
    [SerializeField] private Transform mMixingBowlLocation;
    [SerializeField] private GameObject mIngredientPrefab;

}
