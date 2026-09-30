using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//任务处理管理器
public class MonoManager : SingleMonoBase<MonoManager>//单例模式
{
    private Action updateAction;//任务集合
    public void AddUpdateAction(Action task)
    {
        updateAction += task;
    }
    public void RemoveUpdateAction(Action task)
    {
        updateAction -= task;
    }
    void Update()
    {
        updateAction?.Invoke();
    }
}
