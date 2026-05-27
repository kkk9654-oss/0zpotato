using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    public GameObject gameOverText;

    void Start()
    {
        // 체력 초기화
        currentHP = maxHP;

        // 게임오버 UI가 연결되어 있는지 확인용 로그
        if (gameOverText == null)
        {
            Debug.LogError("gameOverText가 Inspector에 연결되지 않았습니다!");
        }
        else
        {
            // 시작 시 게임오버 텍스트 숨김
            gameOverText.SetActive(false);
        }

        Debug.Log("PlayerHP Start 완료 - 현재 체력: " + currentHP);
    }

    // 외부(몬스터 등)에서 호출되는 데미지 함수
    public void TakeDamage(int damage)
    {
        Debug.Log("TakeDamage 호출됨! 받은 데미지: " + damage);

        currentHP -= damage;

        // 현재 체력 출력
        Debug.Log("플레이어 체력: " + currentHP);

        // 체력이 0 이하이면 사망 처리
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("게임 오버");

        // 게임오버 UI 활성화
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // 게임 정지
        Time.timeScale = 0f;
    }
}