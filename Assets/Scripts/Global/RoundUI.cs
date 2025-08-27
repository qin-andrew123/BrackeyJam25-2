using System.Linq;
using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mTMP;
    [SerializeField] private TextMeshProUGUI mScoreText;
    private void OnEnable()
    {
        TicketManager.OnTicketGenerated += UpdateText;
    }
    private void OnDisable()
    {
        TicketManager.OnTicketGenerated -= UpdateText;
    }

    private void UpdateText(TicketConstraint ticketConstraint)
    {
        string ingredientFlavorConstraint = ticketConstraint.ingredientFlavor.ToString();
        ingredientFlavorConstraint = char.ToUpper(ingredientFlavorConstraint[0]) + ingredientFlavorConstraint.Substring(1).ToLower();

        string ingredientNameConstraint = ticketConstraint.ingredientName;
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

        mTMP.text = $"Order #{ticketConstraint.orderNumber}\n Biscuit Flavor: {ingredientFlavorConstraint} \n Ingredients Needed: {ingredientNameConstraint}";
    }
}
