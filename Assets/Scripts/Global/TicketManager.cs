using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
public struct TicketConstraint
{
    public string ingredientName;
    public IngredientFlavor ingredientFlavor;
    public int orderNumber;
}

public class TicketManager : MonoBehaviour
{
    public static event Action<TicketConstraint> OnTicketGenerated;

    public TicketConstraint CurrentTicketConstraint => mTicketConstraint;
    private TicketConstraint mTicketConstraint;


    private void Start()
    {
    }
    private void OnEnable()
    {
        PlayerInput.TEMPSetRoundRequirements += GenerateTicket;
        RoundManager.OnRoundStart += GenerateTicket;
    }
    private void OnDisable()
    {
        PlayerInput.TEMPSetRoundRequirements -= GenerateTicket;
        RoundManager.OnRoundStart -= GenerateTicket;
    }
    private void GenerateTicket(int roundNumber)
    {
        List<string> ingredientNames = GlobalVariables.Instance.IngredientNames;
        // TODO : Can add weighting later
        mTicketConstraint.ingredientFlavor = (IngredientFlavor)(UnityEngine.Random.Range((int)IngredientFlavor.BLAND, (int)IngredientFlavor.TOTAL_FLAVORS - 1));
        mTicketConstraint.ingredientName = ingredientNames[UnityEngine.Random.Range(0, ingredientNames.Count - 1)];
        mTicketConstraint.orderNumber = roundNumber;
        OnTicketGenerated?.Invoke(mTicketConstraint);
    }
}