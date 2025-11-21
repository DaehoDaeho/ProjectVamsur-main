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

    public string displayName;
    public string description;
    public Sprite icon;
    public EffectType effectType;
    public int maxLevel = 5;
    public float[] valuesPerLevel;

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
