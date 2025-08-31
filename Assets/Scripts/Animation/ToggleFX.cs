using UnityEngine;

public class ToggleFX : MonoBehaviour
{
   public void EndFX(string toggle)
    {
        gameObject.SetActive(bool.Parse(toggle));
    }
}
