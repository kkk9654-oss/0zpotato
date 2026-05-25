using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float moveSpeed = 1f;

    public Transform leftPoint;
    public Transform rightPoint;

    private bool movingRight = true;

    void Update()
    {
        if (leftPoint == null || rightPoint == null) return;

        Move();
    }

    void Move()
    {
        Transform target = movingRight ? rightPoint : leftPoint;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // 목표 지점 도착
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            movingRight = !movingRight;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}