using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
public class TicketConstraint
{
    public string ingredientName;
    public IngredientFlavor ingredientFlavor;
    public int orderNumber;
}

public class TicketManager : MonoBehaviour
{
    public static event Action<TicketConstraint> OnTicketGenerated;

    private void Start()
    {
    }
    private void OnEnable()
    {
        RoundManager.OnRoundStart += GenerateTicket;
    }
    private void OnDisable()
    {
        RoundManager.OnRoundStart -= GenerateTicket;
    }
    private void GenerateTicket(int roundNumber)
    {
        List<string> ingredientNames = GlobalVariables.Instance.IngredientNames;
        // TODO : Can add weighting later
        GlobalVariables.Instance.CurrentTicketConstraint.ingredientFlavor = (IngredientFlavor)(UnityEngine.Random.Range((int)IngredientFlavor.BLAND, (int)IngredientFlavor.TOTAL_FLAVORS - 1));
        GlobalVariables.Instance.CurrentTicketConstraint.ingredientName = ingredientNames[UnityEngine.Random.Range(0, ingredientNames.Count - 1)];
        GlobalVariables.Instance.CurrentTicketConstraint.orderNumber = roundNumber;
        OnTicketGenerated?.Invoke(GlobalVariables.Instance.CurrentTicketConstraint);
    }
}