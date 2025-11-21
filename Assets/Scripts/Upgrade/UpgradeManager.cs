using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeData> upgradePool = new List<UpgradeData>();

    private Dictionary<UpgradeData, int> levels = new Dictionary<UpgradeData, int>();

    [SerializeField]
    private AutoShooter autoShooter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<UpgradeData> DrawThree()
    {
        List<UpgradeData> candidates = new List<UpgradeData>();

        for(int i=0; i<upgradePool.Count; ++i)
        {
            UpgradeData data = upgradePool[i];
            int cur = GetLevel(data);
            if(cur < data.maxLevel)
            {
                candidates.Add(data);
            }
        }

        List<UpgradeData> result = new List<UpgradeData>();
        if(candidates.Count <= 3)
        {
            result.AddRange(candidates);
            return result;
        }

        for(int i=0; i<3; ++i)
        {
            int index = Random.Range(0, candidates.Count);
            result.Add(candidates[index]);
            candidates.RemoveAt(index);
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
            if(autoShooter != null)
            {
                float current = autoShooter.GetAttackCooldown();
                float newValue = Mathf.Max(0.05f, current - value);
                autoShooter.SetAttackCooldown(newValue);
            }
        }
        else if(data.effectType == UpgradeData.EffectType.IncreaseDamage)
        {
            if (autoShooter != null)
            {
                float current = autoShooter.GetDamage();
                float newValue = current + value;
                autoShooter.SetDamage(newValue);
            }
        }
        else if (data.effectType == UpgradeData.EffectType.IncreaseProjectileSpeed)
        {
            if (autoShooter != null)
            {
                float current = autoShooter.GetProjectileSpeed();
                float newValue = current + value;
                autoShooter.SetProjectileSpeed(newValue);
            }
        }
        else if (data.effectType == UpgradeData.EffectType.IncreaseProjectiles)
        {
            if (autoShooter != null)
            {
                int current = autoShooter.GetProjectileCount();
                int add = Mathf.RoundToInt(value);
                int newValue = Mathf.Max(1, current + add);
                autoShooter.SetProjectileCount(newValue);
            }
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
