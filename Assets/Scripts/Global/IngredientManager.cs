using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class IngredientPair
{
    public List<IngredientDataSO> ingredientPrefabs;
    public IngredientDataSO resultPrefab;
}

public class IngredientManager : MonoBehaviour
{
    public static event Action<IngredientDataSO> OnIngredientMixed;
    public bool bIsCombining = false;
    [SerializeField] private List<Ingredient> mMixingBowlIngredients = new List<Ingredient>();
    [SerializeField] private List<Ingredient> mClickedIngredients = new List<Ingredient>();
    [SerializeField] private Transform mMixingBowlLocation;
    [SerializeField] private GameObject mIngredientPrefab;
    [SerializeField] private List<IngredientPair> mCombinableIngredients;
    [SerializeField] private IngredientUI mIngredientUI;
    [SerializeField] private List<Transform> mSpawnIngredientLocations;
    private List<GameObject> mSpawnedIngredients = new List<GameObject>();
    [SerializeField] private RoundManager mRoundManager;

    public void SpawnIngredients(TicketConstraint ticket)
    {
        CleanupSpawnedIngredients();

        bool bHasGeneratedFlavorType = false;
        bool bHasGeneratedIngredientName = false;

        // Create the spawning pool and sort the list randomly
        List<IngredientDataSO> spawningPool = new List<IngredientDataSO>(GlobalVariables.Instance.IngredientData);
        spawningPool = spawningPool.OrderBy(x => Random.value).ToList();

        // Take the first number to spawn as our selected
        List<IngredientDataSO> selectedIngredients = spawningPool.Take(mSpawnIngredientLocations.Count).ToList();

        // Check to ensure that we have the required flavor and value
        for (int i = 0; i < selectedIngredients.Count; ++i)
        {
            if (selectedIngredients[i].IngredientFlavorValue == ticket.ingredientFlavor)
            {
                bHasGeneratedFlavorType = true;
            }
            if (selectedIngredients[i].IngredientName == ticket.ingredientName)
            {
                bHasGeneratedIngredientName = true;
            }
        }

        // If we don't manually override the indices to get the ones we want
        int flavorIndex = 0;
        if (!bHasGeneratedFlavorType)
        {
            flavorIndex = Random.Range(0, mSpawnIngredientLocations.Count);
            GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavor);
            int randomFlavorIndex = Random.Range(0, flavor.Count);
            selectedIngredients[flavorIndex] = flavor[randomFlavorIndex];
        }

        if (!bHasGeneratedIngredientName)
        {
            for (int i = selectedIngredients.Count - 1; i >= 0; --i)
            {
                if (i != flavorIndex)
                {
                    GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavor);
                    int randomIndex = Random.Range(0, flavor.Count);
                    selectedIngredients[i] = flavor[randomIndex];
                    break;
                }
            }
        }

        for (int i = 0; i < mSpawnIngredientLocations.Count; ++i)
        {
            GameObject newIngredient = Instantiate(mIngredientPrefab);
            newIngredient.GetComponent<Ingredient>().SetIngredientData(selectedIngredients[i]);
            newIngredient.transform.position = mSpawnIngredientLocations[i].position;

            mSpawnedIngredients.Add(newIngredient);
        }
    }
    public void AddToMixingBowl(Ingredient ingredient)
    {
        Debug.Log("Added " + ingredient + " to mixing bowl");

        IngredientFlavor prevFlavor = IngredientFlavor.NONE;
        IngredientFlavor currFlavor = ingredient.GetIngredientData().IngredientFlavorValue;

        if (mMixingBowlIngredients.Count > 1)
        {
            prevFlavor = mMixingBowlIngredients[mMixingBowlIngredients.Count - 1].GetIngredientData().IngredientFlavorValue;
        }

        mMixingBowlIngredients.Add(ingredient);
        ingredient.SetIngredientUsed(true);
        ingredient.gameObject.transform.position = mMixingBowlLocation.position;
        AddClickedIngredient(ingredient);

        mRoundManager.CalculateRoundScore(prevFlavor, currFlavor, ingredient.GetIngredientData().IngredientValue);
        mRoundManager.CalculateRoundFlavor(ingredient.GetIngredientData().mFlavorData);
        // We need to decrement the number of turns because we just used one
        // TODO: Pass in the value of the ingredient mixed in so that we can ensure that we are getting the required values.
        OnIngredientMixed?.Invoke(ingredient.GetIngredientData());
    }

    public void AddClickedIngredient(Ingredient ingredient)
    {
        if (mClickedIngredients.Contains(ingredient))
        {
            ingredient.GetMesh().layer = LayerMask.NameToLayer("Ingredient");
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
            ingredient.GetMesh().layer = LayerMask.NameToLayer("Outline");
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

    private void OnEnable()
    {
        TicketManager.OnTicketGenerated += SpawnIngredients;
        RoundManager.OnRoundCleanup += HandleIngredientCleanup;
    }
    private void OnDestroy()
    {
        TicketManager.OnTicketGenerated -= SpawnIngredients;
        RoundManager.OnRoundCleanup -= HandleIngredientCleanup;
    }

    private void HandleIngredientCleanup()
    {
        CleanupSpawnedIngredients();
        CleanupCombinedIngredients();
        CleanupMixingBowl();
    }
    private void CleanupSpawnedIngredients()
    {
        if (mSpawnedIngredients.Count == 0)
        {
            return;
        }

        foreach (GameObject go in mSpawnedIngredients)
        {
            Destroy(go);
        }
    }
    private void CleanupCombinedIngredients()
    {
        mClickedIngredients.Clear();
        Debug.Log("ClickedIngredients count: " + mClickedIngredients.Count);
    }
    private void CleanupMixingBowl()
    {
        mMixingBowlIngredients.Clear();
        Debug.Log("mMixingBowlIngredients count: " + mMixingBowlIngredients.Count);
    }
}
