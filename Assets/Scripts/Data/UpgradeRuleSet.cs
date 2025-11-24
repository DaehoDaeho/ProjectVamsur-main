using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeRuleSet", menuName = "Game/Upgrade Rule Set")]
public class UpgradeRuleSet : ScriptableObject
{
    public float weightCommon = 100.0f;
    public float weightRare = 35.0f;
    public float weightEpic = 12.0f;
    public float weightLegendary = 3.0f;

    public int recentAvoidCount = 2;    // 최근 N회에 같은 groupID 피하기
    public float recentPenalty = 0.25f; // 최근에 많이 나온 그룹 가중치 페널티 배수

    public string pityTag = "Weapon";   // 예 : 무기 태그가 너무 오래 안나오면 보정
    public int pityAfterLevels = 3; // 연속 N 레벨 동안 미출현 시
    public float pityBoost = 3.0f;  // 해당 태그 가중치 배수

    public float GetRarityWeight(UpgradeData.Rarity r)
    {
        switch(r)
        {
            case UpgradeData.Rarity.Common:
                {
                    return weightCommon;
                }

            case UpgradeData.Rarity.Rare:
                {
                    return weightRare;
                }

            case UpgradeData.Rarity.Epic:
                {
                    return weightEpic;
                }

            case UpgradeData.Rarity.Legendary:
                {
                    return weightLegendary;
                }
        }

        return weightCommon;
    }
}
