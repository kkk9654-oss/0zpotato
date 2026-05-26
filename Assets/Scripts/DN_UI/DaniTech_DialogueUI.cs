using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaniTech_DialogueUI : DaniTechUIBase
{
    [SerializeField] private GameObject Layout_CharacterName;
    [SerializeField] private Text Text_Character;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_Next;

    // 자동 종료 시간
    [SerializeField] private float autoCloseTime = 3f;

    private string _currentDialogueId;
    private Queue<string> _descriptionQueue = new Queue<string>();

    private Coroutine autoCloseCoroutine;

    private void OnEnable()
    {
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
    }

    // Next 버튼 클릭
    public void OnClick_Next()
    {
        // 자동 종료 중지
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // 다음 대사 체크
        bool isNextDescriptionExist = CheckAndSetDescription();

        if (isNextDescriptionExist)
        {
            StartAutoClose();
            return;
        }

        // 다음 다이얼로그 체크
        bool isNextDialogueExist = CheckAndStartNextDialogue();

        // 다음 다이얼로그 없으면 종료
        if (isNextDialogueExist == false)
        {
            CloseDialogue();
        }
    }

    // 다음 다이얼로그 체크
    private bool CheckAndStartNextDialogue()
    {
        var dialogueData =
            DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);

        if (dialogueData == null)
        {
            Debug.LogWarning($"현재 다이얼로그 데이터 없음 : {_currentDialogueId}");
            return false;
        }

        string nextDialogueId = dialogueData.NextDialogueId;

        // 다음 ID가 비어있지 않은 경우
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            // 다음 데이터 존재 확인
            var nextData =
                DaniTechGameDataManager.Instance.GetDNDialogueData(nextDialogueId);

            if (nextData != null)
            {
                StartDialogue(nextDialogueId);
                return true;
            }
            else
            {
                Debug.LogWarning($"다음 다이얼로그 데이터 없음 : {nextDialogueId}");
            }
        }

        return false;
    }

    // 다이얼로그 시작
    public void StartDialogue(string dialogueId)
    {
        var dialogueData =
            DaniTechGameDataManager.Instance.GetDNDialogueData(dialogueId);

        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터 없음 : {dialogueId}");

            CloseDialogue();
            return;
        }

        // UI 켜기
        gameObject.SetActive(true);

        _currentDialogueId = dialogueId;

        // 기존 대사 제거
        _descriptionQueue.Clear();

        // <np> 태그로 페이지 분리
        if (dialogueData.Description.Contains("<np>"))
        {
            string[] dialogueDescriptionList =
                dialogueData.Description.Split("<np>");

            foreach (string desc in dialogueDescriptionList)
            {
                _descriptionQueue.Enqueue(desc);
            }

            CheckAndSetDescription();
        }
        else
        {
            SetCurrentDialogueDescription(dialogueData.Description);
        }

        SetCharacterName(dialogueData.CharacterDataId);

        // 자동 종료 시작
        StartAutoClose();
    }

    // 다음 설명 체크
    private bool CheckAndSetDescription()
    {
        bool isNextDescriptionExist = (_descriptionQueue.Count > 0);

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
        bool isActive = !string.IsNullOrEmpty(characterDataId);

        Layout_CharacterName.SetActive(isActive);

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

    // 대사 설정
    private void SetCurrentDialogueDescription(string description)
    {
        Text_Description.text = description;
    }

    // 자동 종료 시작
    private void StartAutoClose()
    {
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        autoCloseCoroutine = StartCoroutine(AutoCloseCoroutine());
    }

    // 자동 종료 코루틴
    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(autoCloseTime);

        CloseDialogue();
    }

    // 다이얼로그 종료
    private void CloseDialogue()
    {
        // 코루틴 종료
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        // 큐 초기화
        _descriptionQueue.Clear();

        // UI 끄기
        gameObject.SetActive(false);
    }
}