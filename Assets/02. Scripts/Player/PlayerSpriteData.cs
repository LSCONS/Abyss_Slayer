using UnityEngine;

public class PlayerSpriteData
{
    [field: Header("어드레서블 스프라이트 이름")]
    [field: SerializeField] public string WeaponTopName { get; private set; }
    [field: SerializeField] public string ClothTopName { get; private set; }
    [field: SerializeField] public string ClothBottomName { get; private set; }
    [field: SerializeField] public string WeaponBottomName { get; private set; }     
    
    public void SetSpriteName(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Mage:
                WeaponTopName       = "weapon5_c4_top";
                ClothTopName        = "cloth10_c7_top";
                ClothBottomName     = "cloth10_c7_bot";
                WeaponBottomName    = "weapon5_c4_bot";
                break;
            case CharacterClass.Tanker:
                WeaponTopName = "weapon2_top";
                ClothTopName = "cloth14_c8_top";
                ClothBottomName = "cloth14_c8_bot";
                WeaponBottomName = "weapon2_bot";
                break;
            case CharacterClass.MagicalBlader:
                WeaponTopName = "weapon1_top";
                ClothTopName = "cloth1_c2_top";
                ClothBottomName = "cloth1_c2_bot";
                WeaponBottomName = "weapon1_bot";
                break;
            case CharacterClass.Healer:
                WeaponTopName = "weapon5_c2_top";
                ClothTopName = "cloth5_c3_top";
                ClothBottomName = "cloth5_c3_bot";
                WeaponBottomName = "weapon5_c2_bot";
                break;
            case CharacterClass.Rogue:
                WeaponTopName = "cloth2_c1_top";
                ClothTopName = "cloth11_c5_top";
                ClothBottomName = "cloth11_c5_bot";
                WeaponBottomName = "cloth2_c1_top";
                break;

        }
    }
}                                                   
