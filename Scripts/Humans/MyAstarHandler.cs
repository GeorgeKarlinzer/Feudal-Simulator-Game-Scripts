using Pathfinding;
using UnityEngine;

using static StaticData;

public static class MyAstarHandler
{
    /// <summary>
    /// Nodes per unit
    /// </summary>
    public static int NPU
    {
        get
        {
            if (AstarPath.active != null)
                return Mathf.RoundToInt(1 / AstarPath.active.data.gridGraph.nodeSize);
            else
                return 10;
        }
    }
    private static GraphNode GetNode(Vector3 pos)
        => AstarPath.active.data.gridGraph.GetNode(
            PosToGraphPos(pos.x),
            PosToGraphPos(pos.y));

    public static bool IsWalkable(Vector3 pos)
        => AstarPath.active.GetNearest(pos).node.Walkable;

    private static int PosToGraphPos(float pos)
        => Mathf.RoundToInt(pos * NPU + 4.5f);

    public static bool IsPathPossible(Transform human, Vector3 destination)
        => PathUtilities.IsPathPossible(GetNode(human.position), GetNode(destination));

    public static bool IsPathPossible(Transform human, Transform destination)
        => PathUtilities.IsPathPossible(GetNode(human.position), GetNode(destination.position));

    public static bool IsPathPossible(Vector3 target, Vector3 destination)
        => PathUtilities.IsPathPossible(GetNode(target), GetNode(destination));

    public static void UpdateArea(Vector2 origin, Vector2 size)
    {
        if (size == Vector2.zero) return;

        var guo = new GraphUpdateObject(new Bounds(origin, size));
        guo.updatePhysics = true;

        AstarPath.active.UpdateGraphs(guo);
        HumanWorkGlobal.CheckWaitersDelayed();
    }

    public static void FillAreaTrue(Vector2 origin, Vector2 size)
    {
        var guo = new GraphUpdateObject(new Bounds(origin, size));

        guo.modifyWalkability = true;
        guo.setWalkability = true;
        guo.updatePhysics = true;

        AstarPath.active.UpdateGraphs(guo);
        HumanWorkGlobal.CheckWaitersDelayed();
    }
}