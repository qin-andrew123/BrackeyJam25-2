using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Animation_Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] List<Animator> playerAnimators;

#if UNITY_EDITOR
    [Header("Debug Animation")]
    [SerializeField] bool enableDebug;
    [SerializeField] bool Idle;
    [SerializeField] bool Trigger;
    [SerializeField] bool Left;	
    [SerializeField] bool Right;	
    [SerializeField] bool Victory;
    [SerializeField] bool Victory2;
    [SerializeField] bool Defeat;
    [SerializeField] bool Defeat2;
    [SerializeField] bool EndLoop;
    [SerializeField] bool Knead;

#endif

    private void SetBool(string name, bool value)
    {
        foreach (var animator in playerAnimators) animator.SetBool(name, value);
    }

    private void SetTrigger(string name)
    {
        foreach (var animator in playerAnimators) animator.SetTrigger(name);
    }

    #region Animation triggers
    public void playKnead()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("Mix/Combine");
    }
    public void playVictory()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("Victory");
    }
    public void playVictory()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("Victory2");
    }

    public void playDefeat()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("Defeat");
    }
    public void playDefeat2()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("Defeat2");
    }

    public void endLoop()
    {
        foreach (var animator in playerAnimators) animator.SetTrigger("EndLoop");
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (enableDebug)
        {
            if (Idle)
            {
                SetBool("Bool 1", true);
            }
            else
            {
                SetBool("Bool 1", false);
            }

            if (Left)
            {
                SetBool("Bool 2", true);
            }
            else
            {
                SetBool("Bool 2", false);
            }

            if (Right)
            {
                SetBool("Bool 3", true);
            }
            else
            {
                SetBool("Bool 3", false);
            }

            if (Trigger)
            {
                Trigger = false;
                SetTrigger("Trigger");
            }

            if (Victory)
            {
                Victory = false;
                SetTrigger("Victory");
            }

            if (Victory2)
            {
                Victory2 = false;
                SetTrigger("Victory2");
            }

            if (EndLoop)
            {
                EndLoop = false;
                SetTrigger("EndLoop");
            }

            if (Defeat)
            {
                Defeat = false;
                SetTrigger("Defeat");
            }

            if (Defeat2)
            {
                Defeat2 = false;
                SetTrigger("Defeat2");
            }

            if (Knead)
            {
                Knead = false;
                SetTrigger("Mix/Combine");
            }
        }
#endif
    }
}

