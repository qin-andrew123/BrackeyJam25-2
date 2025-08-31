using UnityEngine;

public class ToggleFX : MonoBehaviour
{
   public void enableFX()
    {
        gameObject.SetActive(true);
    }
   public void EndFX()
    {
        gameObject.SetActive(false);
    }
}
