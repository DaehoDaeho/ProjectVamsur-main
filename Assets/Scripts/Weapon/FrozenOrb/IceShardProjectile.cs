using UnityEngine;

public class IceShardProjectile : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitCollider;

    private Vector2 moveDir;    // 이동 방향.
    private float moveSpeed;    // 이동 속력.
    private float lifeRemainSec;    // 남은 생존 시간.

    private float damage;   // 대미지.

    private LayerMask enemyLayer;

    public void Setup(Vector2 direction, float speed, float lifeSec, float dmg, LayerMask layer)
    {
        moveDir = direction.normalized;

        moveSpeed = speed;
        lifeRemainSec = lifeSec;
        damage = dmg;
        enemyLayer = layer;
    }

    void TryHit(Collider2D collision)
    {
        int mask = 1 << collision.gameObject.layer;
        if((mask & enemyLayer.value) == 0)
        {
            return;
        }

        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if(enemyHealth == null)
        {
            return;
        }

        enemyHealth.ApplyDamage(damage);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    // Update is called once per frame
    void Update()
    {
        lifeRemainSec -= Time.deltaTime;

        if(lifeRemainSec <= 0)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 delta = moveDir * moveSpeed * Time.deltaTime;
        transform.position += (Vector3)delta;
    }
}
