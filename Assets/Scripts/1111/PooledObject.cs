using UnityEngine;

/// <summary>
/// 풀에서 꺼내 쓰는 오브젝트에 부착한다.
/// 자신이 어느 풀에서 왔는지 기억하고, 수명/충돌/화면 이탈 시 풀로 되돌린다.
/// </summary>
public class PooledObject : MonoBehaviour
{
    [SerializeField]
    private float lifeTimeSeconds = 5.0f; // 수명. 0 이하면 무한

    private ObjectPool ownerPool;
    private float lifeTimer;

    /// <summary>
    /// 풀에서 나올 때 소유 풀과 초기화 시간을 설정한다.
    /// </summary>
    /// <param name="pool">자신을 관리하는 풀</param>
    public void OnSpawned(ObjectPool pool)
    {
        ownerPool = pool;
        lifeTimer = lifeTimeSeconds;
    }

    private void Update()
    {
        if (lifeTimeSeconds > 0.0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0.0f)
            {
                Release();
            }
        }
    }

    /// <summary>
    /// 외부에서 수명을 연장하거나 즉시 회수하도록 조정할 수 있다.
    /// </summary>
    /// <param name="seconds">새 수명(초)</param>
    public void SetLifeTime(float seconds)
    {
        lifeTimeSeconds = seconds;
        lifeTimer = seconds;
    }

    /// <summary>
    /// 충돌 시 회수하고 싶을 때 호출한다(필요 시 Projectile/Enemy에서 직접 호출).
    /// </summary>
    public void Release()
    {
        if (ownerPool != null)
        {
            ownerPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
