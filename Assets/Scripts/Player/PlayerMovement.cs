using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Rigidbody2D body;
    public GameManager gameManager;

    private Vector2 inputDirection;

    // Update is called once per frame
    void Update()
    {
        bool canMove = false;
        if(gameManager != null)
        {
            canMove = gameManager.IsPlaying();
        }

        if(canMove == true)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 raw = new Vector2(x, y);
            if(raw.sqrMagnitude > 1.0f)
            {
                raw = raw.normalized;
            }

            inputDirection = raw;
        }
        else
        {
            inputDirection = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;

        body.linearVelocity = targetVelocity;
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
