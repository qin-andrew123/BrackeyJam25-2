using TMPro;
using UnityEngine;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] private GameObject mPopupImage;
    [SerializeField] private TextMeshProUGUI mIngredientName;

    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Sweet;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Bitter;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Bland;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Umami;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Sour;
    [SerializeField] private TextMeshProUGUI mIngredientFlavor_Salty;

    [SerializeField] private TextMeshProUGUI mIngredientEffect;
    [SerializeField] private TextMeshProUGUI mIngredientValue;

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
        if(inputIngredient == null)
        {
            return;
        }

        mPopupImage.gameObject.SetActive(true);

        string ingredientName = inputIngredient.IngredientName;
        //string ingredientFlavor = inputIngredient.IngredientFlavorValue.ToString();
        //string ingredientEffect = inputIngredient.IngredientEffectValue.ToString();
        string ingredientValue = inputIngredient.IngredientValue.ToString();

        mIngredientName.text = ingredientName;

        mIngredientFlavor_Sweet.text = "Sweet: " + inputIngredient.mFlavorData.SWEET.ToString();
        mIngredientFlavor_Bitter.text = "Bitter: " + inputIngredient.mFlavorData.BITTER.ToString();
        mIngredientFlavor_Bland.text = "Bland: " + inputIngredient.mFlavorData.BLAND.ToString();
        mIngredientFlavor_Umami.text = "Umami: " + inputIngredient.mFlavorData.UMAMI.ToString();
        mIngredientFlavor_Sour.text = "Sour: " + inputIngredient.mFlavorData.SOUR.ToString();
        mIngredientFlavor_Salty.text = "Salty: " + inputIngredient.mFlavorData.SALTY.ToString();

        //mIngredientEffect.text = ingredientEffect;
        mIngredientValue.text = ingredientValue;
        

    }

    public void HidePopup()
    {
        mPopupImage.gameObject.SetActive(false);
    }

}
