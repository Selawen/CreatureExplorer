using UnityEngine;

public class Guard : Torca
{
    [Header("Guard")]
    [SerializeField] private GameObject patrolTarget;
    [SerializeField] private Vector3[] patrolPoints;

    private int patrolIndex = 0;


    // Start is called before the first frame update
    protected override void Start()
    {
        if (patrolPoints.Length < 1 || patrolTarget == null)
        {
            Debug.LogError("No patrol points or target set");
            this.enabled = false;
            return;
        }

        patrolTarget.transform.position = patrolPoints[patrolIndex];
        currentTarget = patrolTarget;
        base.Start();
    }

    protected override void FixedUpdate()
    {
        if (CurrentAction.GetType() == typeof(Move)&&(CurrentAction.failed || CurrentAction.finished))
        {
            patrolIndex++;
            patrolIndex %= patrolPoints.Length;

            patrolTarget.transform.position = patrolPoints[patrolIndex];
            currentTarget = patrolTarget;
        }

        base.FixedUpdate();
    }

    protected override void StartAction()
    {
        if (currentTarget == null)
        {
            currentTarget = patrolTarget;
        }

        base.StartAction();
    }

    /// <summary>
    /// Checks for food in neighbourhood and ups the hunger value with the amount of food nearby
    /// </summary>
    protected override void CheckForFood()
    {
        int foodcount = 0;
        foreach (Collider c in Physics.OverlapSphere(transform.position, data.HearingSensitivity * CurrentAction.Awareness))
        {
            if (c.gameObject.GetComponent(data.FoodSource) != null)
            {
                foodcount++;
            }
        }

        UpdateValues(StateType.Hunger, foodcount*50, StateOperant.Add);

        //DebugMessage($"found {foodcount} {FoodSource}, hunger is now {currentCreatureState.Find(StateType.Hunger).StateValue}");
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(patrolTarget.transform.position, 0.2f);
    }
#endif
}
