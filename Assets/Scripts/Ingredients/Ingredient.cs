using System;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientManager mIngredientManager;
    [SerializeField] private IngredientDataSO mIngredientData;
    public static event Action<IngredientDataSO> OnIngredientClicked;
    private bool bIsUsed = false;

    public void IngredientClicked()
    {
        OnIngredientClicked?.Invoke(mIngredientData);

        mIngredientManager.AddClickedIngredient(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!mIngredientData)
        {
            Debug.LogAssertion("ERROR: Ingredient's ingredient data scriptable object is null");
            return;
        }

        if(!mIngredientManager)
        {
            IngredientManager[] components = UnityEngine.Object.FindObjectsByType<IngredientManager>(FindObjectsSortMode.None);
            if (components.Length > 0)
            {
                mIngredientManager = components[0];
            }
        }
    }

    public IngredientDataSO GetIngredientData()
    {
        return mIngredientData;
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
}
