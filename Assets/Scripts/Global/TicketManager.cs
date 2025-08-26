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
    public static event Action<TicketConstraint> OnSelectedIngredients;

    public TicketConstraint CurrentTicketConstraint => mTicketConstraint;
    // TODO: Move this to a singleton, and we access it from there
    [SerializeField] private List<IngredientDataSO> mIngredientDataSO;
    // TODO: Move this to a singleton
    private List<string> mIngredientDataNames = new List<string>();
    private TicketConstraint mTicketConstraint;


    private void Start()
    {
        // TODO: move the generation of string names to the singleton
        if (mIngredientDataSO.Count == 0)
        {
            Debug.LogError("ERROR: The list of ingredient data scriptable objects in TicketManager is empty");
            return;
        }

        foreach (IngredientDataSO ingredientData in mIngredientDataSO)
        {
            mIngredientDataNames.Add(ingredientData.name);
        }
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
        // TODO : Can add weighting later
        mTicketConstraint.ingredientFlavor = (IngredientFlavor)(UnityEngine.Random.Range((int)IngredientFlavor.BLAND, (int)IngredientFlavor.TOTAL_FLAVORS - 1));
        mTicketConstraint.ingredientName = mIngredientDataNames[UnityEngine.Random.Range(0, mIngredientDataNames.Count - 1)];
        mTicketConstraint.orderNumber = roundNumber;
        OnSelectedIngredients?.Invoke(mTicketConstraint);
    }
}
