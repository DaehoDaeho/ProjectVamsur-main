using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float lifeTime = 3.0f;
    public float damage = 5.0f;

    private Vector2 moveDirection;

    private float elapsed;

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        transform.position += delta;

        elapsed += Time.deltaTime;
        if(elapsed >= lifeTime)
        {
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
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.ApplyDamage(damage);
        }

        Destroy(gameObject);
    }
}
