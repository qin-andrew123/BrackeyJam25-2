using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Animation_Debug : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] List<Animator> animators;

#if UNITY_EDITOR
    [Header("Debug Animation")]
    [SerializeField] bool Idle;
    [SerializeField] bool Trigger;
    [SerializeField] bool Left;	
    [SerializeField] bool Right;	
    [SerializeField] bool Victory;
    [SerializeField] bool Defeat;
    [SerializeField] bool EndLoop;
    [SerializeField] bool Knead;


#endif

    private void Start()
    {

    }

    public void SetBool(string name, bool value)
    {
        foreach (var animator in animators) animator.SetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        foreach (var animator in animators) animator.SetTrigger(name);
    }


    // Update is called once per frame
    void Update()
    {
     
#if UNITY_EDITOR
       
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

            if (Knead)
            {
                Knead = false;
                SetTrigger("Mix/Combine");
            }
#endif
    }
}

