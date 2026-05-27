using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaniTech_2DFieldObject : MonoBehaviour
{
    [Header("필드 오브젝트 정보")]
    [SerializeField] private int _fieldObjectInstanceId;
    [SerializeField] private string _fieldObjectDataId;
    [SerializeField] private string _fieldObjectName;

    [Header("드랍 설정")]

    // 드랍 속도 조절용 (0.1 = 매우 빠름 / 1.0 = 느림)
    [SerializeField] private float _dropDelay = 0.2f;

    // 드랍 개수 배수
    // 2면 기존보다 2배 많이 드랍
    [SerializeField] private int _dropCountMultiplier = 2;

    // 이미 획득 처리 되었는지 체크
    private bool _isCollected = false;

    public void InitFieldObjectInfoOnCreated(int instanceId, string fieldObjectDataId)
    {
        var fieldObjectData = DaniTechGameDataManager.Instance.GetDNFieldObjectData(fieldObjectDataId);

        if (fieldObjectData == null)
        {
            Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {fieldObjectDataId}");
            return;
        }

        _fieldObjectInstanceId = instanceId;
        _fieldObjectDataId = fieldObjectDataId;
    }

    public string GetFieldObjectDataId()
    {
        return _fieldObjectDataId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아니면 무시
        if (collision.CompareTag("Player") == false)
            return;

        // 중복 획득 방지
        if (_isCollected == true)
            return;

        _isCollected = true;

        // 코루틴 시작
        StartCoroutine(Co_DropItems());
    }

    /// <summary>
    /// 아이템을 일정 간격으로 드랍하는 코루틴
    /// </summary>
    private IEnumerator Co_DropItems()
    {
        // 필드 오브젝트 데이터 가져오기
        var fieldObjectData = DaniTechGameDataManager.Instance.GetDNFieldObjectData(_fieldObjectDataId);

        if (fieldObjectData == null)
        {
            Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {_fieldObjectDataId}");
            yield break;
        }

        // Harvest 또는 DropItem 타입인지 확인
        if (fieldObjectData.FieldObjectType != "Harvest" &&
            fieldObjectData.FieldObjectType != "DropItem")
        {
            yield break;
        }

        // 드랍 아이템 ID 확인
        if (string.IsNullOrEmpty(fieldObjectData.DropItemDataId))
        {
            yield break;
        }

        // 아이템 데이터 가져오기
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(fieldObjectData.DropItemDataId);

        if (itemData == null)
        {
            Debug.LogWarning($"유효하지 않은 아이템 데이터 입니다! {_fieldObjectDataId}");
            yield break;
        }

        // 드랍 범위 가져오기
        List<int> dropCountRange = fieldObjectData.DropCountRange;

        // 기본 드랍 수량
        int finalDropItemCount = 1;

        if (dropCountRange != null && dropCountRange.Count > 0)
        {
            // 범위가 2개 이상이면 랜덤값
            if (dropCountRange.Count > 1)
            {
                // Random.Range(int,int)는 최대값 미포함이라 +1 해줌
                finalDropItemCount =
                    Random.Range(dropCountRange[0], dropCountRange[1] + 1);
            }
            else
            {
                // 하나만 있으면 고정 수량
                finalDropItemCount = dropCountRange[0];
            }
        }

        // 드랍 배수 적용
        finalDropItemCount *= _dropCountMultiplier;

        // 아이템을 여러 번 나눠서 지급
        for (int i = 0; i < finalDropItemCount; i++)
        {
            // 아이템 1개씩 추가
            DaniTechGameManager.Inst.AddItem(itemData.Id, 1);

            // 로그 출력 (디버깅용)
            Debug.Log($"{itemData.Name} 획득! ({i + 1}/{finalDropItemCount})");

            // 드랍 속도 조절
            yield return new WaitForSeconds(_dropDelay);
        }

        // 모든 드랍 완료 후 제거
        DaniTechGameObjectManager.Inst.RequestDestroyFieldObject(_fieldObjectInstanceId);
    }
}