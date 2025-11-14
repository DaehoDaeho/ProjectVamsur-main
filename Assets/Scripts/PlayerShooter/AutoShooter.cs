using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public float attackCooldown = 0.5f;
    public GameObject projectilePrefab;

    public float projectileSpeed = 10.0f;
    public float projectileLifeTime = 3.0f;
    public float damage = 5.0f;

    public float targetSearchRadius = 8.0f;
    public LayerMask targetLayerMask;
    public GameManager gameManager;

    public Transform firePoint;

    private float cooldownTimer;
    private float rotatingAngle;

    private void Awake()
    {
        cooldownTimer = 0.0f;
        rotatingAngle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        bool canFire = false;
        if(gameManager != null)
        {
            canFire = gameManager.IsPlaying();
        }

        if(canFire == false)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;
        if(cooldownTimer <= 0.0f)
        {
            FireOneShot();
            cooldownTimer = attackCooldown;
        }
    }

    void FireOneShot()
    {
        Vector3 spawnPos = firePoint.position;

        Vector2 dir = GetAimDirection();
        
        if(projectilePrefab != null)
        {
            GameObject obj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

            Projectile proj = obj.GetComponent<Projectile>();
            if(proj != null)
            {
                proj.SetDirection(dir);
                proj.Configure(projectileSpeed, projectileLifeTime, damage);
            }
        }
    }

    Vector2 GetAimDirection()
    {
        Transform nearest = FindNearestTarget();
        if(nearest != null)
        {
            Vector2 toTarget = nearest.position - transform.position;
            return toTarget.normalized;
        }

        rotatingAngle -= 90.0f * Time.deltaTime;
        float rad = rotatingAngle * Mathf.Deg2Rad;  // 각도를 라디안 값으로 변경.
        Vector2 fallback = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        return fallback.normalized;
    }

    Transform FindNearestTarget()
    {
        Collider2D[] hits;

        hits = Physics2D.OverlapCircleAll(transform.position, targetSearchRadius, targetLayerMask);
        if(hits == null || hits.Length == 0)
        {
            return null;
        }

        float bestDist = float.MaxValue;
        Transform best = null;

        for(int i=0; i<hits.Length; ++i)
        {
            Transform t = hits[i].transform;
            //float dist = (t.position - transform.position).sqrMagnitude; 
            float dist = Vector2.Distance(t.position, transform.position);
            if(dist < bestDist)
            {
                bestDist = dist;
                best = t;
            }
        }

        return best;
    }
}
