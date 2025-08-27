using System.Linq;
using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mTMP;
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

        mTMP.text = $"Order #{ticketConstraint.orderNumber}\n Biscuit Flavor: {ingredientFlavorConstraint} \n Ingredients Needed: {ingredientNameConstraint}";
    }
}
