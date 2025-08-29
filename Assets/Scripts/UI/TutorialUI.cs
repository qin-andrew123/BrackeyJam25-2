using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private List<Image> mTutorial;
    private int mTutorialIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowNextTutorial()
    {
        mTutorial[mTutorialIndex].gameObject.SetActive(false);

        mTutorialIndex++;
        mTutorialIndex = (mTutorialIndex % mTutorial.Count);
        mTutorial[mTutorialIndex].gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OpenTutorial()
    {
        mTutorialIndex = 0;
        mTutorial[mTutorialIndex].enabled = true;
    }
}
