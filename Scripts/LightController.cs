using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public SpriteRenderer image;
    public Sprite daySprite;
    public Sprite nigthSprite;

    [SerializeField] private float dayIntensivity = 1;
    [SerializeField] private float nightIntensivity = 0.5f;
    [SerializeField] private bool isHidding = false;

    private void Awake()
    {
        TimeOfDaySystem.ChangeLightAddEvent(SetIntensity);
        if (isHidding)
            gameObject.SetActive(false);
    }

    public void SetIntensity(bool isDay)
    {
        if (daySprite == null)
        {
            if (image.sprite != null)
            {
                Color.RGBToHSV(image.color, out float H, out float S, out float V);
                image.color = Color.HSVToRGB(H, S, isDay ? dayIntensivity : nightIntensivity);
            }
        }
        else
            image.sprite = isDay ? daySprite : nigthSprite;
    }

    private void OnDestroy()
    {
        TimeOfDaySystem.ChangeLightRemoveEvent(SetIntensity);
    }
}
