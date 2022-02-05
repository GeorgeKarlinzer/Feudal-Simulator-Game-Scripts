using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightControllerTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private float dayIntensivity = 1;
    [SerializeField] private float nightIntensivity = 0.5f;
    private void Awake()
    {
        TimeOfDaySystem.ChangeLightAddEvent(SetIntensity);
    }

    public void SetIntensity(bool isDay)
    {
        Color.RGBToHSV(tilemap.color, out float H, out float S, out float V);
        tilemap.color = Color.HSVToRGB(H, S, isDay ? dayIntensivity : nightIntensivity);
    }
}
