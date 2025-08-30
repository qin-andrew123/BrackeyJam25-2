using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StoryComicManager : MonoBehaviour
{
    [SerializeField] private GameObject posObj;
    [SerializeField] private float tweenDuration = 1.5f;
    [SerializeField] private CanvasGroup continueBtn;
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private SpriteRenderer[] comicPanels;
    // time between allowed actions
    private float _nextTimeToTween = 0f;

    private Transform[] _comicCameraPositions;
    private Camera _mainCam;
    private int _currentPosIndex = 0;
    private bool _canStartClicking = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCam = Camera.main;
        _comicCameraPositions = new Transform[posObj.transform.childCount];
        Setup();
        foreach (SpriteRenderer comicPanel in comicPanels)
        {
            comicPanel.color = new Color(comicPanel.color.r, comicPanel.color.g, comicPanel.color.b, 0);
        }

        



    }

    private void Setup()
    {
        int index = 0;
        foreach (Transform child in posObj.transform)
        {
            _comicCameraPositions[index] = child;
            index++;
        }
        continueBtn.alpha = 0;
        continueBtn.interactable = false;
        _mainCam.transform.position = _comicCameraPositions[_currentPosIndex].position;
        Sequence seq = DOTween.Sequence();
        seq.Append(fader.DOFade(0, 1f).OnComplete(() =>
        {
            fader.gameObject.SetActive(false);
        }));
        seq.Append(comicPanels[0].DOFade(1, 1f));
        seq.OnComplete(() =>
        {
            _currentPosIndex++;
            _canStartClicking = true;

        });
    }

    // when the player can act again

    void Update()
    {
        // Check left mouse button
        if (Input.GetMouseButtonDown(0) && _canStartClicking)
        {
            if (Time.time >= _nextTimeToTween && _currentPosIndex < _comicCameraPositions.Length)
            {
                CheckComicPanelToShow();
                MoveToPosition();
                CheckIfFinalPosition();
                _nextTimeToTween = Time.time + (tweenDuration + .3f);
                
                
            }
        }
    }

    private void CheckComicPanelToShow()
    {
        if (_currentPosIndex == 2)
        {
            comicPanels[1].DOFade(1, 2f);
        }
        else if (_currentPosIndex == 5)
        {
            comicPanels[2].DOFade(1, 2f);
        }
        else if (_currentPosIndex == 7)
        {
            comicPanels[3].DOFade(1, 2f);
        }
    }

    public void CheckIfFinalPosition()
    {
        if(_currentPosIndex >= _comicCameraPositions.Length)
        {
            continueBtn.DOFade(1, 1f);
            continueBtn.interactable = true;
            return;
        }
    }
    
    public void MoveToPosition()
    {
        Vector3 targetPos = _comicCameraPositions[_currentPosIndex].position;

        // Move the camera to the target position and rotation using DOTween
        _mainCam.transform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                //Im lazy but this works
                _canStartClicking = true;
                
                _currentPosIndex++;
                CheckIfFinalPosition();
            });
    }

    private void OnDrawGizmosSelected()
    {
        if (posObj == null) return;
        Transform[] positions = posObj.GetComponentsInChildren<Transform>();
        Gizmos.color = Color.red;
        
        for (int i = 1; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i].position, 1f);
            if (i < positions.Length - 1)
            {
                Gizmos.DrawLine(positions[i].position, positions[i + 1].position);
            }
        }
    }

    public void ContinueBtnOnClick()
    {
        ToggleFadeInOut(false);
        SceneManager.LoadScene("TEMP_JamRelease");
    }

    public void ToggleFadeInOut(bool fadein)
    {
        if (fadein)
        {
            fader.DOFade(0, .75f).OnComplete(() =>
            {
                fader.gameObject.SetActive(false);
            });
        }
        else
        {
            fader.gameObject.SetActive(true);
            fader.DOFade(1, .75f).OnComplete(() =>
            {
            });
        }
    }
}
