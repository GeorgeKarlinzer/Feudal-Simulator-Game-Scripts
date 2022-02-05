using UnityEngine;
using static StaticData;

public class BufferDoor : BufferObject
{
    [SerializeField] private Door doorPrefab;

    public void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var newPos = DoorPlace.GetNearestNeighbour(mousePos);
        newPos.z = _transform.position.z;
        if (newPos != _transform.position)
        {
            newPos.x = Mathf.RoundToInt(newPos.x * 2) / 2f;
            newPos.y = Mathf.RoundToInt(newPos.y * 2) / 2f;
            _transform.position = newPos;
            if ((int)newPos.x - newPos.x < (int)newPos.y - newPos.y)
                _transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            else
                _transform.rotation = Quaternion.identity;
            return;
        }
    }

    public override void TryBuild()
    {
        if (!CanBuild()) return;

        Instantiate(doorPrefab, _transform.position, _transform.rotation)
            .GetComponent<Door>()
            .Build();

        DoorPlace.Hide();

        base.TryBuild();
    }

    public override void ChangePosition(GameObject movedDoor)
    {
        if (!CanBuild()) return;

        base.ChangePosition(movedDoor);
    }

    public override void Cancel()
    {
        base.Cancel();

        if (BuildMenuStatic.FreeDoors <= 0)
            foreach (var price in doorPrefab.GetComponent<Door>().prices)
                ResManager.res[price.name].SetNormalAmount();

        RotateButtonStatic.gameObject.SetActive(true);
        DoorPlace.Hide();
    }

    public bool CanBuild()
    {
        int counter = 0;

        Vector3 dir = new Vector3(0, 0, 1);
        var hits = Physics2D.RaycastAll(_transform.position, dir, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider.GetComponent<Door>())
                return false;
            if (hit.collider.GetComponent<DoorPlace>())
                counter++;
        }
        if (counter > 2) throw new System.Exception($"{counter} мест для двери на одном месте!");

        return counter == 2;
    }
}
