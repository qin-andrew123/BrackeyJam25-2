using System;
using TMPro;
using UnityEngine;
// TODO: create a function to update the game data so that the mesh can update if we forcibly change the data etc
public class Ingredient : MonoBehaviour
{
    [SerializeField] private IngredientManager mIngredientManager;
    [SerializeField] private IngredientDataSO mIngredientData;
    [SerializeField] private GameObject mMesh;
    [SerializeField] private TextMeshPro mTextMeshPro;
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

        if (!mIngredientManager)
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

    public GameObject GetMesh()
    {
        return mMesh;
    }

    public void SetIngredientData(IngredientDataSO ingredientDataSO)
    {
        mIngredientData = ingredientDataSO;
        // This is temp but for debugging purposes. 
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = ingredientDataSO.IngredientMaterial;
        }
        mTextMeshPro.text = ingredientDataSO.IngredientName;
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
