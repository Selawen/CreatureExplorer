using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torca : Creature
{
    protected override void Start()
    {
        base.Start();

        surroundCheck = new CheckSurroundings(CheckForFood);
    }

    protected override void ReactToPlayer(Vector3 playerPos, float playerLoudness)
    {
        base.ReactToPlayer(playerPos, playerLoudness);

        WaryOff = playerPos;
    }

    protected override void ReactToPlayerLeaving(Vector3 playerPos)
    {
        base.ReactToPlayerLeaving(playerPos);
    }

    /// <summary>
    /// Checks for food in neighbourhood and ups the hunger value with the amount of food nearby
    /// </summary>
    public override void CheckForFood()
    {
        int foodcount = 0;
        foreach (Collider c in Physics.OverlapSphere(transform.position, data.HearingSensitivity*CurrentAction.Awareness))
        {
            if (c.gameObject.GetComponent(data.FoodSource) != null)
            {
                foodcount++;
            }
        }

        UpdateValues(StateType.Hunger, foodcount, StateOperant.Add);

        //DebugMessage($"found {foodcount} {FoodSource}, hunger is now {currentCreatureState.Find(StateType.Hunger).StateValue}");
    }
}
