using UnityEngine;

public class InfoHelper : MonoBehaviour
{
    public void SetCanExecute(bool state)
    {
#if !UNITY_EDITOR
        AutoClosingSystem.canExecute = state;
#endif
    }
}
