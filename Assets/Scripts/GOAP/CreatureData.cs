using UnityEngine;

[CreateAssetMenuAttribute(fileName = "NewCreatureData", menuName = "GOAP/Create Creature Data")]
public class CreatureData : ScriptableObject
{
    [Tooltip("The name of the script that is on this creature's foodsource")]
    [field: SerializeField] public string FoodSource { get; private set; } 
    [Tooltip("The name of the script that is on this creature's sleeping spots")]
    [field: SerializeField] public string SleepSpot { get; private set; }
    [field: SerializeField] public float Bedtime { get; private set; }
    [field: SerializeField] public float WakeTime { get; private set; }
    [field: SerializeField] public float HearingSensitivity { get; private set; }
    [field: SerializeField] public float CheckSurroundingsTimer { get; private set; } 

    [field: SerializeField] public float DecayTimer { get; private set; }

    [field: SerializeField] public CreatureState ChangesEverySecond { get; private set; }
    [field: SerializeField] public CreatureState ReactionToAttack { get; private set; }
    [field: SerializeField] public CreatureState ReactionToPlayer { get; private set; }
}
