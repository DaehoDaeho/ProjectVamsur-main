using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float lifeTime = 3.0f;
    public float damage = 5.0f;

    private Vector2 moveDirection;

    private float elapsed;

    // 신규: 전투 파라미터
    public bool canCrit;
    public float critChance;
    public float critMultiplier = 2.0f;
    public float knockbackForce;
    public bool applyBurn;
    public float burnDps;
    public float burnDuration;
    public bool applyFreeze;
    public float freezeSlowPercent;
    public float freezeDuration;
    public GameObject owner;

    //======================================================
    private PooledObject pooled;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();
    }
    //======================================================

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        transform.position += delta;

        elapsed += Time.deltaTime;
        if(elapsed >= lifeTime)
        {
            //=====================================================
            if (pooled != null)
            {
                pooled.Release();
                return;
            }
            //=====================================================

            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        if(dir.sqrMagnitude > 0.0001f)
        {
            moveDirection = dir.normalized;
        }
        else
        {
            // 방향이 (0, 0)일 경우에는 디폴트로 오른쪽 방향으로 설정
            moveDirection = Vector2.right;
        }
    }

    public void Configure(float speed, float time, float dmg)
    {
        moveSpeed = speed;
        lifeTime = time;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") == false)
        {
            return;
        }

        // 자기 편 필터 등은 생략(프로젝트 규칙에 맞게 추가 가능)
        // 우선 Router를 찾는다.
        DamageRouter router = collision.GetComponent<DamageRouter>();
        if (router != null)
        {
            DamageContext ctx = new DamageContext();
            ctx.baseDamage = damage;
            ctx.canCrit = canCrit;
            ctx.critChance = critChance;
            ctx.critMultiplier = critMultiplier;
            ctx.knockbackForce = knockbackForce;
            ctx.applyBurn = applyBurn;
            ctx.burnDps = burnDps;
            ctx.burnDuration = burnDuration;
            ctx.applyFreeze = applyFreeze;
            ctx.freezeSlowPercent = freezeSlowPercent;
            ctx.freezeDuration = freezeDuration;
            ctx.attacker = owner != null ? owner : gameObject;

            router.Receive(ctx);

            //=====================================================
            if (pooled != null)
            {
                pooled.Release();
                return;
            }
            //=====================================================
            Destroy(gameObject);
            return;
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

        //=====================================================
        if (pooled != null)
        {
            pooled.Release();
            return;
        }
        //=====================================================

        Destroy(gameObject);
    }

    // --- 신규 Setter들(무기에서 주입) ---
    public void SetOwner(GameObject obj)
    {
        owner = obj;
    }

    public void SetCrit(bool enable, float chance, float multiplier)
    {
        canCrit = enable;
        critChance = Mathf.Clamp01(chance);
        critMultiplier = Mathf.Max(1.0f, multiplier);
    }

    public void SetKnockback(float force)
    {
        knockbackForce = Mathf.Max(0.0f, force);
    }

    public void SetBurn(float dps, float duration)
    {
        if (dps > 0.0f && duration > 0.0f)
        {
            applyBurn = true;
            burnDps = dps;
            burnDuration = duration;
        }
    }

    public void SetFreeze(float slowPercent, float duration)
    {
        if (slowPercent > 0.0f && duration > 0.0f)
        {
            applyFreeze = true;
            freezeSlowPercent = Mathf.Clamp01(slowPercent);
            freezeDuration = duration;
        }
    }

    //===================================================
    /// <summary>
    /// 외부에서 데미지를 읽고 싶을 때 사용.
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// 탄 속도를 동적으로 조정하고 싶을 때 사용.
    /// </summary>
    public void SetSpeed(float v)
    {
        moveSpeed = v;
    }
    //===================================================
}
