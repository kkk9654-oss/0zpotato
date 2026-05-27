using UnityEngine;

public class MonsterPetrolatrolPos : MonoBehaviour
{
    public float radius = 2f;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius); // 반지름 0.5로 원 그리기
    }
}
