using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game Settings/New Settings")]
public class GameSettings : ScriptableObject
{
    [field:Range(0.01f, 1f), SerializeField] public float LookSensitivity { get; set; } = 0.1f;
}
