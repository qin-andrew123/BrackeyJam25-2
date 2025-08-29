using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInput : MonoBehaviour
{
    // TODO: either completely delete this event or move it to a place that will use it in the future
    public static event Action<int> TEMPSetRoundRequirements;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        MouseInputCalculation();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TEMPSetRoundRequirements?.Invoke(5);
        }
#endif
    }

    private void MouseInputCalculation()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mClickablesMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                // Check for a custom component
                Ingredient ingredient = hit.collider.GetComponent<Ingredient>();

                // TODO : We should delete the ingredient and just let the bowl symbolically show that this is added in.
                if (ingredient != null)
                {
                    ingredient.IngredientClicked();
                }
            }
            // TODO: Add in highlighting
        }
    }

    private Camera mainCamera;
    [SerializeField] private LayerMask mClickablesMask;
}
