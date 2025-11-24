using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public enum EffectType
    {
        ReduceAttackCooldown,   // 공격 쿨타임 감소.
        IncreaseDamage, // 대미지 증가.
        IncreaseProjectileSpeed,    // 투사체 속도 증가,
        IncreaseProjectiles // 동시 발사 수 증가.
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public string displayName;
    public string description;
    public Sprite icon;
    public EffectType effectType;
    public int maxLevel = 5;
    public float[] valuesPerLevel;

    public Rarity rarity = Rarity.Common;
    public string[] tags;
    public string[] requiresTags;  // 있어야 하는 태그
    public string[] excludesTags;   // 있으면 안되는 태그
    public string groupId;  // 상호배제/중복 제어 그룹
    public int requiresLevelMin = 1;    // 최소 플레이어 레벨
    public bool requiresOwnedWeapon;    // 해당 카드가 무기 강화형이라면 보유 필요
    public int maxPerRun = 99;  // 한 판 동안 최대 등장 회수

    public float GetValuePerLevel(int level)
    {
        int index = level - 1;
        if(valuesPerLevel != null && valuesPerLevel.Length > 0)
        {
            if(index >= 0 && index < valuesPerLevel.Length)
            {
                return valuesPerLevel[index];
            }
            else
            {
                return valuesPerLevel[valuesPerLevel.Length-1];
            }
        }

        return 0.0f;
    }
}
