using UnityEngine;

public class BuildingChild : MonoBehaviour
{
    [SerializeField] private Building building;
    private void OnMouseUp()
    {
        if (building != null)
            building.OnMouseUp();
    }
}
