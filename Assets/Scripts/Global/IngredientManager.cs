using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    public static event Action<GameObject> OnCleanupIngredients;
    public bool bIsCombining = false;
    [SerializeField] private List<Ingredient> mMixingBowlIngredients = new List<Ingredient>();
    [SerializeField] private List<Ingredient> mClickedIngredients = new List<Ingredient>();
    [SerializeField] private Transform mMixingBowlLocation;
    [SerializeField] private GameObject mIngredientPrefab;
    [SerializeField] private List<IngredientPair> mCombinableIngredients;
    [SerializeField] private IngredientUI mIngredientUI;
    [SerializeField] private List<Transform> mSpawnIngredientLocations;
    [SerializeField] private RoundManager mRoundManager;
    [SerializeField] private float mChanceToSpawnRequired = 0.7f;
    public static List<GameObject> mSpawnedIngredients = new List<GameObject>();
    public static Dictionary<Transform, bool> mTransformToHasSpawnedIngredient = new Dictionary<Transform, bool>();

    public static void AddIngredient(GameObject go)
    {
        mSpawnedIngredients.Add(go);
    }
    // TODO : Refactor Spawn Ingredients
    public void SpawnIngredients()
    {
        CleanupSpawnedIngredients();

        GenerateIngredients(mSpawnIngredientLocations);
    }

    public void AddToMixingBowl()
    {
        if (mClickedIngredients.Count != 1)
        {
            Debug.LogWarning("Error: You can only mix one ingredient at a time!");
            CleanupClickedIngredients();
            return;
        }
        Ingredient ingredient = mClickedIngredients[0];

        // TODO : We probably do not require this since we will delete all the ingredients on mix anyways
        if (ingredient.GetIngredientUsed())
        {
            Debug.LogWarning("Cannot mix. Ingredient already in mixing bowl");
            CleanupClickedIngredients();
            return;
        }

        Debug.Log("Added " + ingredient + " to mixing bowl");

        IngredientFlavor prevFlavor = IngredientFlavor.NONE;
        IngredientFlavor currFlavor = ingredient.IngredientData.IngredientFlavorValue;

        if (mMixingBowlIngredients.Count > 1)
        {
            prevFlavor = mMixingBowlIngredients[mMixingBowlIngredients.Count - 1].IngredientData.IngredientFlavorValue;
        }

        mMixingBowlIngredients.Add(ingredient);
        ingredient.SetIngredientUsed(true);
        ingredient.gameObject.transform.position = mMixingBowlLocation.position;
        ClickedIngredientsRemove(ingredient);

        // TODO: We should delete the mixed in obj since we want the mixing bowl to represent what we have.

        mRoundManager.CalculateRoundScore(prevFlavor, currFlavor, ingredient.IngredientData.IngredientValue);
        mRoundManager.CalculateRoundFlavor(ingredient.IngredientData.mFlavorData);
        // We need to decrement the number of turns because we just used one
        // TODO: Pass in the value of the ingredient mixed in so that we can ensure that we are getting the required values.
        OnIngredientMixed?.Invoke(ingredient.IngredientData);
        OnCleanupIngredients?.Invoke(ingredient.gameObject);

        RespawnIngredients();
    }

    private void ClickedIngredientsTryAdd(Ingredient ingredient)
    {

        Debug.Log("Adding Clicked Ingredient");
        if (mClickedIngredients.Contains(ingredient))
        {
            ClickedIngredientsRemove(ingredient);
        }
        else
        {
            mClickedIngredients.Add(ingredient);
            mIngredientUI.UpdatePopup(ingredient.IngredientData);
            ingredient.gameObject.layer = LayerMask.NameToLayer("Outline");
        }
    }
    private void ClickedIngredientsRemove(Ingredient ingredient)
    {
        if(!ingredient.gameObject)
        {
            return;
        }

        Debug.Log("Removing Clicked Ingredient");
        ingredient.gameObject.layer = LayerMask.NameToLayer("Ingredient");
        mClickedIngredients.Remove(ingredient);
        if (mClickedIngredients.Count > 0)
        {
            mIngredientUI.UpdatePopup(mClickedIngredients[mClickedIngredients.Count - 1].IngredientData);
        }
        else
        {
            mIngredientUI.HidePopup();
        }
    }

    public void Combine()
    {
        Debug.Log("Trying to Combine Ingredients");

        if (mClickedIngredients.Count < 2)
        {
            Debug.LogWarning("Cannot enter combine mode without selecting at least two ingredients!");
            return;
        }

        foreach (IngredientPair pair in mCombinableIngredients)
        {
            bool bCanCombine = true;
            List<IngredientDataSO> ingredients = pair.ingredientPrefabs;
            foreach (Ingredient ingredient in mClickedIngredients)
            {
                if (!ingredients.Contains(ingredient.IngredientData))
                {
                    bCanCombine = false;
                    break;
                }
            }
            if (bCanCombine)
            {
                Debug.Log("Combining");
                Transform resultPos = mClickedIngredients[0].IngredientData.InitialSpawnTransform;
                for(int i = mClickedIngredients.Count - 1; i >= 0; --i)
                {
                    OnCleanupIngredients?.Invoke(mClickedIngredients[i].gameObject);
                }
                mClickedIngredients.Clear();

                // instantiate new prefab and delete previous ingredients
                GameObject newIngredient = Instantiate(mIngredientPrefab);
                HandleNewIngredientSpawned(newIngredient, resultPos, pair.resultPrefab);

                RespawnIngredients();

                return;
            }
        }

        Debug.Log("Ingredients Not Combinable. Player did not select correct ones!");
        CleanupClickedIngredients();
    }
    private void RespawnIngredients()
    {
        List<Transform> spotsToRespawnIngredients = new List<Transform>();
        foreach (var KeyValue in mTransformToHasSpawnedIngredient)
        {
            if (!KeyValue.Value)
            {
                Debug.Log("Key: " + KeyValue.Key.position + " | Value: " + KeyValue.Value);
                spotsToRespawnIngredients.Add(KeyValue.Key);
            }
        }

        GenerateIngredients(spotsToRespawnIngredients);
    }
    private void GenerateIngredients(List<Transform> spotsToSpawnIngredients)
    {
        Debug.Log("Total Number of items to respawn: " + spotsToSpawnIngredients.Count);
        bool bHasGeneratedFlavorType = false;
        bool bHasGeneratedIngredientName = false;
        TicketConstraint ticket = GlobalVariables.Instance.CurrentTicketConstraint;

        // TODO Weighting
        // Create the spawning pool and sort the list randomly
        List<IngredientDataSO> spawningPool = new List<IngredientDataSO>(GlobalVariables.Instance.IngredientData);
        spawningPool = spawningPool.OrderBy(x => Random.value).ToList();

        // Take the first number to spawn as our selected
        List<IngredientDataSO> selectedIngredients = spawningPool.Take(spotsToSpawnIngredients.Count).ToList();

        // TODO : Fix it so that there is a high chance of getting the required, but only if you haven't used it yet. Otherwise, just do the basic randomization.

        // Check to ensure that we have the required flavor and value
        for (int i = 0; i < selectedIngredients.Count; ++i)
        {
            if (selectedIngredients[i].IngredientFlavorValue == ticket.ingredientFlavor)
            {
                Debug.Log($"Index {i}: We got flavor {selectedIngredients[i].IngredientFlavorValue} | We need {ticket.ingredientFlavor}");
                bHasGeneratedFlavorType = true;
            }
            if (selectedIngredients[i].IngredientName == ticket.ingredientName)
            {
                Debug.Log($"Index {i}: We got name {selectedIngredients[i].IngredientName} | We need {ticket.ingredientName}");
                bHasGeneratedIngredientName = true;
            }
        }

        float randThreshold = Random.Range(0.0f, 1.0f);
        if(randThreshold < mChanceToSpawnRequired)
        {
            int nameIndex = -1;
            GlobalVariables.Instance.NameDictionary.TryGetValue(ticket.ingredientName, out var nameSO);
            if (!GlobalVariables.Instance.MetRoundNameRequirement)
            {
                Debug.LogAssertion("WE HAVEN'T MET THE ROUND NAME REQ YET");
                if (!bHasGeneratedIngredientName)
                {
                    nameIndex = Random.Range(0, spotsToSpawnIngredients.Count);
                    selectedIngredients[nameIndex] = nameSO;
                    bHasGeneratedIngredientName = true;
                }
            }
            GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavorSO);
            int flavorIndex = -1;
            if (!GlobalVariables.Instance.MetRoundFlavorRequirement)
            {
                Debug.LogAssertion("WE HAVEN'T MET THE ROUND FLAVOR REQ YET");
                if (!bHasGeneratedFlavorType)
                {
                    for (int i = selectedIngredients.Count - 1; i >= 0; --i)
                    {
                        if (i != nameIndex)
                        {
                            flavorIndex = i;
                            int randomFlavorIndex = Random.Range(0, flavorSO.Count);
                            selectedIngredients[flavorIndex] = flavorSO[randomFlavorIndex];
                            bHasGeneratedFlavorType = true;
                        }
                        break;
                    }
                }
            }
            if (nameIndex != -1)
            {
                bool bAssertHasRightName = selectedIngredients[nameIndex].IngredientName == ticket.ingredientName;
                Debug.Assert(bAssertHasRightName, "WE FAILED TO GENERATE RIGHT NAME, we made " + selectedIngredients[nameIndex].IngredientName + " we need " + ticket.ingredientName);
            }
            if (flavorIndex != -1)
            {
                bool bAssertHasRightName = selectedIngredients[flavorIndex].IngredientName == ticket.ingredientName;
                Debug.Assert(bAssertHasRightName, "WE FAILED TO GENERATE RIGHT NAME, we made " + selectedIngredients[flavorIndex].IngredientName + " we need " + ticket.ingredientName);
            }
        }
        
        
