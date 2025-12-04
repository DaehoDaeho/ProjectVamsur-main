using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 단일 프리팹을 관리하는 오브젝트 풀.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private string poolKey = "Projectile"; // PoolManager에서 찾을 때 쓰는 키

    [SerializeField]
    private PooledObject prefab; // 반드시 PooledObject가 붙은 프리팹

    [SerializeField]
    private int warmCount = 30; // 시작 시 미리 만들어 둘 개수

    private readonly Queue<PooledObject> queue = new Queue<PooledObject>();
    private int totalCreated;

    /// <summary>
    /// PoolManager가 키로 이 풀을 등록할 때 사용한다.
    /// </summary>
    public string GetKey()
    {
        return poolKey;
    }

    private void Awake()
    {
        Warm(warmCount);
    }

    /// <summary>
    /// 초기 워밍업. 요청 개수만큼 만들어 비활성 큐에 넣는다.
    /// </summary>
    /// <param name="count">생성 개수</param>
    public void Warm(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            CreateOneInactive();
        }
    }

    /// <summary>
    /// 하나를 꺼낸다. 없으면 자동 확장한다.
    /// </summary>
    /// <returns>활성화된 PooledObject</returns>
    public PooledObject Spawn()
    {
        PooledObject obj = null;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = CreateOneInactive();
        }

        if (obj != null)
        {
            obj.gameObject.SetActive(true);
            obj.OnSpawned(this);
        }

        return obj;
    }

    /// <summary>
    /// 회수: 비활성화 후 큐에 넣는다.
    /// </summary>
    /// <param name="pooled">돌려보낼 오브젝트</param>
    public void Release(PooledObject pooled)
    {
        if (pooled == null)
        {
            return;
        }

        pooled.gameObject.SetActive(false);
        queue.Enqueue(pooled);
    }

    /// <summary>
    /// 현재 큐에 대기 중인 개수.
    /// </summary>
    public int GetAvailableCount()
    {
        return queue.Count;
    }

    /// <summary>
    /// 지금까지 실제로 만든 총 개수(진단용).
    /// </summary>
    public int GetTotalCreated()
    {
        return totalCreated;
    }

    private PooledObject CreateOneInactive()
    {
        if (prefab == null)
        {
            Debug.LogWarning("ObjectPool: prefab is null.");
            return null;
        }

        PooledObject obj = Instantiate(prefab, transform);
        ++totalCreated;
        obj.gameObject.SetActive(false);
        queue.Enqueue(obj);
        return obj;
    }
}
