using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePrefabUI : MonoBehaviour
{
    [SerializeField] private float mAnimationDuration = 1.0f;
    private Slider mSlider;
    private TextMeshProUGUI mTextMeshPro;
    private float mSliderValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mSlider = gameObject.GetComponentInChildren<Slider>();
        mSlider.value = 0;
        mTextMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeSliderValue(float sliderValue, int biscuitNum)
    {
        mSliderValue = sliderValue;

        if (mSlider == null)
        {
            mSlider = gameObject.GetComponentInChildren<Slider>();
            mSlider.value = 0;
        }

        if (mTextMeshPro == null)
        {
            mTextMeshPro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            mTextMeshPro.text = $"Biscuit #{biscuitNum}";
        }
        StartCoroutine(AnimateSlider());
    }
    private IEnumerator AnimateSlider()
    {
        yield return new WaitForSeconds(1.0f);

        float startValue = 0f;
        float elapsed = 0f;

        while (elapsed < mAnimationDuration)
        {
            elapsed += Time.deltaTime;
            mSlider.value = Mathf.Lerp(startValue, mSliderValue, elapsed / mAnimationDuration);
            yield return null;
        }

        mSlider.value = mSliderValue; // Make sure it finishes exactly
    }
}
