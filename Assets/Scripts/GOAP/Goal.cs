using UnityEngine;

[CreateAssetMenuAttribute(fileName = "NewGoal", menuName = "GOAP/Create Goal")]
public class Goal: ScriptableObject
{
    [field:SerializeField] public string Name { get; private set; }

    [field: SerializeField] public MoodState[] Target { get; private set; }

    private void OnValidate()
    {
        foreach (MoodState mood in Target)
        {
            if (!(mood.Operator == StateOperant.GreaterThan || mood.Operator == StateOperant.LessThan))
            {
                Debug.LogError("Goal Target relation has to be either greater or less than");
            }
        }
    }
}
