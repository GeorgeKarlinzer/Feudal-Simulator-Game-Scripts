using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static StaticData;

public class Door : Marketable, ISelectable
{
    public Vector2 doorSize = new Vector2(0.4f, 0.2f);
    private Vector2 DoorSize 
        => Mathf.Round(_transform.eulerAngles.z) % 180 == 0 ? doorSize : new Vector2(doorSize.y, doorSize.x);

    [SerializeField] private bool canSelect = true;
    [SerializeField] private LayerMask workerLayer;
    [SerializeField] private LayerMask doorPlaceLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform _transform;
    public bool isStatic = false;

    [SerializeField] private List<SpriteRenderer> graphics;
    public List<SpriteRenderer> Graphics { get => graphics; }

    int collisionCount = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("human"))
        {
            if(collisionCount++ == 0)
                animator.SetTrigger("Open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("human"))
        {
            if (--collisionCount == 0)
                animator.SetTrigger("Close");
        }
    }

    public void Build()
    {
        if (BuildMenuStatic.FreeDoors > 0)
        {
            BuildMenuStatic.FreeDoors--;
            prices = new List<Price>(0);
        }
        else
            Price.Spend(prices.ToArray());
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !Cursor.IsBusy && canSelect && !Cursor.IsCameraMoving)
            SideMenuStatic.ShowSideMenu(gameObject);
    }

    private void OnEnable()
    {
        MyAstarHandler.FillAreaTrue(_transform.position, DoorSize);

        ChangeDoorPlaceState(false);
    }

    private void OnDisable()
    {
        MyAstarHandler.UpdateArea(_transform.position, DoorSize);

        ChangeDoorPlaceState(true);

        PushAwayWorker();
    }

    private void PushAwayWorker()
    {
        var pushDirection = DoorSize.x > DoorSize.y ? new Vector3(0, DoorSize.y / 2) : new Vector3(DoorSize.x / 2, 0);

        var workers = Physics2D.RaycastAll(_transform.position, Vector2.zero, Mathf.Infinity, workerLayer);
        foreach (var w in workers)
        {
            // Если дверь расположена горизонтально (направление вектора сдвига по иксу равно нулю) то в зависимости от того с какой стороны рабочий, в ту сторону
            // Он будет сдвигаться относительно двери
            float pushPhase = Mathf.Sign(pushDirection.x == 0 ? w.transform.position.y - _transform.position.y : w.transform.position.x - _transform.position.x);
            w.transform.position += pushDirection * pushPhase;
        }
    }

    private void ChangeDoorPlaceState(bool state)
    {
        Vector3 dir = new Vector3(0, 0, 1);
        var hits = Physics2D.RaycastAll(_transform.position, dir, Mathf.Infinity, doorPlaceLayer);
        foreach (var hit in hits)
            if(state)
                DoorPlace.Add(hit.collider.GetComponent<SpriteRenderer>());
            else
                DoorPlace.Remove(hit.collider.GetComponent<SpriteRenderer>());
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public override void Sell()
    {
        base.Sell();

        Destroy();
    }
}
