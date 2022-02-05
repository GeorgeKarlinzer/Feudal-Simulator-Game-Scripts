using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeOfDaySystem : MonoBehaviour
{
    [SerializeField] private GameObject rain;
    private static float dayIntensity = 1;
    private static float nightIntensity = 0.5f;

    private static bool isDay = true;
    private static UnityAction<bool> changeLightEvent;

    public static void ChangeLightAddEvent(UnityAction<bool> action)
    {
        changeLightEvent += action;
        if (!isDay)
            action(isDay);
    }

    public static void ChangeLightRemoveEvent(UnityAction<bool> action)
    {
        changeLightEvent -= action;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            SetDay();
        if (Input.GetKeyDown(KeyCode.L))
            SetNight();
        if (Input.GetKeyDown(KeyCode.J))
            SetRain();
    }
#endif

    public void SetRain()
    {
        //changeLightEvent.Invoke(nightIntensity);
    }

    public void SetDay()
    {
        isDay = true;
        changeLightEvent.Invoke(isDay);
    }

    public void SetNight()
    {
        isDay = false;
        changeLightEvent.Invoke(isDay);
    }
}
