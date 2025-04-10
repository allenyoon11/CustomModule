using UniRx;
using UnityEngine;
using neuroears.allen.utils;
public class OpenDirectoryTester : MonoBehaviour
{

    private void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.A))
            .Subscribe(_ =>
            {
                OpenDir.Open();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.S))
            .Subscribe(_ =>
            {
                OpenDir.Open(OpenDir.PathType.PROJECT);
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.D))
            .Subscribe(_ =>
            {
                OpenDir.Open(OpenDir.PathType.DATA);
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.F))
            .Subscribe(_ =>
            {
                OpenDir.Open(Application.persistentDataPath, "data");
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.G))
            .Subscribe(_ =>
            {
            }).AddTo(this);
    }
}
