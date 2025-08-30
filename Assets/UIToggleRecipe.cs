using UnityEngine;
using DG.Tweening;

public class UIToggleRecipe : MonoBehaviour
{
    [SerializeField] private RectTransform RecipePanel;
    bool isOpen = false;
    float _nextTimeToToggle = 0f;
    float _toggleCooldown = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleRecipePanel()
    {
        if (Time.time > _nextTimeToToggle)
        {
            if (!isOpen )
            {
                RecipePanel.DOAnchorPosX(-45,1f);
                isOpen = true;
                
            }
            else
            {
                RecipePanel.DOAnchorPosX(-350,1f);
                isOpen = false;
            }
            _nextTimeToToggle = Time.time + _toggleCooldown;
            
        }
    }
}
