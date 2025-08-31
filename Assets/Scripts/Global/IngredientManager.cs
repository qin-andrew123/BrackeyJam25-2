using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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
    public static event Action<IngredientDataSO> OnIngredientCombined;
    public static event Action<bool> OnIngredientCombinedStatus;
    public static event Action<Ingredient> OnSpawnIngredient;
    public static List<GameObject> mSpawnedIngredients = new List<GameObject>();

    public bool bIsCombining = false;
    [SerializeField] private List<Ingredient> mMixingBowlIngredients = new List<Ingredient>();
    [SerializeField] private List<Ingredient> mClickedIngredients = new List<Ingredient>();
    [SerializeField] private Transform mMixingBowlLocation;
    [SerializeField] private GameObject mIngredientPrefab;
    [SerializeField] private List<IngredientPair> mCombinableIngredients;
    [SerializeField] private IngredientUI mIngredientUI;
    [SerializeField] private RoundManager mRoundManager;
    [SerializeField] private float mChanceToSpawnRequired = 0.7f;
    [SerializeField] private List<IngredientTransformPoint> mTransformPoints;
    private List<Ingredient> mCurrentlyActiveBaseIngredients = new List<Ingredient>();
    private List<Ingredient> mCurrentlyActiveFlavorIngredients = new List<Ingredient>();

    private Animation_Manager playerAniamtion;

    private void Start()
    {
        playerAniamtion = GameObject.Find("Player").GetComponent<Animation_Manager>();
    }

    // TODO : Refactor Spawn Ingredients
    public void SpawnIngredients()
    {
        CleanupSpawnedIngredients();

        GenerateIngredients(true);
    }

    public void AddToMixingBowl()
    {
        if (mClickedIngredients.Count != 1)
        {
            Debug.LogWarning("Error: You can only mix one ingredient at a time!");
            CleanupClickedIngredients();
            playerAniamtion.playDefeat2();
            return;
        }
        Ingredient ingredient = mClickedIngredients[0];

        // TODO : We probably do not require this since we will delete all the ingredients on mix anyways
        if (ingredient.GetIngredientUsed())
        {
            Debug.LogWarning("Cannot mix. Ingredient already in mixing bowl");
            CleanupClickedIngredients();
            playerAniamtion.playDefeat2();
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

        foreach (IngredientPair pair in mCombinableIngredients)
        {
            if (pair.resultPrefab == ingredient.IngredientData)
            {
                foreach (IngredientDataSO ingredientSO in pair.ingredientPrefabs)
                {
                    OnIngredientCombined?.Invoke(ingredientSO);
                }
                break;
            }
        }
        CleanupIngredient(ingredient.gameObject);

        GenerateIngredients(false);

        // TODO: Pass in the value of the ingredient mixed in so that we can ensure that we are getting the required values.
        OnIngredientMixed?.Invoke(ingredient.IngredientData);
        playerAniamtion.playKnead();
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
    private bool ClickedIngredientsRemove(Ingredient ingredient)
    {
        if (!ingredient)
        {
            return false;
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

        return true;
    }

    public void Combine()
    {
        Debug.Log("Trying to Combine Ingredients");

        if (mClickedIngredients.Count < 2)
        {
            Debug.LogWarning("Cannot enter combine mode without selecting at least two ingredients!");
            OnIngredientCombinedStatus?.Invoke(false);
            return;
        }

        foreach (IngredientPair pair in mCombinableIngredients)
        {
            bool bCanCombine = true;
            List<IngredientDataSO> ingredients = pair.ingredientPrefabs;
            foreach (Ingredient ingredient in mClickedIngredients)
            {
                bool bDoesContainIngredient = ingredients.Contains(ingredient.IngredientData);
                if (!bDoesContainIngredient)
                {
                    bCanCombine = false;
                    break;
                }
            }
            if (bCanCombine)
            {
                OnIngredientCombinedStatus?.Invoke(true);
                Debug.Log("Combining");
                Transform resultPos = mClickedIngredients[0].IngredientData.InitialSpawnTransform;
                for (int i = mClickedIngredients.Count - 1; i >= 0; --i)
                {
                    // POST MORTEM: WE SHOULD NOT BE MAKING A COPY HERE PROB LMFAO
                    //IngredientDataSO sendToListeners = ScriptableObject.Instantiate(mClickedIngredients[i].IngredientData);
                    //OnIngredientCombined?.Invoke(sendToListeners);
                    CleanupIngredient(mClickedIngredients[i].gameObject);
                }
                mClickedIngredients.Clear();

                List<int> freedIndices = GetFreedIndices();
                // instantiate new prefab and delete previous ingredients
                SpawnIngredient(pair.resultPrefab, freedIndices[0]);

                GenerateIngredients(false);

                playerAniamtion.playVictory2();

                return;
            }
            
        }
        OnIngredientCombinedStatus?.Invoke(false);
        Debug.Log("Ingredients Not Combinable. Player did not select correct ones!");
        CleanupClickedIngredients();

    }
    private void GenerateIngredients(bool bIsInitialSpawning)
    {
        List<int> freedIndices = GetFreedIndices();

        freedIndices = freedIndices.OrderBy(x => Random.value).ToList();

        int numNewToSpawn = freedIndices.Count;
        bool bHasGeneratedFlavorType = false;
        bool bHasGeneratedIngredientName = false;
        TicketConstraint ticket = GlobalVariables.Instance.CurrentTicketConstraint;

        List<IngredientDataSO> ingredientsToSpawn = new List<IngredientDataSO>();
        if (bIsInitialSpawning)
        {
            List<IngredientDataSO> flavorIngredients = new List<IngredientDataSO>(GlobalVariables.Instance.FlavorIngredients);
            flavorIngredients = flavorIngredients.OrderBy(x => Random.value).ToList();
            List<IngredientDataSO> selectedBunch = flavorIngredients.Take(3).ToList();

            ingredientsToSpawn.AddRange(GlobalVariables.Instance.BaseIngredients);
            ingredientsToSpawn.AddRange(selectedBunch);
        }
        else
        {
            for (int i = 0; i < numNewToSpawn; ++i)
            {
                int randomIndex = 0;
                if (Random.Range(0, 1f) < 0.5f) // TODO : Adjust in case too rare
                {
                    List<IngredientDataSO> potentialToAdd = new List<IngredientDataSO>(GlobalVariables.Instance.BaseIngredients);
                    foreach (Ingredient ing in mCurrentlyActiveBaseIngredients)
                    {
                        if (potentialToAdd.Contains(ing.IngredientData))
                        {
                            potentialToAdd.Remove(ing.IngredientData);
                        }
                    }
                    if (potentialToAdd.Count > 0)
                    {
                        randomIndex = Random.Range(0, potentialToAdd.Count);
                        ingredientsToSpawn.Add(potentialToAdd[randomIndex]);
                    }
                    else
                    {
                        randomIndex = Random.Range(0, GlobalVariables.Instance.BaseIngredients.Count);
                        ingredientsToSpawn.Add(GlobalVariables.Instance.BaseIngredients[randomIndex]);
                    }
                }
                else
                {
                    List<IngredientDataSO> potentialToAdd = new List<IngredientDataSO>(GlobalVariables.Instance.FlavorIngredients);
                    foreach (Ingredient ing in mCurrentlyActiveFlavorIngredients)
                    {
                        if (potentialToAdd.Contains(ing.IngredientData))
                        {
                            potentialToAdd.Remove(ing.IngredientData);
                        }
                    }
                    if (potentialToAdd.Count > 0)
                    {
                        randomIndex = Random.Range(0, potentialToAdd.Count);
                        ingredientsToSpawn.Add(potentialToAdd[randomIndex]);
                    }
                    else
                    {
                        randomIndex = Random.Range(0, GlobalVariables.Instance.FlavorIngredients.Count);
                        ingredientsToSpawn.Add(GlobalVariables.Instance.FlavorIngredients[randomIndex]);
                    }
                }
            }
        }

        // Check to ensure that we have the required flavor and value
        for (int i = 0; i < ingredientsToSpawn.Count; ++i)
        {
            if (ingredientsToSpawn[i].IngredientFlavorValue == ticket.ingredientFlavor)
            {
                Debug.Log($"Index {i}: We got flavor {ingredientsToSpawn[i].IngredientFlavorValue} | We need {ticket.ingredientFlavor}");
                bHasGeneratedFlavorType = true;
            }
            if (ingredientsToSpawn[i].IngredientName == ticket.ingredientName)
            {
                Debug.Log($"Index {i}: We got name {ingredientsToSpawn[i].IngredientName} | We need {ticket.ingredientName}");
                bHasGeneratedIngredientName = true;
            }
        }

        float randThreshold = Random.Range(0.0f, 1.0f);
        if (randThreshold < mChanceToSpawnRequired)
        {
            int nameIndex = -1;
            GlobalVariables.Instance.NameDictionary.TryGetValue(ticket.ingredientName, out var nameSO);
            if (!GlobalVariables.Instance.MetRoundNameRequirement)
            {
                if (!bHasGeneratedIngredientName)
                {
                    nameIndex = Random.Range(0, numNewToSpawn);
                    ingredientsToSpawn[nameIndex] = nameSO;
                    bHasGeneratedIngredientName = true;
                }
            }
            GlobalVariables.Instance.FlavorDictionary.TryGetValue(ticket.ingredientFlavor, out var flavorSO);
            if (flavorSO == null)
            {
#if UNITY_EDITOR
                Assert.IsTrue(flavorSO != null, $"{ticket.ingredientFlavor}: Does not have any ingredients tied to its flavor dictionary, did we forget to add in any ingredients that match this flavor profile?");
#endif
                return;
            }

            int flavorIndex = -1;
            if (!GlobalVariables.Instance.MetRoundFlavorRequirement)
            {
                Debug.Log("Ingredient Flavor: " + ticket.ingredientFlavor + " Generating FlavorSO: " + flavorSO.Count);
                if (!bHasGeneratedFlavorType)
                {
                    for (int i = ingredientsToSpawn.Count - 1; i >= 0; --i)
                    {
                        if (i != nameIndex)
                        {
                            flavorIndex = i;
                            int randomFlavorIndex = Random.Range(0, flavorSO.Count);
                            ingredientsToSpawn[flavorIndex] = flavorSO[randomFlavorIndex];
                            bHasGeneratedFlavorType = true;
                            break;
                        }
                    }
                }
            }
            if (nameIndex != -1)
            {
                bool bAssertHasRightName = ingredientsToSpawn[nameIndex].IngredientName == ticket.ingredientName;
                Debug.Assert(bAssertHasRightName, "WE FAILED TO GENERATE RIGHT NAME, we made " + ingredientsToSpawn[nameIndex].IngredientName + " we need " + ticket.ingredientName);
            }
            if (flavorIndex != -1)
            {
                bool bAssertHasRightName = ingredientsToSpawn[flavorIndex].IngredientName == ticket.ingredientName;
                Debug.Assert(bAssertHasRightName, "WE FAILED TO GENERATE RIGHT NAME, we made " + ingredientsToSpawn[flavorIndex].IngredientName + " we need " + ticket.ingredientName);
            }
        }

#if UNITY_EDITOR
        Assert.IsTrue(freedIndices.Count == ingredientsToSpawn.Count, $"We have more ingredients to spawn than we do spawn points");
#endif
        for (int i = 0; i < ingredientsToSpawn.Count; i++)
        {
            SpawnIngredient(ingredientsToSpawn[i], freedIndices[i]);
        }
    }
    private void SpawnIngredient(IngredientDataSO ingredientToSpawn, int freedIndex)
    {
        GameObject newIngredient = Instantiate(ingredientToSpawn.PrefabObject);
        Ingredient ingredient = newIngredient.GetComponent<Ingredient>();
        if (ingredient == null)
        {
            Debug.Log("null ingredient when generating table ingredients");
        }
        ingredient.SetIngredientData(ingredientToSpawn);
        mTransformPoints[freedIndex].InitializePoint(ingredient);
        if (GlobalVariables.Instance.BaseIngredients.Contains(ingredientToSpawn))
        {
            mCurrentlyActiveBaseIngredients.Add(ingredient);
        }
        else if (GlobalVariables.Instance.FlavorIngredients.Contains(ingredientToSpawn))
        {
            mCurrentlyActiveFlavorIngredients.Add(ingredient);
        }
    }
    private List<int> GetFreedIndices()
    {
        List<int> freedIndices = new List<int>();

        for (int i = 0; i < mTransformPoints.Count; ++i)
        {
            if (mTransformPoints[i].IsAvailable)
            {
                freedIndices.Add(i);
            }
        }

        return freedIndices;
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
            CleanupIngredient(mSpawnedIngredients[i]);
        }

        mSpawnedIngredients.Clear();
    }
    private void CleanupIngredient(GameObject go)
    {
        if (!go)
        {
            return;
        }

        Ingredient ingredient = go.GetComponent<Ingredient>();

        if (!ingredient)
        {
            return;
        }

        ingredient.TransformPoint.ClearPoint();
        if (GlobalVariables.Instance.BaseIngredients.Contains(ingredient.IngredientData))
        {
            mCurrentlyActiveBaseIngredients.Remove(ingredient);
        }
        else if (GlobalVariables.Instance.FlavorIngredients.Contains(ingredient.IngredientData))
        {
            mCurrentlyActiveFlavorIngredients.Remove(ingredient);
        }

        Destroy(go);
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
