using System;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static event Action<int> OnRoundStart;
    public static event Action OnRoundEnd;
    private int mRoundNumber = 0;

    private void InitiateNextRound()
    {
        mRoundNumber++;
        OnRoundStart?.Invoke(mRoundNumber);
    }
    private void EndRound()
    {
        OnRoundEnd?.Invoke();
    }

    private void OnEnable()
    {
        LevelManager.OnBakeSceneLoad += InitiateNextRound;
    }
    private void OnDisable()
    {
        LevelManager.OnBakeSceneLoad -= InitiateNextRound;
    }
}
