using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 업그레이드 드래프트를 위한 풀/가중치 롤을 관리한다.
/// 진행 시간에 따라 희귀도 가중치를 조정할 수 있다.
/// 선별 후보에서 희귀도/가중치로 선별 -> 최디 레벨인 항목 제외 -> 중복을 포함해서 N개 뽑기 (지금 당장은 3개)
/// </summary>
public class UpgradePool : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeDefinition> allDefs = new List<UpgradeDefinition>(); // 전체 업그레이드 목록

    [Header("Base Weights")]
    [SerializeField]
    private int weightCommon = 60;

    [SerializeField]
    private int weightUncommon = 28;

    [SerializeField]
    private int weightRare = 10;

    [SerializeField]
    private int weightEpic = 2;

    [Header("Progress Scaling")]
    [SerializeField]
    private float rareBonusPerMinute = 1.0f; // 분당 Rare 가중치 증가

    [SerializeField]
    private float epicBonusPerMinute = 0.3f; // 분당 Epic 가중치 증가

    /// <summary>
    /// 드래프트 옵션을 뽑는다.
    /// </summary>
    /// <param name="count">옵션 개수</param>
    /// <param name="state">런타임 상태(최대레벨 제외에 사용)</param>
    /// <param name="elapsedSeconds">세션 경과 시간(희귀도 가중치 보정용)</param>
    /// <returns>선택된 업그레이드 정의 리스트</returns>
    /// <remarks>
    /// 부작용 없음.
    /// 예외: 정의가 비어 있으면 빈 리스트 반환.
    /// 성능: count가 작을 때 O(count + N) 수준(간단 가중치 롤).
    /// </remarks>
    public List<UpgradeDefinition> RollOptions(int count, UpgradeRuntimeState state, float elapsedSeconds)
    {
        List<UpgradeDefinition> result = new List<UpgradeDefinition>();
        if (allDefs == null || allDefs.Count == 0)
        {
            return result;
        }

        // 진행도 기반 가중치 보정
        float minutes = Mathf.Max(0.0f, elapsedSeconds / 60.0f);
        float wr = weightRare + minutes * rareBonusPerMinute;
        float we = weightEpic + minutes * epicBonusPerMinute;

        // 안전한 정수화
        int wCommon = Mathf.Max(1, weightCommon);
        int wUncommon = Mathf.Max(1, weightUncommon);
        int wRare = Mathf.Max(1, Mathf.RoundToInt(wr));
        int wEpic = Mathf.Max(1, Mathf.RoundToInt(we));

        for (int k = 0; k < count; ++k)
        {
            // 1) 희귀도 선택
            UpgradeRarity pickedRarity = PickRarity(wCommon, wUncommon, wRare, wEpic);

            // 2) 해당 희귀도의 후보 중 선택(최대레벨 제외)
            UpgradeDefinition picked = PickOneByRarity(pickedRarity, state);

            // 2-보정) 만약 해당 희귀도에서 모두 Maxed라면, 낮은 희귀도로 단계적 강등 시도
            if (picked == null)
            {
                picked = TryFallbackPick(state, pickedRarity);
            }

            if (picked != null)
            {
                result.Add(picked);
            }
        }

        return result;
    }

    /// <summary>
    /// 희귀도 가중치에 따라 하나를 선택한다.
    /// </summary>
    private UpgradeRarity PickRarity(int wC, int wU, int wR, int wE)
    {
        int sum = wC + wU + wR + wE;
        int r = Random.Range(0, sum);
        int cur = 0;

        cur += wC;
        if (r < cur)
        {
            return UpgradeRarity.Common;
        }

        cur += wU;
        if (r < cur)
        {
            return UpgradeRarity.Uncommon;
        }

        cur += wR;
        if (r < cur)
        {
            return UpgradeRarity.Rare;
        }

        return UpgradeRarity.Epic;
    }

    /// <summary>
    /// 특정 희귀도에서 아직 최대레벨이 아닌 항목 하나를 랜덤으로 선택한다.
    /// </summary>
    private UpgradeDefinition PickOneByRarity(UpgradeRarity rarity, UpgradeRuntimeState state)
    {
        List<UpgradeDefinition> candidates = new List<UpgradeDefinition>();
        for (int i = 0; i < allDefs.Count; ++i)
        {
            UpgradeDefinition d = allDefs[i];
            if (d == null)
            {
                continue;
            }

            if (d.GetRarity() != rarity)
            {
                continue;
            }

            if (state != null)
            {
                if (state.IsMaxed(d) == true)
                {
                    continue;
                }
            }

            candidates.Add(d);
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    /// <summary>
    /// 선택한 희귀도에서 실패했을 때, 한 단계씩 낮추며 대안을 찾는다.
    /// </summary>
    private UpgradeDefinition TryFallbackPick(UpgradeRuntimeState state, UpgradeRarity start)
    {
        // Rare 실패 시 Uncommon → Common 순
        // Epic 실패 시 Rare → Uncommon → Common 순
        UpgradeRarity[] orderEpic = { UpgradeRarity.Rare, UpgradeRarity.Uncommon, UpgradeRarity.Common };
        UpgradeRarity[] orderRare = { UpgradeRarity.Uncommon, UpgradeRarity.Common };
        UpgradeRarity[] orderUncommon = { UpgradeRarity.Common };
        UpgradeRarity[] none = { };

        UpgradeRarity[] order = none;
        if (start == UpgradeRarity.Epic)
        {
            order = orderEpic;
        }
        else if (start == UpgradeRarity.Rare)
        {
            order = orderRare;
        }
        else if (start == UpgradeRarity.Uncommon)
        {
            order = orderUncommon;
        }
        else
        {
            order = none;
        }

        for (int i = 0; i < order.Length; ++i)
        {
            UpgradeDefinition d = PickOneByRarity(order[i], state);
            if (d != null)
            {
                return d;
            }
        }

        // 모든 희귀도가 막혔다면 null
        return null;
    }
}
