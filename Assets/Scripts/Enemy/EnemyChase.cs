using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public Transform target;
    public Rigidbody2D body;    
    public float stopDistance = 1.0f;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        bool canMove = false;
        if (gameManager != null)
        {
            canMove = gameManager.IsPlaying();     
        }

        if(canMove == false)
        {
            return;
        }

        if(target == null)
        {
            return;
        }

        Vector2 toTarget = (Vector2)(target.position - transform.position);
        float dist = toTarget.magnitude;

        if(dist <= stopDistance)
        {
            return;
        }

        Vector2 dir = toTarget.normalized;
        //Vector2 dir = toTarget / dist;

        Vector2 current = body.position;
        Vector2 next = current + (dir * moveSpeed * Time.fixedDeltaTime);
        body.MovePosition(next);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
}
