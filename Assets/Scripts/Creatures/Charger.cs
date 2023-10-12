using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Creature
{
    protected override void ReactToPlayer(Vector3 playerPos)
    {
        base.ReactToPlayer(playerPos);

        WaryOff = playerPos;
        worldState |= Condition.IsNearDanger;
    }

    protected override void ReactToPlayerLeaving(Vector3 playerPos)
    {
        base.ReactToPlayerLeaving(playerPos);

        WaryOff = playerPos;
        worldState &= ~Condition.IsNearDanger;
    }
}
