using UnityEngine;

/// <summary>
/// 체력을 가진 피격 대상의 예시 구현
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageableActor : MonoBehaviour, IDamageable, IReceivesHitContext
{
    [SerializeField]
    private float maxHp = 50.0f; // 최대 체력

    [SerializeField, Range(0.0f, 0.9f)]
    private float resistance = 0.1f; // 피해 저항

    private float hp; // 현재 체력
    private bool isDead; // 사망 상태

    private bool hasLastContext; // 최근 문맥 보유 여부
    private HitContext lastContext; // 최근 문맥 저장

    private void OnEnable()
    {
        hp = maxHp; // 체력 초기화
        isDead = false; // 사망 상태 초기화
        hasLastContext = false; // 문맥 보유 초기화
    }

    /// <summary>
    /// 다음 피해 적용에서 사용할 히트 문맥을 저장한다
    /// </summary>
    public void SetHitContext(HitContext context)
    {
        lastContext = context; // 문맥 저장
        hasLastContext = true; // 보유 표시
    }

    /// <summary>
    /// 실수형 피해를 적용하고 알림을 발행한다
    /// </summary>
    public void ApplyDamage(float damage)
    {
        if (isDead == true)
        {
            return;
        }

        HitContext context; // 사용할 문맥
        if (hasLastContext == true)
        {
            context = lastContext; // 최근 문맥 사용
            hasLastContext = false; // 사용 후 해제
        }
        else
        {
            context = HitContext.Create(null, transform, transform.position, damage, DamageType.Physical); // 기본 문맥 합성
        }

        float finalDamage = DamageResolver.ComputeFinalDamage(context, resistance, damage); // 최종 피해 계산

        hp -= finalDamage; // 체력 감소
        if (hp < 0.0f)
        {
            hp = 0.0f; // 음수 방지
        }

        DamageAppliedEvent e = new DamageAppliedEvent(); // 피해확정 알림 데이터
        e.context = context; // 문맥
        e.finalDamage = finalDamage; // 최종 피해
        e.remainingHp = hp; // 남은 체력
        EventBus.PublishDamageApplied(e); // 피해확정 알림 발행

        if (hp <= 0.0f)
        {
            if (isDead == false)
            {
                isDead = true; // 사망 표시

                DeathEvent de = new DeathEvent(); // 사망 알림 데이터
                de.context = context; // 문맥
                de.victim = transform; // 희생자 변환
                EventBus.PublishDeath(de); // 사망 알림 발행
            }
        }
    }
}
