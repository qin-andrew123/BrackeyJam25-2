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
        // TODO: ADD UI For Round Number as well
    }
    private void EndRound()
    {
        OnRoundEnd?.Invoke();
    }

    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnEnable()
    {
        PlayerInput.TEMPSetRoundRequirements += InitiateNextRound;
    }
    // TODO: Delete or refactor after initial prototyping since this is reliant on PlayerInput hitting f5
    private void OnDisable()
    {
        PlayerInput.TEMPSetRoundRequirements -= InitiateNextRound;
    }
}
