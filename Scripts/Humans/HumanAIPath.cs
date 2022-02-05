using Pathfinding;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class HumanAIPath : AIPath
{
    protected IHuman human;
    protected AIDestinationSetter destinationSetter;

    /// <summary>
    /// Max delta between last path node position and target position
    /// </summary>
    protected float deltaTarget;

    protected override void Awake()
    {
        base.Awake();
        deltaTarget = 0.4f;
        human = GetComponent<IHuman>();
        destinationSetter = GetComponent<AIDestinationSetter>();
    }

    public void Enable()
        => enabled = true;

    public override void OnTargetReached()
    {
        var targetPos = (Vector2)destinationSetter.target.position - (Vector2)transform.position;

        enabled = false;

        if (Mathf.Abs(targetPos.magnitude) > deltaTarget)
        {
            human.CantReachTarget();
            return;
        }

        StartSmoothRotateToTarget(targetPos);

        human.ExecuteAction();
    }

    protected void StartSmoothRotateToTarget(Vector2 targetPos)
        => StartCoroutine(SmoothRotateToTarget(targetPos));

    IEnumerator SmoothRotateToTarget(Vector2 targetPos)
    {
        float angle = -transform.rotation.eulerAngles.z - 90 + Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;

        float time = 0.1f;
        float dTime = 0.01f;
        int steps = (int)(time / dTime);
        for (int i = 0; i < steps; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + angle / steps);
            yield return new WaitForSecondsRealtime(dTime);
        }
    }

    protected override void OnPathComplete(Path newPath)
    {
        // If after path was updated human can not reach target, he waits untill path appears

        base.OnPathComplete(newPath);

        var deltaPos = (Vector2)((Vector3)newPath.path.Last().position - destinationSetter.target.position);

        // If the target further than last node in path (that means that deltaPos.magnitude > 0.4) then human cannot reach target
        if (Mathf.Abs(deltaPos.magnitude) > deltaTarget || !destinationSetter.target.gameObject.activeInHierarchy)
        {
            if (canMove)
                human.CantReachTarget();
        }
        else if (!canMove)
            human.StartGo();
    }
}
