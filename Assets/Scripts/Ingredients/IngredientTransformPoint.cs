using UnityEngine;

public class IngredientTransformPoint : MonoBehaviour
{
    public Ingredient IngredientReference { get; set; }
    public bool IsAvailable => IngredientReference == null;
    public void InitializePoint(Ingredient ingredient)
    {
        if (IngredientReference != null)
        {
            return;
        }

        IngredientReference = ingredient;
        IngredientReference.TransformPoint = this;
        ingredient.transform.position = transform.position;
    }

    public void ClearPoint()
    {
        IngredientReference = null;
    }
}
