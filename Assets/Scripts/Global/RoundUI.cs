using System.Linq;
using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mTMP;
    [SerializeField] private TextMeshProUGUI mScoreText;
    private void OnEnable()
    {
        RoundManager.OnSelectedIngredients += UpdateText;
    }
    private void OnDisable()
    {
        RoundManager.OnSelectedIngredients -= UpdateText;
    }

    private void UpdateText(IngredientFlavor[] ingredientFlavors)
    {
        if (ingredientFlavors.Count() != 2)
        {
            Debug.LogAssertion("We are passing more than 2 requried ingredient flavors to the UI");
            mTMP.text = "UpdateText() Function failed";
            return;
        }
        string ingredientOne = ingredientFlavors[0].ToString();
        ingredientOne = char.ToUpper(ingredientOne[0]) + ingredientOne.Substring(1).ToLower();

        string ingredientTwo = ingredientFlavors[1].ToString();
        ingredientTwo = char.ToUpper(ingredientTwo[0]) + ingredientTwo.Substring(1).ToLower();

        mTMP.text = $"You Require: {ingredientOne} AND {ingredientTwo}";
    }

    public void UpdateScoreText(float newValue)
    {
        mScoreText.text = newValue.ToString();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
