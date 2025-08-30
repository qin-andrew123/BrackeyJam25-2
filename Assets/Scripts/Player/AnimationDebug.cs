using UnityEngine;

public class Animation_Debug : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Animator m_Animator;

#if UNITY_EDITOR
    [Header("Debug Animation")]
    [SerializeField] bool Idle;
    [SerializeField] bool Trigger;
    [SerializeField] bool Left;	
    [SerializeField] bool Right;	

#endif

    private void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }

    public void SetBool(string name, bool value)
    {
        m_Animator.SetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        m_Animator.SetTrigger(name);
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
#endif
    }
}

