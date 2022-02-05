using UnityEngine;
using UnityEngine.Events;

public class AutoClosingSystem : MonoBehaviour
{
    public static bool canExecute;
    private bool waitFrame;
    private UnityEvent OnPointerDownAction = new UnityEvent();

    public void SetCanExecute(bool state) 
        => canExecute = state;

    public void SetAction(UnityAction action)
    {
        enabled = true;
        waitFrame = true;
        canExecute = true;
        OnPointerDownAction.RemoveAllListeners();
        OnPointerDownAction.AddListener(action);
    }

    public void Cancel()
    {
        canExecute = true;
        OnPointerDownAction.RemoveAllListeners();
        enabled = false;
    }

    private void OnPointerDown()
    {
        OnPointerDownAction.Invoke();
        OnPointerDownAction.RemoveAllListeners();
        enabled = false;
    }

    void Update()
    {
        if (!waitFrame)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && canExecute)
                OnPointerDown();
#endif
            if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began && canExecute)
                OnPointerDown();
        }
        else
            waitFrame = false;
    }
}
