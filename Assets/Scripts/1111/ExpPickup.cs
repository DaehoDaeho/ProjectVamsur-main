using UnityEngine;

/// <summary>
/// 경험치 구슬. 플레이어가 트리거에 진입하면 지정된 경험치를 지급하고 사라진다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ExpPickup : MonoBehaviour
{
    [SerializeField]
    private float expAmount = 5.0f; // 지급할 경험치

    /// <summary>
    /// 플레이어가 트리거에 진입하면 경험치를 지급한다.
    /// </summary>
    /// <param name="other">진입한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        PlayerExperience px = other.GetComponent<PlayerExperience>();
        if (px != null)
        {
            px.AddExp(expAmount);
        }

        GameObject.Destroy(gameObject);
    }
}
