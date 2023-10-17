using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Prey
{
    protected override void ReactToPlayer(Vector3 playerPos)
    {
        base.ReactToPlayer(playerPos);

        ReactToThreat(playerPos);
    }

    protected override void ReactToPlayerLeaving(Vector3 playerPos)
    {
        base.ReactToPlayerLeaving(playerPos);

        //WaryOff = playerPos;
        SetConditionFalse(worldState, Condition.IsNearDanger);
    }
}
