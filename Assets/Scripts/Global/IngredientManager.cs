using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class IngredientPair
{
    public List<Ingredient> ingredientPrefabs;
    public IngredientDataSO resultPrefab;
}

public class IngredientManager : MonoBehaviour
{
    private void OnEnable()
    {
        Ingredient.OnIngredientClicked += HandleIngredientInteracted;
        TicketManager.OnTicketGenerated += SpawnIngredients;
    }
    private void OnDestroy()
    {
        Ingredient.OnIngredientClicked -= HandleIngredientInteracted;
        TicketManager.OnTicketGenerated -= SpawnIngredients;
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

    public void SpawnIngredients(TicketConstraint ticket)
    {
        bool bHasGeneratedFlavorType = false;
        bool bHasGeneratedIngredientName = false;
        List<IngredientDataSO> spawningPool = GlobalVariables.Instance.IngredientData;
        for (int i = 0; i < mSpawnIngredientLocations.Count; ++i)
        {
            int randomIndex = Random.Range(0, spawningPool.Count);
            GameObject newIngredient = Instantiate(mIngredientPrefab);
            newIngredient.GetComponent<Ingredient>().SetIngredientData(spawningPool[randomIndex]);
            newIngredient.transform.position = mSpawnIngredientLocations[i].position;

            mSpawnedIngredients.Add(newIngredient);
            if (spawningPool[randomIndex].IngredientFlavorValue == ticket.ingredientFlavor)
            {
                bHasGeneratedFlavorType = true;
            }
            if (spawningPool[randomIndex].IngredientName == ticket.ingredientName)
            {
                bHasGeneratedIngredientName = true;
            }
        }

        int flavorIndex = -1;
        if (!bHasGeneratedFlavorType)
        {
            for(int i = 0; i <mSpawnedIngredients.Count; ++i)
            {
                if(Random.Range(0f, 1f) < 0.4)
                {
                    GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavor);
                    int randomIndex = Random.Range(0, flavor.Count);
                    mSpawnedIngredients[i].GetComponent<Ingredient>().SetIngredientData(flavor[randomIndex]);
                    flavorIndex = i;
                }
            }
        }

        int nameIndex = -1;
        if (!bHasGeneratedIngredientName)
        {
            for (int i = mSpawnedIngredients.Count - 1; i >= 0; --i)
            {
                if (i != flavorIndex)
                {
                    GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavor);
                    int randomIndex = Random.Range(0, flavor.Count);
                    mSpawnedIngredients[i].GetComponent<Ingredient>().SetIngredientData(flavor[randomIndex]);
                    flavorIndex = i;
                }
            }
        }
    }
    public void AddToMixingBowl(Ingredient ingredient)
    {
        Debug.Log("Added " + ingredient + " to mixing bowl");
        mMixingBowlIngredients.Add(ingredient);
        ingredient.SetIngredientUsed(true);
        ingredient.gameObject.transform.position = mMixingBowlLocation.position;
        AddClickedIngredient(ingredient);
    }

    public void AddClickedIngredient(Ingredient ingredient)
    {
        if (mClickedIngredients.Contains(ingredient))
        {
            mClickedIngredients.Remove(ingredient);
            if (mClickedIngredients.Count > 0)
            {
                mIngredientUI.UpdatePopup(mClickedIngredients[mClickedIngredients.Count - 1].GetIngredientData());
            }
            else
            {
                mIngredientUI.HidePopup();
            }

        }
        else
        {
            mClickedIngredients.Add(ingredient);
            mIngredientUI.UpdatePopup(ingredient.GetIngredientData());
        }
    }

    public List<Ingredient> GetClickedIngredients()
    {
        return mClickedIngredients;
    }

    public List<IngredientPair> GetCombinableIngredients()
    {
        return mCombinableIngredients;
    }

    public void Combine(IngredientDataSO data)
    {
        // instantiate new prefab and delete previous ingredients
        Transform resultPos = mClickedIngredients[0].transform;
        foreach (Ingredient ingredient in mClickedIngredients)
        {
            Destroy(ingredient.gameObject);
        }
        mClickedIngredients.Clear();

        GameObject newIngredient = Instantiate(mIngredientPrefab);
        newIngredient.GetComponent<Ingredient>().SetIngredientData(data);
        newIngredient.transform.position = resultPos.position;
    }

    // TODO: Hook up UI with this value
    private float mScore = 0;
    public bool bIsCombining = false;
    [SerializeField] private List<Ingredient> mMixingBowlIngredients = new List<Ingredient>();
    [SerializeField] private List<Ingredient> mClickedIngredients = new List<Ingredient>();
    [SerializeField] private Transform mMixingBowlLocation;
    [SerializeField] private GameObject mIngredientPrefab;
    [SerializeField] private List<IngredientPair> mCombinableIngredients;
    [SerializeField] private IngredientUI mIngredientUI;
    [SerializeField] private List<Transform> mSpawnIngredientLocations;
    // TODO : Make this private
    public List<GameObject> mSpawnedIngredients;
}
