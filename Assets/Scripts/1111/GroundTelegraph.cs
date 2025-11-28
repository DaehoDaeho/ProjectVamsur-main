using UnityEngine;

/// <summary>
/// 지면 텔레그래프를 표시/숨김하고, 간단한 펄스 애니메이션을 제공한다.
/// 스프라이트는 Circle/Box 등 형태 스프라이트를 사용한다.
/// </summary>
public class GroundTelegraph : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;      // 시각 표시

    [SerializeField]
    private Color idleColor = new Color(1.0f, 0.4f, 0.2f, 0.25f); // 기본 반투명

    [SerializeField]
    private Color warnColor = new Color(1.0f, 0.2f, 0.1f, 0.6f);  // 경고 강색

    [SerializeField]
    private float pulseSpeed = 2.0f;           // 펄스 속도

    private float pulseTime;
    private bool isVisible;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        Hide();
    }

    private void Update()
    {
        if (isVisible == true)
        {
            pulseTime = pulseTime + Time.unscaledDeltaTime * pulseSpeed;

            float s = 1.0f + Mathf.Sin(pulseTime) * 0.05f; // 5% 스케일 펄스
            transform.localScale = new Vector3(s, s, 1.0f);

            if (spriteRenderer != null)
            {
                float a = 0.5f + Mathf.Sin(pulseTime) * 0.25f; // 알파 펄스
                Color c = spriteRenderer.color;
                c.a = Mathf.Clamp01(a);
                spriteRenderer.color = c;
            }
        }
    }

    public void ShowIdle()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = idleColor;
        }
        isVisible = true;
        pulseTime = 0.0f;
        SetRenderer(true);
    }

    public void ShowWarn()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = warnColor;
        }
        isVisible = true;
        pulseTime = 0.0f;
        SetRenderer(true);
    }

    public void Hide()
    {
        isVisible = false;
        SetRenderer(false);
    }

    private void SetRenderer(bool enabled)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = enabled;
        }
    }
}
