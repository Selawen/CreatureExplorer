using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Planner))]
public class Creature : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject testTarget;

    [Header("GOAP")]
    [SerializeField] private WorldState currentWorldState;
    [field:SerializeField] private Goal[] possibleGoals;
    [field:SerializeField] private List<Action> currentPlan;
    
    private Goal currentGoal;
    private Action currentAction;

    private Planner planner;


    // Start is called before the first frame update
    void Start()
    {
        planner = GetComponent<Planner>();
        currentGoal = possibleGoals[0];

        if (currentWorldState.WorldStates.Length <=0)
        {
            currentWorldState = new WorldState();
        }

        currentPlan = planner.Plan(currentGoal, currentWorldState);
        currentAction = currentPlan[0];
    }

    // Update is called once per frame
    void Update()
    {
        

        if (debug)
        {
            goalText.text = currentGoal.Name;
            actionText.text = currentAction.Name;
        }
    }
}
