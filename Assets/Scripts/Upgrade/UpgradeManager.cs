using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 후보 선정 과정
/// 1) 유효성 필터(만렙/잠금/배제)
/// 2) 희귀도 가중치
/// 3) 최근 중복/그룹 페널티
/// 4) 피티 보정 (오래 미출현 태그)
/// 5) 가중치 룰렛으로 중복 없이 3장 선택
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeData> upgradePool = new List<UpgradeData>();

    [SerializeField]
    private UpgradeRuleSet ruleSet;

    [SerializeField]
    private UpgradeHistory history;

    private Dictionary<UpgradeData, int> levels = new Dictionary<UpgradeData, int>();
        
    [SerializeField]
    private WeaponManager weaponManager;

    private System.Random rng;

    private void Awake()
    {
        rng = new System.Random();
    }

    bool IsEligible(UpgradeData d, int playerLevel, HashSet<string> ownedTags)
    {
        if(d == null)
        {
            return false;
        }

        // 최대 레벨 제외
        int lv = GetLevel(d);
        if(lv >= d.maxLevel)
        {
            return false;
        }

        // 한 판 등장 제한
        if(d.maxPerRun <= 0)
        {
            return false;
        }

        // 플레이어 레벨 요건
        if(playerLevel < d.requiresLevelMin)
        {
            return false;
        }

        // 해금 태그 검사
        if(d.requiresTags != null && d.requiresTags.Length > 0)
        {
            if(ownedTags == null)
            {
                return false;
            }

            for(int i=0; i<d.requiresTags.Length; ++i)
            {
                if (ownedTags.Contains(d.requiresTags[i]) == false)
                {
                    return false;
                }
            }
        }

        // 상호 배제 태그 검사
        if(d.excludesTags != null && d.excludesTags.Length > 0 && ownedTags != null)
        {
            for(int i=0; i<d.excludesTags.Length; ++i)
            {
                if (ownedTags.Contains(d.excludesTags[i]) == true)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    public List<UpgradeData> DrawThree(int playerLevel = 1, HashSet<string> ownedTags = null)
    {
        List<UpgradeData> candidates = new List<UpgradeData>();

        for(int i=0; i<upgradePool.Count; ++i)
        {
            UpgradeData data = upgradePool[i];
            if(IsEligible(data, playerLevel, ownedTags) == true)
            {
                candidates.Add(data);
            }
        }

        if(candidates.Count == 0)
        {
            return new List<UpgradeData>();
        }

        List<float> weights = new List<float>(candidates.Count);
        for(int i=0; i<candidates.Count; ++i)
        {
            float w = ruleSet.GetRarityWeight(candidates[i].rarity);
            weights.Add(w);
        }

        if(history != null)
        {
            for (int i = 0; i < candidates.Count; ++i)
            {
                UpgradeData d = candidates[i];
                if(d.tags != null)
                {
                    for(int t=0; t<d.tags.Length; ++t)
                    {
                        string tag = d.tags[t];
                        if(tag == ruleSet.pityTag)
                        {
                            int absence = history.GetAbsenceLevelsForTag(tag);
                            if(absence >= ruleSet.pityAfterLevels)
                            {
                                weights[i] *= ruleSet.pityBoost;
                            }
                        }
                    }
                }
            }
        }

        List<UpgradeData> result = new List<UpgradeData>();
        HashSet<int> used = new HashSet<int>();

        int picks = Mathf.Min(3, candidates.Count);
        for(int k=0; k<picks; ++k)
        {
            int idx = WeightedPicker.PickIndexByWeight(weights, rng);
            if(idx < 0)
            {
                break;
            }

            if(used.Contains(idx) == true)
            {
                weights[idx] = 0.0f;
                --k;
                continue;
            }

            used.Add(idx);
            result.Add(candidates[idx]);

            weights[idx] = 0.0f;

            if(history != null)
            {
                history.RecordShown(candidates[idx]);
            }
        }

        return result;
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        if(data == null)
        {
            return;
        }

        int cur = GetLevel(data);
        if(cur >= data.maxLevel)
        {
            return;
        }

        int next = cur + 1;
        levels[data] = next;

        float value = data.GetValuePerLevel(next);

        if(data.effectType == UpgradeData.EffectType.ReduceAttackCooldown)
        {
            weaponManager.ReduceCooldownAll(value);
        }
        else if(data.effectType == UpgradeData.EffectType.IncreaseDamage)
        {
            weaponManager.AddDamageAll(value);
        }
        else if (data.effectType == UpgradeData.EffectType.IncreaseProjectileSpeed)
        {
            weaponManager.AddProjectileSpeedAll(value);
        }
        else if (data.effectType == UpgradeData.EffectType.IncreaseProjectiles)
        {
            weaponManager.AddProjectileCountAll((int)value);
        }

        if(history != null)
        {
            history.RecordPicked(data);
        }
    }

    public int GetLevel(UpgradeData data)
    {
        if(data != null)
        {
            if(levels.ContainsKey(data) == true)
            {
                return levels[data];
            }
        }

        return 0;
    }
}