#if UNITY_EDITOR
        Assert.IsTrue(spotsToSpawnIngredients.Count == selectedIngredients.Count, "The number of selected ingredients to spawn does not match the number of spawn locations");
#endif

        for (int i = 0; i < spotsToSpawnIngredients.Count; ++i)
        {
            GameObject newIngredient = Instantiate(mIngredientPrefab);
            HandleNewIngredientSpawned(newIngredient, spotsToSpawnIngredients[i], selectedIngredients[i]);
        }
    }
    private void HandleNewIngredientSpawned(GameObject ingredient, Transform ingredientPosition, IngredientDataSO ingredientData)
    {
        ingredient.GetComponent<Ingredient>().SetIngredientData(ingredientData, ingredientPosition);
        ingredient.transform.position = ingredientPosition.position;

        // We want to remember which transforms has a ingredient or not
        if (mTransformToHasSpawnedIngredient.ContainsKey(ingredientPosition))
        {
            Debug.LogError("Position " + ingredientPosition + " Already has osmething there");
        }
        mTransformToHasSpawnedIngredient[ingredientPosition] = true;
    }

    private void OnEnable()
    {
        Ingredient.OnIngredientClicked += ClickedIngredientsTryAdd;
        IngredientActions.OnMixClicked += AddToMixingBowl;
        IngredientActions.OnCombineClicked += Combine;
        TicketManager.OnTicketGenerated += SpawnIngredients;
        RoundManager.OnRoundCleanup += HandleIngredientCleanup;
    }
    private void OnDestroy()
    {
        Ingredient.OnIngredientClicked -= ClickedIngredientsTryAdd;
        IngredientActions.OnMixClicked -= AddToMixingBowl;
        IngredientActions.OnCombineClicked -= Combine;
        TicketManager.OnTicketGenerated -= SpawnIngredients;
        RoundManager.OnRoundCleanup -= HandleIngredientCleanup;
    }

    private void HandleIngredientCleanup()
    {
        CleanupClickedIngredients();
        CleanupMixingBowl();
        CleanupSpawnedIngredients();
    }
    private void CleanupSpawnedIngredients()
    {
        if (mSpawnedIngredients.Count == 0)
        {
            return;
        }

        for (int i = mSpawnedIngredients.Count - 1; i >= 0; --i)
        {
            OnCleanupIngredients?.Invoke(mSpawnedIngredients[i]);
        }

        mSpawnedIngredients.Clear();
    }
    private void CleanupClickedIngredients()
    {
        for (int i = mClickedIngredients.Count - 1; i >= 0; i--)
        {
            ClickedIngredientsRemove(mClickedIngredients[i]);
        }
        mClickedIngredients.Clear();
        Debug.Log("ClickedIngredients count: " + mClickedIngredients.Count);
    }
    private void CleanupMixingBowl()
    {
        mMixingBowlIngredients.Clear();
        Debug.Log("mMixingBowlIngredients count: " + mMixingBowlIngredients.Count);
    }
}
