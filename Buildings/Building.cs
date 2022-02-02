using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using static StaticData;

public class Building : Marketable, ISelectable
{
    public bool canMove = true;
    public bool canRotate = true;
    public bool getFreeDoor = true;

    [SerializeField] private Vector2Int size;
    public Vector2Int Size
    {
        get
        {
            if (Mathf.RoundToInt(transform.rotation.eulerAngles.z) / 90 % 2 == 1)
                return (size.y, size.x).ToV2();
            else
                return size;
        }
        private set => size = value;
    }

    public Info info;

    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public Sprite movingSprite;

    [SerializeField] private List<SpriteRenderer> graphics = new List<SpriteRenderer>();
    public List<SpriteRenderer> Graphics { get => graphics; }

    [HideInInspector] public UnityEvent UpdateUI = new UnityEvent();

    [Serializable]
    public struct Info
    {
        public string description;
        public Sprite classSprite;
        public GameObject panel;
        public Sprite sprite;
    }

    public void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !Cursor.IsBusy && !Cursor.IsCameraMoving)
            SideMenuStatic.ShowSideMenu(gameObject);
    }

    protected virtual void OnEnable()
    {
        MyAstarHandler.UpdateArea(transform.position, Size);
        ForestStatic.ClearArea(transform.position, Size);
        MapManagerStatic.SetMapFullness(Size, transform.position, false);
    }

    protected virtual void OnDisable()
    {
        MyAstarHandler.UpdateArea(transform.position, Size);
        MapManagerStatic.SetMapFullness(Size, transform.position, true);
    }

    public virtual void Build()
    {
        Price.Spend(prices.ToArray());
        Health = MaxHealth;
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public override void Sell()
    {
        base.Sell();
        Destroy();
    }
}