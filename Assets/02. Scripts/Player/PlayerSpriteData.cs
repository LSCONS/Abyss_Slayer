using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSpriteData", menuName = "Player/SpriteData")]
public class PlayerSpriteData : ScriptableObject
{
    [field: Header("어드레서블 스프라이트 이름")]
    [field: SerializeField] public string WeaponTopName { get; private set; }
    [field: SerializeField] public string ClothTopName { get; private set; }
    [field: SerializeField] public string HairTopName { get; private set; }
    [field: SerializeField] public string ClothBottomName { get; private set; }
    [field: SerializeField] public string HairBottomName { get; private set; }
    [field: SerializeField] public string FaceName { get; private set; }
    [field: SerializeField] public string SkinName { get; private set; }
    [field: SerializeField] public string WeaponBottomName { get; private set; }                              
}                                                   
