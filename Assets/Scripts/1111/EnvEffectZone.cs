using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 스테이지 존에서 환경 효과를 적용한다(대미지/힐/슬로우/점화 등).
/// - mode == Continuous: tickInterval마다 내부 대상에 효과 적용
/// - mode == Pulse: preTelegraph -> activeWindow 동안만 효과 적용, 이후 cooldown
/// </summary>
[RequireComponent(typeof(AreaMembership))]
public class EnvEffectZone : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private GroundTelegraph telegraph;          // 지면 경고(선택)

    [SerializeField]
    private bool affectPlayer = true;           // 플레이어에 영향

    [SerializeField]
    private bool affectEnemy = false;           // 적에 영향

    [SerializeField]
    private float tickInterval = 1.0f;          // 연속형 틱 주기

    [Header("Effects")]
    [SerializeField]
    private float damagePerTick = 8.0f;         // 틱당 피해(0이면 미사용)

    [SerializeField]
    private float healPerTick = 0.0f;           // 틱당 회복(0이면 미사용)

    [SerializeField]
    private float slowPercent = 0.0f;           // 0~1, 0.4=40% 슬로우

    [SerializeField]
    private bool applyBurn = false;             // 점화 적용 여부

    [SerializeField]
    private float burnDps = 0.0f;               // 점화 DpS

    [SerializeField]
    private float burnDuration = 0.0f;          // 점화 지속

    private AreaMembership membership;
    private GameManager gameManager;

    // 타이머
    private float timer;

    private void Awake()
    {
        membership = GetComponent<AreaMembership>();
        gameManager = FindAnyObjectByType<GameManager>();

        if (telegraph == null)
        {
            telegraph = GetComponentInChildren<GroundTelegraph>();
        }

        timer = 0.0f;

        if (telegraph != null)
        {
            telegraph.ShowIdle();
        }
    }

    private void Update()
    {
        if (gameManager != null)
        {
            if (gameManager.IsPlaying() == false)
            {
                return;
            }
        }

        UpdateContinuous();
    }

    private void UpdateContinuous()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            ApplyEffectsToAll();
            timer = Mathf.Max(0.05f, tickInterval);
        }
    }

    private void ApplyEffectsToAll()
    {
        foreach (Collider2D col in membership.Enumerate())
        {
            if (col == null)
            {
                continue;
            }

            if (affectPlayer == false)
            {
                if (IsPlayer(col) == true)
                {
                    continue;
                }
            }

            if (affectEnemy == false)
            {
                if (IsEnemy(col) == true)
                {
                    continue;
                }
            }

            ApplyOne(col);
        }
    }

    private void ApplyOne(Collider2D col)
    {
        // 1) 피해
        if (damagePerTick > 0.0f)
        {
            DamageRouter router = col.GetComponent<DamageRouter>();
            if (router != null)
            {
                DamageContext ctx = new DamageContext();
                ctx.baseDamage = damagePerTick;
                ctx.canCrit = false;
                ctx.knockbackForce = 0.0f;
                ctx.attacker = gameObject;

                // 상태이상 페이로드(점화)
                if (applyBurn == true)
                {
                    ctx.applyBurn = true;
                    ctx.burnDps = burnDps;
                    ctx.burnDuration = burnDuration;
                }

                router.Receive(ctx);
            }
        }

        // 2) 힐
        if (healPerTick > 0.0f)
        {
            // 힐은 Router가 아니라 Health API로 직접 적용(프로젝트 규칙에 따라 Router로 음수 대미지도 가능)
            PlayerHealth ph = col.GetComponent<PlayerHealth>();
            EnemyHealth eh = col.GetComponent<EnemyHealth>();

            if (ph != null)
            {
                ph.Heal(healPerTick);
            }
            else if (eh != null)
            {
                eh.Heal(healPerTick);
            }
        }

        // 3) 슬로우/빙결성
        if (slowPercent > 0.0f)
        {
            StatusEffectHost host = col.GetComponent<StatusEffectHost>();
            if (host != null)
            {
                // 빙결 느낌의 슬로우 1초 기본 제공(지속은 존 설정에 따라 확장 가능)
                host.ApplyFreeze(slowPercent, 1.0f);
            }
        }
    }

    private bool IsPlayer(Collider2D col)
    {
        GameObject go = col.gameObject;
        if (go.CompareTag("Player") == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsEnemy(Collider2D col)
    {
        // 단순 판정: EnemyHealth 존재여부
        EnemyHealth eh = col.GetComponent<EnemyHealth>();
        if (eh != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
