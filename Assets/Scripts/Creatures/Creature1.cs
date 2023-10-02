using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature1 : Creature
{
    protected override void ReactToPlayer(Vector3 playerPos)
    {
        base.ReactToPlayer(playerPos);

        waryOff = playerPos;
    }
}
