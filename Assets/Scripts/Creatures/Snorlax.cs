using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snorlax : Charger
{
    [Header("Snorlax")]

    [SerializeField] private Action sleepAction;
    [SerializeField] private Action moveAction, fleeAction;

    private bool luredAway = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        surroundCheck = new CheckSurroundings(CheckForFood);
        surroundCheck += CheckForPredators;
        surroundCheck += CheckForFleeing;
        StartAction();
        StartCoroutine(LookAtSurroundings());
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (luredAway && (CurrentAction.finished || CurrentAction.failed))
        {
            if (CurrentAction == moveAction)
            {
                surroundCheck += CheckForFood;
            }

            Interrupt(sleepAction);
            luredAway = false;
        } 
    }

    protected override void ReactToThreat(Vector3 threatPosition, float threatLoudness)
    {
        base.ReactToThreat(threatPosition, threatLoudness);

        Interrupt(fleeAction);
        GetComponent<Collider>().isTrigger = true;
    }

    protected override void ReactToAttack(Vector3 attackPos)
    {
        base.ReactToAttack(attackPos);
        GetComponent<Collider>().isTrigger = true;
    }
    
    
    protected override void ReactToPlayer(Vector3 playerPos, float playerLoudness)
    {
    }

    protected override void ReactToPlayerLeaving(Vector3 playerPos)
    {
    }

    /// <summary>
    /// Checks for food in neighbourhood and ups the hunger value with the amount of food nearby
    /// </summary>
    public override void CheckForFood()
    {
        Food f = null;
        if (LookForObjects<Food>.TryGetClosestObject(f, transform.position, data.HearingSensitivity, out f) && CurrentAction != fleeAction)
        {
            currentTarget = f.gameObject;
            surroundCheck -= CheckForFood;

            Interrupt(moveAction, "", true);
            luredAway = true;
            GetComponent<Collider>().isTrigger = true;
        }
    }
}
