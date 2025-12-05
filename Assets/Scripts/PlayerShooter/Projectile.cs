using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float lifeTime = 3.0f;
    public float damage = 5.0f;

    private Vector2 moveDirection;

    private float elapsed;

    // �ű�: ���� �Ķ����
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

    private PooledObject pooled;

    private void Awake()
    {
        pooled = GetComponent<PooledObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        transform.position += delta;
    }

    public void SetDirection(Vector2 dir)
    {
        if(dir.sqrMagnitude > 0.0001f)
        {
            moveDirection = dir.normalized;
        }
        else
        {
            // ������ (0, 0)�� ��쿡�� ����Ʈ�� ������ �������� ����
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

            if (pooled != null)
            {
                pooled.Release();
                return;
            }
            
            Destroy(gameObject);
            return;
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

        if (pooled != null)
        {
            pooled.Release();
            return;
        }

        Destroy(gameObject);
    }

    // --- �ű� Setter��(���⿡�� ����) ---
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

    /// <summary>
    /// 대미지를 반환
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// 속도를 세팅
    /// </summary>
    public void SetSpeed(float v)
    {
        moveSpeed = v;
    }
}
