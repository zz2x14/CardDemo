using UnityEngine;

[CreateAssetMenu(menuName = "Configure/NewImageConfigure",fileName = "NewImageConfigure")]
public class ImageConfigure : ScriptableObject
{
    [Header("房间图标")] 
    public Sprite enemyRoomIcon;
    public Sprite eliteEnemyRoomIcon;
    public Sprite shopRoomIcon;
    public Sprite doubleShopRoomIcon;
    public Sprite randomRoomIcon;
    public Sprite restRoomIcon;
    public Sprite treasureRoomIcon;
}
