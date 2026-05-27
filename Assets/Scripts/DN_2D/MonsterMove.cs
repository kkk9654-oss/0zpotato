using System;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed = 2f;
    public Transform leftLimit;
    public Transform rightLimit;
    // 이동 범위 (Inspector에서 직접 설정)
    // public float leftLimit = -5f;
    // public float rightLimit = 5f;

    // 방향 (true = 오른쪽, false = 왼쪽)
    private bool moveRight = true;

    // Rigidbody2D
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    void Start()
    {   
        spriteRenderer= GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 중력 영향 제거 (2D 횡이동 필수)
         rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        Move();
       // ClampPosition(); // ⭐ 핵심: 항상 위치 강제 제한
    }

    void Move()
    {
        float xVelocity;

        // 방향에 따라 속도 결정
        if (moveRight)
            xVelocity = moveSpeed;
        else
            xVelocity = -moveSpeed;

        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);

        // 오른쪽 끝 넘으면 방향 변경
        if (transform.position.x >= rightLimit.position.x)
        {
            spriteRenderer.flipX = false;
            moveRight = false;
        }
        // 왼쪽 끝 넘으면 방향 변경
        if (transform.position.x <= leftLimit.position.x)
        {
            spriteRenderer.flipX = true;

            moveRight = true;
        }
    }
     
    void ClampPosition()
    {
        Vector3 pos = transform.position;
        if (pos.x > rightLimit.position.x)
            pos.x = rightLimit.position.x;

        if (pos.x < leftLimit.position.x)
            pos.x = leftLimit.position.x;

        transform.position = pos;
        Console.WriteLine("pos.x: " + pos.x + ", pos.y: " + pos.y + ", pos.z: " + pos.z);
    }
}