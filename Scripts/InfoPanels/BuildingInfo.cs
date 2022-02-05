using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    [SerializeField, HideInInspector] private Building target;

    public T GetTarget<T>() where T : Building => (T)target;

    public void SetTarget(Building target)
    {
        this.target = target;
    }
    public virtual void InitUI() { }

    public virtual void UpdateUI() { }
}
