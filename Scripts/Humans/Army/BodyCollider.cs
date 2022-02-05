using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollider : MonoBehaviour
{
    public bool Enabled
    {
        set => GetComponent<Collider2D>().enabled = value;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("soldierBody"))
        {
            var deltaPos = (Vector2)(collision.transform.position - transform.position);

            collision.transform.parent.position += (Vector3)(deltaPos.normalized * Time.deltaTime * 0.2f);
        }

        if (collision.GetComponent<BuildingChild>())
        {
            var deltaPos = (Vector2)(collision.transform.position - transform.position);

            transform.parent.position -= (Vector3)(deltaPos.normalized * Time.deltaTime * 0.2f);
        }
    }
}
