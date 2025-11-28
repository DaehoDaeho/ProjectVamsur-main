using UnityEngine;
using System.Collections;

/// <summary>
/// 예시: 패턴 실행 중 특정 타이밍에 스테이지 기믹을 켠다.
/// </summary>
public class PatternStageHook : AttackPatternBase
{
    [SerializeField]
    private ZoneWaveController zoneController;

    [SerializeField]
    private string triggerTag = "phase2";

    protected override IEnumerator Execute()
    {
        if (zoneController != null)
        {
            zoneController.TriggerTag(triggerTag);
        }

        // 패턴 자체는 간단 대기만 수행(예시)
        float hold = 0.8f;
        float t = 0.0f;
        while (t < hold)
        {
            t = t + Time.deltaTime;
            yield return null;
        }
    }
}
