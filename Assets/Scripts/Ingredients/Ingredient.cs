using System;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientDataSO mIngredientData;
    public static event Action<IngredientDataSO> OnIngredientClicked;
    public void IngredientClicked()
    {
        OnIngredientClicked?.Invoke(mIngredientData);
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
}
