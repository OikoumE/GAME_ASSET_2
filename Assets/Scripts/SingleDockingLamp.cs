using UnityEngine;

public class SingleDockingLamp : MonoBehaviour
{
    public bool EnableLight;
    [SerializeField] private Material greenLED, redLED;

    [SerializeField] private MeshRenderer mR;

    private Light pLight;

    private void Start()
    {
        OnStart();
    }

    private void OnValidate()
    {
        OnStart();
    }

    private void OnStart()
    {
        pLight = GetComponentInChildren<Light>();
        Material mat;
        Color lightColor;
        if (EnableLight)
        {
            mat = greenLED;
            lightColor = Color.green;
        }
        else
        {
            mat = redLED;
            lightColor = Color.red;
        }

        pLight.color = lightColor;
        mR.material = mat;
    }
}