using System;
using UnityEngine;
using DG.Tweening;
public class Buttons : MonoBehaviour
{
    [SerializeField] private IngredientActions mIngredientActions;
    
    //yep this is messy, but just to make it work only with combine bbutton
    [SerializeField] private bool isCombineButton = false;
    
    private RectTransform UIbutton;
    private bool isShaking = false;
    
    
   
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIbutton = GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        IngredientManager.OnIngredientCombinedStatus += OnCombinedIngredientStatus;
    }

    private void OnDisable()
    {
        IngredientManager.OnIngredientCombinedStatus -= OnCombinedIngredientStatus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mix()
    {
        mIngredientActions.Mix();
    }

    public void Combine()
    {
        mIngredientActions.CheckCombine();
    }

    private void OnCombinedIngredientStatus(bool obj)
    {
        if (isCombineButton && !isShaking)
        {
            isShaking = true;
            if (obj)
            {
                //do something good
                UIbutton.DOPunchScale(new Vector3(0.005f,0.005f,0), 1f, 1,1)
                    .OnComplete( () => { isShaking = false; } );
            }
            else
            {
                UIbutton.DOShakeAnchorPos(1f, new Vector2(5,5), 10, 90, false, true)
                    .OnComplete( () => { isShaking = false; } );
            }
            
        }
    }
}
