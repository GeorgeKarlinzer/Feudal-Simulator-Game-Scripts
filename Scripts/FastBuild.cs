using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FastBuild : MonoBehaviour
{
    public List<FastBuildBuilding> queue = new List<FastBuildBuilding>();

    void Start()
    {
        foreach (var item in queue)
        {
            var build = Instantiate(item.building, item.pos, Quaternion.Euler(item.rotation));
            StartCoroutine(Employ(build.GetComponent<IEmployable>(), item.employedCount));
            
            Door door = item.building.GetComponent<Door>();
            if (door)
                StartCoroutine(Scan(item.pos, item.rotation.z, door.doorSize));
        }
    }

    IEnumerator Employ(IEmployable employable, int count)
    {
        yield return new WaitForSeconds(1.1f);
        for (int i = 0; i < count; i++)
            employable.Employ();

    }

    IEnumerator Scan(Vector3 pos, float z, Vector2 doorSize)
    {
        yield return new WaitForSeconds(3);
        Vector2 newDoorSize = Mathf.Round(z) % 180 == 0 ? doorSize : new Vector2(doorSize.y, doorSize.x);
        MyAstarHandler.FillAreaTrue(pos, newDoorSize);
    }

    [System.Serializable]
    public class FastBuildBuilding
    {
        public GameObject building;
        public int employedCount;
        public Vector3 pos;
        public Vector3 rotation;
    }
}
