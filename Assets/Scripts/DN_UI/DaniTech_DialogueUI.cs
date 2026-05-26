using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 다이얼로그 UI 클래스
// DaniTechUIBase를 상속받아 UI 시스템과 연결
public class DaniTech_DialogueUI : DaniTechUIBase
{
    // 캐릭터 이름 영역 오브젝트
    [SerializeField] private GameObject Layout_CharacterName;

    // 캐릭터 이름 텍스트
    [SerializeField] private Text Text_Character;

    // 대사 텍스트
    [SerializeField] private Text Text_Description;

    // 다음 버튼
    [SerializeField] private DaniTechUIButton Button_Next;

    // 자동으로 닫히기까지 걸리는 시간
    [SerializeField] private float autoCloseTime = 3f;

    // 현재 진행중인 다이얼로그 ID 저장
    private string _currentDialogueId;

    // 여러 페이지 대사를 저장하는 큐
    private Queue<string> _descriptionQueue = new Queue<string>();

    // 자동 종료 코루틴 저장 변수
    private Coroutine autoCloseCoroutine;

    // UI가 활성화될 때 호출
    private void OnEnable()
    {
        // Next 버튼 클릭 이벤트 연결
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
    }

    // Next 버튼 클릭 시 실행
    public void OnClick_Next()
    {
        // 자동 종료 중이면 중지
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // 다음 페이지 대사가 있는지 확인
        bool isNextDescriptionExist = CheckAndSetDescription();

        // 다음 페이지가 있다면 다시 자동 종료 시작
        if (isNextDescriptionExist)
        {
            StartAutoClose();
            return;
        }

        // 다음 다이얼로그 존재 여부 확인
        bool isNextDialogueExist = CheckAndStartNextDialogue();

        // 다음 다이얼로그가 없으면 UI 종료
        if (isNextDialogueExist == false)
        {
            CloseDialogue();
        }
    }

    // 다음 다이얼로그 실행
    private bool CheckAndStartNextDialogue()
    {
        // 현재 다이얼로그 데이터 가져오기
        var dialogueData =
            DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);

        // 데이터가 없으면 종료
        if (dialogueData == null)
        {
            Debug.LogWarning($"현재 다이얼로그 데이터 없음 : {_currentDialogueId}");
            return false;
        }

        // 다음 다이얼로그 ID 가져오기
        string nextDialogueId = dialogueData.NextDialogueId;

        // 다음 ID가 비어있지 않은 경우
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            // 다음 다이얼로그 데이터 존재 확인
            var nextData =
                DaniTechGameDataManager.Instance.GetDNDialogueData(nextDialogueId);

            // 다음 데이터가 있으면 실행
            if (nextData != null)
            {
                StartDialogue(nextDialogueId);
                return true;
            }
            else
            {
                // 다음 데이터가 없으면 경고 출력
                Debug.LogWarning($"다음 다이얼로그 데이터 없음 : {nextDialogueId}");
            }
        }

        return false;
    }

    // 다이얼로그 시작 함수
    public void StartDialogue(string dialogueId)
    {
        // 다이얼로그 데이터 가져오기
        var dialogueData =
            DaniTechGameDataManager.Instance.GetDNDialogueData(dialogueId);

        // 데이터가 없으면 종료
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터 없음 : {dialogueId}");

            CloseDialogue();
            return;
        }

        // UI 활성화
        gameObject.SetActive(true);

        // 현재 다이얼로그 ID 저장
        _currentDialogueId = dialogueId;

        // 이전 대사 초기화
        _descriptionQueue.Clear();

        // <np> 태그가 있는 경우 페이지 분리
        if (dialogueData.Description.Contains("<np>"))
        {
            // <np> 기준으로 문자열 자르기
            string[] dialogueDescriptionList =
                dialogueData.Description.Split("<np>");

            // 큐에 순서대로 저장
            foreach (string desc in dialogueDescriptionList)
            {
                _descriptionQueue.Enqueue(desc);
            }

            // 첫 번째 페이지 출력
            CheckAndSetDescription();
        }
        else
        {
            // 페이지 분리가 없으면 바로 출력
            SetCurrentDialogueDescription(dialogueData.Description);
        }

        // 캐릭터 이름 설정
        SetCharacterName(dialogueData.CharacterDataId);

        // 자동 종료 시작
        StartAutoClose();
    }

    // 다음 페이지 대사 출력
    private bool CheckAndSetDescription()
    {
        // 다음 페이지 존재 여부 확인
        bool isNextDescriptionExist = (_descriptionQueue.Count > 0);

        // 다음 페이지가 있다면 출력
        if (isNextDescriptionExist)
        {
            string desc = _descriptionQueue.Dequeue();
            SetCurrentDialogueDescription(desc);
        }

        return isNextDescriptionExist;
    }

    // 캐릭터 이름 설정
    private void SetCharacterName(string characterDataId)
    {
        // 캐릭터 ID 존재 여부 확인
        bool isActive = !string.IsNullOrEmpty(characterDataId);

        // 이름 영역 활성화 여부 설정
        Layout_CharacterName.SetActive(isActive);

        // 캐릭터 정보가 존재하면 이름 출력
        if (isActive)
        {
            var characterData =
                DaniTechGameDataManager.Instance.GetCharacterData(characterDataId);

            if (characterData != null)
            {
                Text_Character.text = characterData.Name;
            }
        }
    }

    // 현재 대사 출력
    private void SetCurrentDialogueDescription(string description)
    {
        Text_Description.text = description;
    }

    // 자동 종료 시작
    private void StartAutoClose()
    {
        // 기존 코루틴 중지
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // 새 자동 종료 코루틴 실행
        autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
    }

    // 자동 종료 코루틴
    private IEnumerator AutoCloseCoroutine()
    {
        // 지정 시간 대기
        yield return new WaitForSeconds(autoCloseTime);

        // 다이얼로그 종료
        CloseDialogue();
    }

    // 다이얼로그 종료
    private void CloseDialogue()
    {
        // 코루틴 중지
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // 대사 큐 초기화
        _descriptionQueue.Clear();

        // UI 비활성화
        gameObject.SetActive(false);
    }
}