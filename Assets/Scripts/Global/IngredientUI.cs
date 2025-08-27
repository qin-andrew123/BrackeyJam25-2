using TMPro;
using UnityEngine;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] private GameObject mPopupImage;
    [SerializeField] private TextMeshProUGUI mIngredientName;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor;
    [SerializeField] private TextMeshProUGUI mIngredientEffect;
    [SerializeField] private TextMeshProUGUI mIngredientValue;
    private bool bShouldReveal = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePopup(IngredientDataSO inputIngredient)
    {
        mPopupImage.gameObject.SetActive(true);

        string ingredientName = inputIngredient.IngredientName;
        string ingredientFlavor = inputIngredient.IngredientFlavorValue.ToString();
        //string ingredientEffect = inputIngredient.IngredientEffectValue.ToString();
        string ingredientValue = inputIngredient.IngredientValue.ToString();

        mIngredientName.text = ingredientName;
        mIngredientFlavor.text = ingredientFlavor;
        //mIngredientEffect.text = ingredientEffect;
        mIngredientValue.text = ingredientValue;
        

    }

    public void HidePopup()
    {
        mPopupImage.gameObject.SetActive(false);
    }

}
