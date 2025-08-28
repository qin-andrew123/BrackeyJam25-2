using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlavorPieChart : MonoBehaviour
{
    [SerializeField] private Image[] mChartImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClearValues();
    }
    private void OnEnable()
    {
        RoundManager.OnRoundCleanup += ClearValues;
    }
    private void OnDisable()
    {
        RoundManager.OnRoundCleanup -= ClearValues;
    }
    public void SetValues(FlavorData mChartValues)
    {
        float totalValues = 0.0f;
        float[] values = { mChartValues.BLAND, mChartValues.BITTER, mChartValues.SWEET, mChartValues.SALTY, mChartValues.SOUR, mChartValues.UMAMI };
        for (int i = 0; i < mChartImages.Length - 1; i++)
        {
            float percent = (values[i] / mChartValues.GetTotalSum());
            if (percent >= 1.0f)
            {
                ClearValues();
                mChartImages[i].fillAmount = 1.0f;
                return;

            }

            totalValues += percent;
            mChartImages[i].fillAmount = totalValues;
        }
    }

    public void ClearValues()
    {
        for (int i = 0; i < mChartImages.Length; i++)
        {
            mChartImages[i].fillAmount = 0.0f;
        }
    }
}
