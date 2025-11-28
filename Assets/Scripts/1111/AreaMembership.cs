using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Trigger 2D ������� '���� �� ���ο� �ִ� Collider2D'�� �����Ѵ�.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AreaMembership : MonoBehaviour
{
    [SerializeField]
    private LayerMask affectMask;                 // ��� ���̾�

    private HashSet<Collider2D> inside = new HashSet<Collider2D>();

    public delegate void EnterDelegate(Collider2D col);
    public delegate void ExitDelegate(Collider2D col);

    public event EnterDelegate OnEntered;
    public event ExitDelegate OnExited;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        // HashSet을 초기화하지는 말고, 현재 트리거와 겹치는 대상들을 다시 채운다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter();
            List<Collider2D> results = new List<Collider2D>();

            int count = col.Overlap(filter, results);
            for (int i = 0; i < count; i = i + 1)
            {
                Collider2D c = results[i];
                if (c != null)
                {
                    if (IsInMask(c.gameObject.layer) == true)
                    {
                        if (inside.Contains(c) == false)
                        {
                            inside.Add(c);
                            if (OnEntered != null)
                            {
                                OnEntered(c);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInMask(other.gameObject.layer) == false)
        {
            return;
        }

        if (inside.Contains(other) == false)
        {
            inside.Add(other);

            if (OnEntered != null)
            {
                OnEntered(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (inside.Contains(other) == true)
        {
            inside.Remove(other);

            if (OnExited != null)
            {
                OnExited(other);
            }
        }
    }

    public IEnumerable<Collider2D> Enumerate()
    {
        return inside;
    }

    public void ClearAll()
    {
        inside.Clear();
    }

    private bool IsInMask(int layer)
    {
        int flag = 1 << layer;
        if ((affectMask.value & flag) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
