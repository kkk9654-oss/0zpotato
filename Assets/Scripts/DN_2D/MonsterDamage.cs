using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    public int damage = 1;

    // 물리 충돌이 발생했을 때 호출됨 (Trigger 아님!)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 충돌했는지 확인
        if (collision.collider.CompareTag("Player"))
        {
            // 플레이어 HP 스크립트 가져오기
            PlayerHP playerHP = collision.collider.GetComponent<PlayerHP>();

            // HP 스크립트가 존재하면 데미지 적용
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }
        }
    }
}