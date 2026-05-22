using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float moveSpeed = 2f;

    public Transform leftPoint;
    public Transform rightPoint;

    private bool movingRight = true;

    void Start()
    {
        if (leftPoint == null || rightPoint == null)
        {
            Debug.LogError("LeftPoint 또는 RightPoint가 Inspector에 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        if (leftPoint == null || rightPoint == null) return;

        Move();
    }

    void Move()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

            if (transform.position.x >= rightPoint.position.x)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

            if (transform.position.x <= leftPoint.position.x)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}