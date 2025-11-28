using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 여러 EnvEffectZone의 등장/퇴장을 시간축으로 제어한다.
/// - 시작 후 자동 재생(startOnPlay)
/// - 외부에서 Tag로 트리거(예: 보스 패턴이 phase2를 시작할 때)
/// </summary>
public class ZoneWaveController : MonoBehaviour
{
    [System.Serializable]
    private class Entry
    {
        public string tag;                // 트리거 식별자(선택)
        public EnvEffectZone zone;        // 대상 존
        public float startDelay = 0.0f;   // 시작까지 지연
        public float duration = 5.0f;     // 유지 시간
        public bool loop = false;         // 반복 여부
        public float loopCooldown = 2.0f; // 반복 대기
    }

    [SerializeField]
    private bool startOnPlay = true;

    [SerializeField]
    private List<Entry> entries = new List<Entry>();

    private GameManager gameManager;
    private Dictionary<string, List<Entry>> tagMap = new Dictionary<string, List<Entry>>();

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // 태그 맵 구성
        for (int i = 0; i < entries.Count; i = i + 1)
        {
            Entry e = entries[i];
            if (e == null || e.zone == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(e.tag) == false)
            {
                if (tagMap.ContainsKey(e.tag) == false)
                {
                    tagMap.Add(e.tag, new List<Entry>());
                }
                tagMap[e.tag].Add(e);
            }

            e.zone.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (startOnPlay == true)
        {
            for (int i = 0; i < entries.Count; i = i + 1)
            {
                Entry e = entries[i];
                if (e == null || e.zone == null)
                {
                    continue;
                }
                StartCoroutine(RunEntry(e));
            }
        }
    }

    public void TriggerTag(string tag)
    {
        if (string.IsNullOrEmpty(tag) == true)
        {
            return;
        }

        if (tagMap.ContainsKey(tag) == false)
        {
            return;
        }

        List<Entry> list = tagMap[tag];
        for (int i = 0; i < list.Count; i = i + 1)
        {
            StartCoroutine(RunEntry(list[i]));
        }
    }

    private IEnumerator RunEntry(Entry e)
    {
        // 지연
        float t = e.startDelay;
        while (t > 0.0f)
        {
            if (gameManager != null)
            {
                if (gameManager.IsPlaying() == false)
                {
                    yield return null;
                    continue;
                }
            }
            t = t - Time.deltaTime;
            yield return null;
        }

        do
        {
            // 활성
            if (e.zone != null)
            {
                e.zone.gameObject.SetActive(true);
            }

            // 유지
            float d = e.duration;
            while (d > 0.0f)
            {
                if (gameManager != null)
                {
                    if (gameManager.IsPlaying() == false)
                    {
                        yield return null;
                        continue;
                    }
                }
                d = d - Time.deltaTime;
                yield return null;
            }

            // 비활성
            if (e.zone != null)
            {
                e.zone.gameObject.SetActive(false);
            }

            if (e.loop == true)
            {
                float c = e.loopCooldown;
                while (c > 0.0f)
                {
                    if (gameManager != null)
                    {
                        if (gameManager.IsPlaying() == false)
                        {
                            yield return null;
                            continue;
                        }
                    }
                    c = c - Time.deltaTime;
                    yield return null;
                }
            }

        } while (e.loop == true);
    }
}
