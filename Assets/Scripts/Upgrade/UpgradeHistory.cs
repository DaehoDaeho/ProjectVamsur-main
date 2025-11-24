using UnityEngine;
using System.Collections.Generic;

public class UpgradeHistory : MonoBehaviour
{
    [SerializeField]
    private int recentCapacity = 8; // 최근 표시/선택 기록 버퍼 크기

    private Queue<string> recentShowGroups = new Queue<string>();   // 표시된 그룹 id
    private Queue<string> recentPickedGroups = new Queue<string>(); // 선택된 그룹 id

    private Dictionary<string, int> tagAbsenceLevels = new Dictionary<string, int>();   // 태그 별 연속 미출현 카운트

    public int GetAbsenceLevelsForTag(string tag)
    {
        if (tagAbsenceLevels.ContainsKey(tag) == true)
        {
            return tagAbsenceLevels[tag];
        }

        return 0;
    }

    public int GetRecentPickedCountOfGroup(string groupId, int lookback)
    {
        if (lookback <= 0)
        {
            return 0;
        }

        string id = string.IsNullOrEmpty(groupId) == true ? "-" : groupId;

        int count = 0;
        string[] arr = recentPickedGroups.ToArray();
        for (int i = arr.Length - 1; i >= 0 && lookback > 0; --i)
        {
            if (arr[i] == id)
            {
                ++count;
            }
            --lookback;
        }

        return count;
    }

    /// <summary>
    /// 레벨업 1회가 지나갈 때마다 호출하여 '미출현' 카운트를 증가시킨다
    /// 해당 레벨에서 어떤 태그가 표시되면 RecordShown 함수에서 0으로 리셋된다
    /// </summary>
    public void TickAbsenceForAllTags(HashSet<string> observedTags)
    {
        List<string> keys = new List<string>(tagAbsenceLevels.Keys);
        for(int i=0; i<keys.Count; ++i)
        {
            string k = keys[i];
            if(observedTags.Contains(k) == false)
            {
                ++tagAbsenceLevels[k];
            }
        }
    }

    void EnqueueGroup(Queue<string> q, string groupId)
    {
        string id = string.IsNullOrEmpty(groupId) == true ? "-" : groupId;
        q.Enqueue(id);  // 큐에 데이터를 추가
        while(q.Count > recentCapacity)
        {
            q.Dequeue();    // 큐에서 데이터를 꺼내고 삭제
        }
    }

    public void RecordPicked(UpgradeData data)
    {
        if(data != null)
        {
            EnqueueGroup(recentPickedGroups, data.groupId);
        }
    }

    public void RecordShown(UpgradeData data)
    {
        if(data != null)
        {
            EnqueueGroup(recentShowGroups, data.groupId);
            if(data.tags != null)
            {
                for(int i=0; i<data.tags.Length; ++i)
                {
                    string t = data.tags[i];
                    tagAbsenceLevels[t] = 0;
                }
            }
        }
    }
}
