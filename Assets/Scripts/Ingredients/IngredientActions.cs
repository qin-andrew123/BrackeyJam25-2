using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientActions : MonoBehaviour
{
    public static event Action OnMixClicked;
    public static event Action OnCombineClicked;
    [SerializeField] private IngredientManager mIngredientManager;

    public void CheckCombine()
    {
        OnCombineClicked?.Invoke();
    }

    public void Mix()
    {
        OnMixClicked?.Invoke();
    }
}
