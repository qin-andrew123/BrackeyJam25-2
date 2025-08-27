using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] private IngredientActions mIngredientActions;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mix()
    {
        mIngredientActions.Mix();
    }

    public void Combine()
    {
        mIngredientActions.CheckCombine();
    }
}
