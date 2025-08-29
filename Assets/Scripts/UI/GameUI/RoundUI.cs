using System.Linq;
using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mTMP;
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mTurnsLeft;
    [SerializeField] private TextMeshProUGUI mBiscuitsLeft;
    [SerializeField] private TextMeshProUGUI mQuota;

    private void Start()
    {
    }
    private void OnEnable()
    {
        //TicketManager.OnSelectedIngredients += UpdateText;
        TicketManager.OnTicketGenerated += UpdateTicketText;
        RoundManager.OnTurnUsed += UpdateTurnsText;
        RoundManager.OnRoundStart += UpdateBiscuitsText;
        GlobalVariables.OnQuotaChange += UpdateQuotaText;
    }
    private void OnDisable()
    {
        //TicketManager.OnSelectedIngredients -= UpdateText;
        TicketManager.OnTicketGenerated -= UpdateTicketText;
        RoundManager.OnTurnUsed -= UpdateTurnsText;
        RoundManager.OnRoundStart -= UpdateBiscuitsText;
        GlobalVariables.OnQuotaChange -= UpdateQuotaText;

    }

    private void UpdateTicketText()
    {
        TicketConstraint ticketConstraint = GlobalVariables.Instance.CurrentTicketConstraint;
        string ingredientFlavorConstraint = ticketConstraint.ingredientFlavor.ToString();
        ingredientFlavorConstraint = char.ToUpper(ingredientFlavorConstraint[0]) + ingredientFlavorConstraint.Substring(1).ToLower();

        string ingredientNameConstraint = ticketConstraint.ingredientName;
        string ingredientTwo = ingredientFlavorConstraint[1].ToString();
        ingredientTwo = char.ToUpper(ingredientTwo[0]) + ingredientTwo.Substring(1).ToLower();

        //mTMP.text = $"You Require: {ingredientFlavorConstraint} AND {ingredientTwo}";
        mTMP.text = $"Order #{ticketConstraint.orderNumber}\n Biscuit Flavor: {ingredientFlavorConstraint} \n Ingredients Needed: {ingredientNameConstraint}";

    }

    private void UpdateQuotaText()
    {
        mQuota.text = $"Today's Quota: {GlobalVariables.Instance.QuotaNumber}";
    }
    public void UpdateScoreText(float newValue)
    {
        mScoreText.text = newValue.ToString();
    }

    public void UpdateTurnsText(int turnsLeft)
    {
        mTurnsLeft.text = $"Moves Left: {turnsLeft}";
    }
    public void UpdateBiscuitsText(int biscuitsMade)
    {
        mBiscuitsLeft.text = $"{biscuitsMade} / {GlobalVariables.Instance.RoundsPerLevel} Biscuits Made";
    }
}
