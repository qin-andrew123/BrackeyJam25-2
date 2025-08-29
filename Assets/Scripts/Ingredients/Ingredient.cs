using System;
using TMPro;
using UnityEngine;
// TODO: create a function to update the game data so that the mesh can update if we forcibly change the data etc
public class Ingredient : MonoBehaviour
{
    public static event Action<Ingredient> OnIngredientClicked;
    public IngredientTransformPoint TransformPoint { get; set; }
    public IngredientDataSO IngredientData => mIngredientData;
    public IngredientDataSO mIngredientData = null;
    // TODO : DELETE THIS, WE ARE GONNA DELETE THE GO'S ANW
    private bool bIsUsed = false;

    public void IngredientClicked()
    {
        OnIngredientClicked?.Invoke(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!mIngredientData)
        {
            Debug.LogAssertion("ERROR: Ingredient's ingredient data scriptable object is null");
            return;
        }
    }

    public void SetIngredientData(IngredientDataSO ingredientDataSO)
    {
        mIngredientData = ingredientDataSO;
    }

    public void SetIngredientUsed(bool used)
    {
        bIsUsed = used;
    }

    public bool GetIngredientUsed()
    {
        return bIsUsed;
    }
    private void OnEnable()
    {
        IngredientManager.mSpawnedIngredients.Add(gameObject);
    }
}
