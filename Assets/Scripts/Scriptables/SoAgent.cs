using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Agent")]
public class SoAgent : ScriptableObject
{
    public string nameAgent;
    public Sprite[] spritesMove, spritesDeath, spritesAttack;
}

