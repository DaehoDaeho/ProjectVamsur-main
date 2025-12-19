using UnityEngine;

/// <summary>
/// 플레이어에게 접근해 일정 반경에서 퓨즈 시동 후 폭발하는 적 클래스.
/// 이동 -> 퓨즈 시동 -> 폭발 -> 범위 대미지 적용
/// </summary>
public class SuicideBomber : MonoBehaviour
{
    [SerializeField]
    private Transform player;   // 플레이어.

    [SerializeField]
    private float moveSpeed = 3.0f; // 이동 속도.

    [SerializeField]
    private float detectDistance = 10.0f;   // 감지 가능 거리 범위.

    [SerializeField]
    private float fuseRadius = 2.5f;    // 퓨즈 시동 가능 거리.

    [SerializeField]
    private float fuseSeconds = 1.0f;   // 퓨즈 유지 시간.

    [SerializeField]
    private float explodeRadius = 3.0f; // 폭발 적용 반경.

    [SerializeField]
    private float explosionDamage = 20.0f;  // 폭발 대미지.

    [SerializeField]
    private LayerMask targetLayer;  // 대미지를 적용할 오브젝트의 레이어.

    private bool fuseArmed; // 현재 퓨즈 시동 중인지 여부.
    private float fuseRemain;    // 퓨즈 타이머.

    private bool exploded;  // 폭발 했는지 여부.

    private void Awake()
    {
        fuseArmed = false;
        fuseRemain = 0.0f;
        exploded = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }

        if(exploded == true)
        {
            return;
        }

        Vector3 myPos = transform.position;
        Vector3 toPlayer = player.position - myPos;
        float dist = toPlayer.magnitude;    // 플레이어와의 현재 거리.

        if(dist <= detectDistance && fuseArmed == false)
        {
            // 아직 퓨즈 시동이 가능한 거리보다 멀면 플레이어를 향해 이동.
            if(dist > fuseRadius)
            {
                Vector3 dir = toPlayer.normalized;
                float step = moveSpeed * Time.deltaTime;
                transform.position = myPos + dir * step;
            }
            else
            {
                ArmFuse();
            }
        }

        if(fuseArmed == true)
        {
            fuseRemain -= Time.deltaTime;
            if(fuseRemain <= 0.0f)
            {
                Explode();  // 퓨즈 시동 후 시간이 다 되면 폭발.
            }
        }

        if(toPlayer.sqrMagnitude > 0.0001f)
        {
            transform.up = Vector3.Slerp(transform.up, toPlayer.normalized, 10.0f * Time.deltaTime);
        }
    }

    /// <summary>
    /// 퓨즈를 시동하고 카운트다운 시작.
    /// </summary>
    void ArmFuse()
    {
        if(fuseArmed == true)
        {
            return;
        }

        fuseArmed = true;
        fuseRemain = fuseSeconds;
    }

    /// <summary>
    /// 폭발.
    /// </summary>
    void Explode()
    {
        if(exploded == true)
        {
            return;
        }

        exploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);
        if(hits != null)
        {
            for(int i=0; i<hits.Length; ++i)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                IDamageable damageable = hits[i].GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.ApplyDamage(explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // 폭발 반경.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fuseRadius);
    }
}
