using UnityEngine;

public class FrozenOrbCore : MonoBehaviour
{
    [SerializeField]
    private GameObject shardPrefab;

    [SerializeField]
    private float moveSpeed = 6.5f;

    [SerializeField]
    private float maxDistance = 7.0f;

    [SerializeField]
    private float lifeTimeSec = 2.0f;

    [SerializeField]
    private float shardIntervalSec = 0.15f;

    [SerializeField]
    private int shardBurstCount = 20;

    [SerializeField]
    private int shardPerTick = 4;

    [SerializeField]
    private float shardDamage = 3.0f;

    [SerializeField]
    private float shardSpeed = 12.0f;

    [SerializeField]
    private float shardLifeSec = 1.0f;

    [SerializeField]
    private LayerMask enemyLayer;

    private Transform owner;
    private Vector2 moveDir;

    private Vector2 startPos;
    private float liveSec;
    private float shardTimer;

    public void Setup(Transform ownerTransform, Vector2 direction)
    {
        owner = ownerTransform;

        moveDir = direction.normalized;

        startPos = transform.position;
        liveSec = 0.0f;
        shardTimer = 0.0f;
    }

    /// <summary>
    /// 각도를 방향 벡터로 바꾼다.
    /// </summary>
    /// <param name="deg">각도</param>
    /// <returns></returns>
    Vector2 DegToDir(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        Vector2 v = new Vector2(x, y);

        return v.normalized;
    }

    void SpawnOneShard(Vector2 dir)
    {
        Vector3 pos = transform.position;

        GameObject obj = Instantiate(shardPrefab, pos, Quaternion.identity);
        if(obj == null)
        {
            return;
        }

        IceShardProjectile shard = obj.GetComponent<IceShardProjectile>();
        if(shard == null)
        {
            Destroy(obj);
            return;
        }

        shard.Setup(dir, shardSpeed, shardLifeSec, shardDamage, enemyLayer);
    }

    void SpawnRadialShards(int count)
    {
        if(shardPrefab == null)
        {
            return;
        }

        float stepDeg = 360.0f / count;
        float startDeg = Random.Range(0.0f, stepDeg);

        for(int i=0; i<count; ++i)
        {
            float deg = startDeg + (stepDeg * i);
            Vector2 dir = DegToDir(deg);

            SpawnOneShard(dir);
        }
    }

    void FinalBurst()
    {
        SpawnRadialShards(shardBurstCount);
    }

    bool IsFinished()
    {
        if(liveSec >= lifeTimeSec)
        {
            return true;
        }

        Vector2 curPos = transform.position;
        Vector2 diff = curPos - startPos;

        float travelSqr = diff.sqrMagnitude;
        float maxSqr = maxDistance * maxDistance;

        if(travelSqr >= maxSqr)
        {
            return true;
        }

        return false;
    }

    void UpdateShardTick()
    {
        shardTimer += Time.deltaTime;

        if(shardTimer >= shardIntervalSec)
        {
            shardTimer -= shardIntervalSec;
            SpawnRadialShards(shardPerTick);
        }
    }

    void MoveForward()
    {
        Vector2 delta = moveDir * moveSpeed * Time.deltaTime;
        transform.position += (Vector3)delta;
    }

    // Update is called once per frame
    void Update()
    {
        liveSec += Time.deltaTime;

        MoveForward();
        UpdateShardTick();

        if(IsFinished() == true)
        {
            FinalBurst();
            Destroy(gameObject);
        }
    }
}
