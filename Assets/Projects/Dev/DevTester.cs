using System;
using UnityEngine;
using UniRx;

public class DevTester : MonoBehaviour
{
    protected void Start()
    {
        SetSubscribe();
    }

    protected virtual void SetSubscribe()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
            .Subscribe(_ =>
            {
                Test1();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
            .Subscribe(_ =>
            {
                Test2();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
            .Subscribe(_ =>
            {
                Test3();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
            .Subscribe(_ =>
            {
                Test4();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
            .Subscribe(_ =>
            {
                Test5();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha6))
            .Subscribe(_ =>
            {
                Test6();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha7))
            .Subscribe(_ =>
            {
                Test7();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha8))
            .Subscribe(_ =>
            {
                Test8();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha9))
            .Subscribe(_ =>
            {
                Test9();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Alpha0))
            .Subscribe(_ =>
            {
                Test0();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Q))
            .Subscribe(_ =>
            {
                TestQ();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.W))
            .Subscribe(_ =>
            {
                TestW();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.E))
            .Subscribe(_ =>
            {
                TestE();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ =>
            {
                TestR();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ =>
            {
                TestA();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ =>
            {
                TestS();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.D))
            .Subscribe(_ =>
            {
                TestD();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F))
            .Subscribe(_ =>
            {
                TestF();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Z))
            .Subscribe(_ =>
            {
                TestZ();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.X))
            .Subscribe(_ =>
            {
                TestX();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ =>
            {
                TestC();
            }).AddTo(this);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.V))
            .Subscribe(_ =>
            {
                TestV();
            }).AddTo(this);
        //
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.O))
            .Subscribe(_ =>
            {
                neuroears.allen.utils.OpenDir.Open();
            }).AddTo(this);
    }

    protected virtual void TestV()
    {
        Debug.Log("<color=yellow>TestV</color>");
    }

    protected virtual void TestC()
    {
        Debug.Log("<color=yellow>TestC</color>");
    }

    protected virtual void TestX()
    {
        Debug.Log("<color=yellow>TestX</color>");
    }

    protected virtual void TestZ()
    {
        Debug.Log("<color=yellow>TestZ</color>");
    }

    protected virtual void TestF()
    {
        Debug.Log("<color=yellow>TestF</color>");
    }

    protected virtual void TestD()
    {
        Debug.Log("<color=yellow>TestD</color>");
    }

    protected virtual void TestS()
    {
        Debug.Log("<color=yellow>TestS</color>");
    }

    protected virtual void TestA()
    {
        Debug.Log("<color=yellow>TestA</color>");
    }

    protected virtual void TestR()
    {
        Debug.Log("<color=yellow>TestR</color>");
    }

    protected virtual void TestE()
    {
        Debug.Log("<color=yellow>TestE</color>");
    }

    protected virtual void TestW()
    {
        Debug.Log("<color=yellow>TestW</color>");
    }

    protected virtual void TestQ()
    {
        Debug.Log("<color=yellow>TestQ</color>");
    }

    protected virtual void Test0()
    {
        Debug.Log("<color=yellow>Test0</color>");
    }

    protected virtual void Test9()
    {
        Debug.Log("<color=yellow>Test9</color>");
    }

    protected virtual void Test8()
    {
        Debug.Log("<color=yellow>Test8</color>");
    }

    protected virtual void Test7()
    {
        Debug.Log("<color=yellow>Test7</color>");
    }

    protected virtual void Test6()
    {
        Debug.Log("<color=yellow>Test6</color>");
    }

    protected virtual void Test5()
    {
        Debug.Log("<color=yellow>Test5</color>");
    }

    protected virtual void Test4()
    {
        Debug.Log("<color=yellow>Test4</color>");
    }

    protected virtual void Test3()
    {
        Debug.Log("<color=yellow>Test</color>");
    }

    protected virtual void Test2()
    {
        Debug.Log("<color=yellow>Test2</color>");
    }

    protected virtual void Test1()
    {
        Debug.Log("<color=yellow>Test1</color>");
    }
}
