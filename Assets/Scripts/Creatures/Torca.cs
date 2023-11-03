using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torca : Creature
{
    protected override void ReactToPlayer(Vector3 playerPos, float playerLoudness)
    {
        base.ReactToPlayer(playerPos, playerLoudness);

        WaryOff = playerPos;
    }

    protected override void ReactToPlayerLeaving(Vector3 playerPos)
    {
        base.ReactToPlayerLeaving(playerPos);
    }
}
