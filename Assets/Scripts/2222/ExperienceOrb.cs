using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    [SerializeField]
    private float amount = 1.0f;

    [SerializeField]
    private float moveSpeedWhenMagnet = 9.0f;

    private Transform targetPlayer;
    private bool isMagnetized;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMagnetized == true && targetPlayer != null)
        {
            Vector3 current = transform.position;
            Vector3 goal = targetPlayer.position;
            Vector3 to = goal - current;
            float dist = to.magnitude;

            Vector3 dir = Vector3.zero;
            if (dist > 0.0001f)
            {
                dir = to / dist;
            }

            Vector3 delta = dir * moveSpeedWhenMagnet * Time.deltaTime;
            transform.position = current + delta;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
        {
            return;
        }

        LevelSystem level = collision.GetComponent<LevelSystem>();
        if(level != null)
        {
            level.AddExperience(amount);
        }

        Destroy(gameObject);
    }

    public void BeginMagnetize(Transform player)
    {
        targetPlayer = player;
        isMagnetized = true;
    }

    public void SetAmount(float value)
    {
        amount = value;
    }
}
