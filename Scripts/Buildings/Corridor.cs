using System;
using UnityEngine;
using static StaticData;

public class Corridor : Building, INeighbourRelated
{
    [SerializeField] private GameObject[] doorPlaces;
    [Tooltip("Ширина и высота прохода между корридорами в боковой части")]
    [SerializeField] private Vector2 gateSize;

    private new SpriteRenderer renderer;

    public Vector2 Pos2D => transform.position;

    public int SpriteIndex { get; set; }

    public Sprite[] Sprites => SpritesLoaderStatic.corridors;

    public SpriteRenderer Renderer => renderer;

    public Action<int> NeighbourFound => (i) =>
    {
        Vector2 newGateSize = i % 2 == 0 ? gateSize : (gateSize.y, gateSize.x).ToVec();
        var origin = (Vector2)transform.position + new Vector2(0.5f * Mathf.Cos(i * Mathf.PI / 2), 0.5f * Mathf.Sin(i * Mathf.PI / 2));
        MyAstarHandler.FillAreaTrue(origin, newGateSize);
    };

    public Action<int> ThisWasFound => (delta) =>
    {
        var absDelta = Mathf.Abs(delta);
        doorPlaces[(int)Mathf.Log(absDelta, 2)].SetActive(delta < 0);
    };


    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        NeighbourRelated.Instance.CheckNeighbors<Corridor>(1, this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        NeighbourRelated.Instance.CheckNeighbors<Corridor>(-1, this);
    }
}
