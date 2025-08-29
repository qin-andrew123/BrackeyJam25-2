using System;
using TMPro;
using UnityEngine;
// TODO: create a function to update the game data so that the mesh can update if we forcibly change the data etc
public class Ingredient : MonoBehaviour
{
    public static event Action<Ingredient> OnIngredientClicked;
    public IngredientTransformPoint TransformPoint { get; set; }
    public IngredientDataSO IngredientData => mIngredientData;
    [SerializeField] private IngredientDataSO mIngredientData;
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

    public void SetIngredientData(IngredientDataSO ingredientDataSO, Transform startingSpawnLocation)
    {
        mIngredientData = ingredientDataSO;
        mIngredientData.InitialSpawnTransform = startingSpawnLocation;
        // This is temp but for debugging purposes. 
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = ingredientDataSO.IngredientMaterial;
            Bounds worldBounds = renderer.bounds;
        }
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = ingredientDataSO.IngredientMesh;
            Bounds meshBounds = meshFilter.sharedMesh.bounds;
            BoxCollider box = gameObject.GetComponent<BoxCollider>();
            box.center = meshBounds.center;
            box.size = meshBounds.size + Vector3.one * 0.001f;
        }
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
