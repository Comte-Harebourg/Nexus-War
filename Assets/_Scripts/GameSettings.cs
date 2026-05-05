using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Config/GameSettings")]
public class GameSettings : ScriptableObject
{
    public bool SkipAnimation = false;
    public bool Bot = true;
    public bool AutoEndTurn = false;
    public float AnimationSpeed = 0.15f;
    public int PopUpSize = 10;
    public int Level = 0;
    public int Faction = 0;
}