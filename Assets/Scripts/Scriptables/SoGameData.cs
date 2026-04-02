using UnityEngine;
using Sirenix.OdinInspector;

public class SoGameData : ScriptableObject
{
    [Title("Scenes")]
    public string sceneMainMenu;
    public string sceneGame;
    
    [Title("Sprite sorting layers")]
    public string spriteLayerEnemy;
    public string spriteLayerProjectiles;
    public string spriteLayerPlayer;
    public string spriteLayerFloatingDamage;
}
