using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeighbourRelated
{
    int SpriteIndex { get; set; }

    Vector2 Pos2D { get; }

    Sprite[] Sprites { get; }

    SpriteRenderer Renderer { get; }

    Action<int> NeighbourFound { get; }

    Action<int> ThisWasFound { get; }
}
