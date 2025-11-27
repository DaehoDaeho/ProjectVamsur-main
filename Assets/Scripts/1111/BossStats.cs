using UnityEngine;

/// <summary>
/// 보스용 수치/면역/저항 값 저장소.
/// 다른 시스템이 이 값을 읽어 밸런싱한다.
/// </summary>
public class BossStats : MonoBehaviour
{
    [Header("Multipliers")]
    [SerializeField]
    private float healthMultiplier = 8.0f;       // 최대 체력 배수

    [SerializeField]
    private float damageTakenMultiplier = 1.0f;  // 받는 피해 배수(난이도 튜닝)

    [Header("Immunities")]
    [SerializeField]
    private bool immuneBurn = false;

    [SerializeField]
    private bool immuneFreeze = false;

    [Header("Resistances (0~1)")]
    [SerializeField]
    private float burnResist = 0.2f;

    [SerializeField]
    private float freezeResist = 0.2f;

    private void Start()
    {
        EnemyHealth hp = GetComponent<EnemyHealth>();
        if (hp != null)
        {
            float max = hp.GetMaxHealth();
            if (max > 0.0f)
            {
                hp.SetMaxHealth(max * healthMultiplier);
            }
        }
    }

    public bool IsImmuneBurn()
    {
        return immuneBurn;
    }

    public bool IsImmuneFreeze()
    {
        return immuneFreeze;
    }

    public float GetBurnResist()
    {
        return Mathf.Clamp01(burnResist);
    }

    public float GetFreezeResist()
    {
        return Mathf.Clamp01(freezeResist);
    }

    public float GetDamageTakenMultiplier()
    {
        return Mathf.Max(0.0f, damageTakenMultiplier);
    }
}
