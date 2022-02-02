using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class Road : Building, INeighbourRelated
{
    public int SpriteIndex { get; set; }

    public Vector2 Pos2D => transform.position;

    public Sprite[] Sprites => SpritesLoaderStatic.roads;

    private new SpriteRenderer renderer;
    public SpriteRenderer Renderer => renderer;

    public Action<int> NeighbourFound => null;

    public Action<int> ThisWasFound => null;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out AIPath aIPath))
        {
            aIPath.maxSpeed = 1.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out AIPath aIPath))
        {
            aIPath.maxSpeed = 0.74f;
        }
    }

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        NeighbourRelated.Instance.CheckNeighbors<Road>(1, this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        NeighbourRelated.Instance.CheckNeighbors<Road>(-1, this);
    }
}
