using System.Collections;
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

    [Header("GOAP")]
    [SerializeField] private List<Goal> possibleGoals;
    [SerializeField] private List<Action> currentPlan;
    
    private Goal currentGoal;
    private Action currentAction;

    private Planner planner;



    // Start is called before the first frame update
    void Start()
    {
        planner = GetComponent<Planner>();

    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            goalText.text = currentGoal.name;
            actionText.text = currentAction.name;
        }
    }
}
