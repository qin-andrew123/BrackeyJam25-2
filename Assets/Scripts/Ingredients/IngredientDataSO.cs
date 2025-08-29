using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum IngredientFlavor
{
    NONE = -1,
    BLAND = 0,
    BITTER,
    SWEET,
    SALTY,
    SOUR,
    UMAMI,

    // KEEP THIS AT THE END. If we ever add more flavors, we need to update this value
    TOTAL_FLAVORS = 6,
}

[System.Serializable]
public class FlavorData
{
    public float BLAND = 0;
    public float BITTER = 0;
    public float SWEET = 0;
    public float SALTY = 0;
    public float SOUR = 0;
    public float UMAMI = 0;

    public static FlavorData operator +(FlavorData a, FlavorData b)
    {
        FlavorData c = new FlavorData();
        c.BLAND = Mathf.Max(0, a.BLAND + b.BLAND);
        c.BITTER = Mathf.Max(0, a.BITTER + b.BITTER);
        c.SWEET = Mathf.Max(0, a.SWEET + b.SWEET);
        c.SALTY = Mathf.Max(0, a.SALTY + b.SALTY);
        c.SOUR = Mathf.Max(0, a.SOUR + b.SOUR);
        c.UMAMI = Mathf.Max(0, a.UMAMI + b.UMAMI);

        return c;
    }

    public float GetTotalSum()
    {
        return (BLAND + BITTER + SWEET + SALTY + SOUR + UMAMI);
    }

    public float GetPercentage(float flavor)
    {
        return (flavor / GetTotalSum());
    }
}


[CreateAssetMenu(fileName = "IngredientDataSO", menuName = "Scriptable Objects/IngredientDataSO")]
public class IngredientDataSO : ScriptableObject
{
    // Returns antagonizing flavor
    public static int GetAntagonizingIngredientFlavor(IngredientFlavor inputFlavor)
    {
        if (inputFlavor == IngredientFlavor.NONE)
        {
            return -1;
        }

        int offsetValue = (int)(IngredientFlavor.TOTAL_FLAVORS) / 2;
        return ((int)(inputFlavor) + offsetValue) % (int)(IngredientFlavor.TOTAL_FLAVORS);
    }

    // Returns the two synergistic ingredient flavors
    public static int[] GetSynergisticIngredientFlavors(IngredientFlavor inputFlavor)
    {
        int[] result = { -1, -1 };

        if (inputFlavor == IngredientFlavor.NONE)
        {
            return result;
        }
        int thisIndex = (int)(inputFlavor);

        int leftIndex = thisIndex - 1;
        if (leftIndex < 0)
        {
            // Could technically set this to UMAMI, but in case we want to add more
            leftIndex = (int)(IngredientFlavor.TOTAL_FLAVORS) - 1;
        }
        result[0] = leftIndex;

        int rightIndex = thisIndex + 1;
        if (rightIndex >= (int)(IngredientFlavor.TOTAL_FLAVORS))
        {
            rightIndex = 0;
        }
        result[1] = rightIndex;

        return result;
    }

    [SerializeField] private string mIngredientName = "NONE";
    public string IngredientName => mIngredientName;
    [SerializeField] private IngredientFlavor mIngredientFlavor = IngredientFlavor.NONE;
    public IngredientFlavor IngredientFlavorValue => mIngredientFlavor;
    [SerializeField] private float mIngredientValue = 1.0f;
    public float IngredientValue => mIngredientValue;

    [SerializeField] public FlavorData mFlavorData = new FlavorData();
    public Material IngredientMaterial => mIngredientMaterial;
    [SerializeField] private Material mIngredientMaterial;
    public Transform InitialSpawnTransform { get; set; }
    public Mesh IngredientMesh => mIngredientMesh;
    [SerializeField] private Mesh mIngredientMesh;
    public GameObject PrefabObject => mPrefabObject;
    [SerializeField] private GameObject mPrefabObject;
}
