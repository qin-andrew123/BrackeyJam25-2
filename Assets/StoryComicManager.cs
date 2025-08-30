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
    // time between allowed actions
    private float _nextTimeToTween = 0f;

    private Transform[] _comicCameraPositions;
    private Camera _mainCam;
    private int _currentPosIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCam = Camera.main;
        _comicCameraPositions = new Transform[posObj.transform.childCount];
        Setup();
        fader.DOFade(0, 1f).OnComplete(() =>
        {
            fader.gameObject.SetActive(false);
        });
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
        MoveToPosition();
    }

    // when the player can act again

    void Update()
    {
        // Check left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= _nextTimeToTween && _currentPosIndex < _comicCameraPositions.Length)
            {
                MoveToPosition();
               
                CheckIfFinalPosition();
                _nextTimeToTween = Time.time + (tweenDuration + .3f);
            }
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
        Vector3 targetPos = new Vector3(_comicCameraPositions[_currentPosIndex].position.x,
            _comicCameraPositions[_currentPosIndex].position.y,
            _mainCam.transform.position.z);

        // Move the camera to the target position and rotation using DOTween
        _mainCam.transform.DOMove(targetPos, tweenDuration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
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
