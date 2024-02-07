using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game Settings/New Settings")]
public class GameSettings : ScriptableObject
{
    [field:Range(0.01f, 1f), SerializeField] public float LookSensitivity { get; set; } = 0.1f;
    [field: Range(0, 100), SerializeField] public float GlobalVolume { get; set; } = 100f;
    [field: Range(0, 100), SerializeField] public float SFXVolume { get; set; } = 100f;
    [field: Range(0, 100), SerializeField] public float UIVolume { get; set; } = 100f;
    [field: Range(0, 100), SerializeField] public float MusicVolume { get; set; } = 100f;

}
