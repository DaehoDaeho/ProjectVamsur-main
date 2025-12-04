using UnityEngine;

/// <summary>
/// 입력에 따라 풀에서 탄환을 꺼내 스폰한다.
/// </summary>
public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField]
    private PoolManager poolManager;

    [SerializeField]
    private string projectileKey = "Projectile";

    [SerializeField]
    private Transform muzzle; // 발사 위치

    [SerializeField]
    private int burstCount = 3;

    [SerializeField]
    private float burstSpreadDeg = 8.0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            FireBurst();
        }
    }

    /// <summary>
    /// 지정 개수만큼 부채꼴로 발사한다.
    /// </summary>
    private void FireBurst()
    {
        if (poolManager == null)
        {
            return;
        }
        if (muzzle == null)
        {
            return;
        }

        float half = (burstCount - 1) * 0.5f;
        for (int i = 0; i < burstCount; i = i + 1)
        {
            float delta = (i - half) * burstSpreadDeg;
            Quaternion rot = muzzle.rotation * Quaternion.Euler(0.0f, 0.0f, delta);
            PooledObject obj = poolManager.Spawn(projectileKey, muzzle.position, rot);
            if (obj == null)
            {
                continue;
            }

            // 필요 시 탄속/수명 조정
            Projectile pr = obj.GetComponent<Projectile>();
            if (pr != null)
            {
                pr.SetSpeed(14.0f);
            }
        }
    }
}
