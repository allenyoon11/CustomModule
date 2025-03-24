using UnityEngine;
using neuroears.allen.utils;
using UnityEngine.InputSystem;

public class CustomInputHandlerTester : MonoBehaviour
{
    private CustomInputHandler inputHandler;

    private void Start()
    {
        inputHandler = new CustomInputHandler();

        // ✅ Space 키를 Jump로 등록
        inputHandler.AddAction(1, Key.Space, () => Debug.Log("✅ Jump! (Space)"));

        // ✅ LeftCtrl 키를 Attack으로 등록
        inputHandler.AddAction(2, Key.LeftCtrl, () => Debug.Log("✅ Attack! (LeftCtrl)"));

        // ✅ 등록된 액션 확인
        foreach (var action in inputHandler.GetActions())
        {
            Debug.Log($"ID: {action.Key}, Key: {action.Value.key}");
        }

        // ✅ 5초 후 키 변경 (Space → Enter)
        Invoke(nameof(ChangeKeyTest), 1f);

        // ✅ 8초 후 액션 변경
        Invoke(nameof(ChangeActionTest), 2f);

        // ✅ 10초 후 입력 감지 중지
        Invoke(nameof(PauseInput), 3f);

        // ✅ 13초 후 입력 감지 재개
        Invoke(nameof(ResumeInput), 10f);
    }

    private void ChangeKeyTest()
    {
        inputHandler.ChangeKey(1, Key.Enter);
        Debug.Log("🔄 Space → Enter 키로 변경됨");
    }

    private void ChangeActionTest()
    {
        inputHandler.ChangeAction(1, () => Debug.Log("🚀 새로운 Jump 액션 실행!"));
        Debug.Log("🔄 Jump 액션 변경됨");
    }

    private void PauseInput()
    {
        inputHandler.PauseInputHandling();
    }

    private void ResumeInput()
    {
        inputHandler.ResumeInputHandling();
    }

    private void OnDestroy()
    {
        inputHandler.Dispose();
    }
}
