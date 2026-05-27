using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    public int damage = 1;

    // 2D 물리 충돌 발생 시 호출되는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 Player인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌 감지됨");

            // Player 오브젝트 또는 부모에서 PlayerHP 찾기
            PlayerHP playerHP = collision.gameObject.GetComponent<PlayerHP>();

            // 혹시 PlayerHP가 자식/부모에 있을 경우 대비
            if (playerHP == null)
            {
                playerHP = collision.gameObject.GetComponentInParent<PlayerHP>();
            }

            // PlayerHP가 존재할 때만 데미지 적용
            if (playerHP != null)
            {
                Debug.Log("데미지 적용: " + damage);
                playerHP.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("PlayerHP 컴포넌트를 찾지 못했습니다!");
            }
        }
    }
}