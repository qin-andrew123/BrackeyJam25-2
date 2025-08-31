using UnityEngine;

public class KneadFX : MonoBehaviour
{
    public GameObject poofFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void ToggleFX(string toggle)
    {
        poofFX.SetActive(bool.Parse(toggle));
    }
}
