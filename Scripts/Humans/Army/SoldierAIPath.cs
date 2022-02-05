using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAIPath : HumanAIPath
{
    public override void OnTargetReached()
    {
        base.OnTargetReached();
        // TODO: Хождения в зоны куда нельзя попасть если человек не главный в группе
    }
}
