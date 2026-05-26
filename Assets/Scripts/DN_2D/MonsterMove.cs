using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed = 2f;

    // 이동 범위
    public float leftLimit = -20f;
    public float rightLimit = 20f;

    // 오른쪽 이동 여부
    private bool movingRight = true;

    // 메인 카메라
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Move();
        ClampToCamera();
    }

    // 몬스터 이동
    void Move()
    {
        Vector3 pos = transform.position;

        // 오른쪽 이동
        if (movingRight)
        {
            pos.x += moveSpeed * Time.deltaTime;

            // 오른쪽 끝 도착
            if (pos.x >= rightLimit)
            {
                pos.x = rightLimit;

                movingRight = false;

                Flip();
            }
        }
        // 왼쪽 이동
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;

            // 왼쪽 끝 도착
            if (pos.x <= leftLimit)
            {
                pos.x = leftLimit;

                movingRight = true;

                Flip();
            }
        }

        transform.position = pos;
    }

    // 몬스터 방향 반전
    void Flip()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }

    // 🔥 카메라 밖 이동 방지
    void ClampToCamera()
    {
        if (cam == null)
            return;

        Vector3 pos = transform.position;

        // 🔥 카메라와 몬스터 거리 계산
        float distance =
            Mathf.Abs(transform.position.z - cam.transform.position.z);

        // 🔥 화면 경계 계산
        Vector3 leftBound =
            cam.ViewportToWorldPoint(new Vector3(0, 0.5f, distance));

        Vector3 rightBound =
            cam.ViewportToWorldPoint(new Vector3(1, 0.5f, distance));

        // 🔥 화면 안으로 제한
        pos.x = Mathf.Clamp(pos.x, leftBound.x, rightBound.x);

        transform.position = pos;
    }

    // Scene 창 이동 범위 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            new Vector3(leftLimit, transform.position.y, 0),
            new Vector3(rightLimit, transform.position.y, 0)
        );
    }
}