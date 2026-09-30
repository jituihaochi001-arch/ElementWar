using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IStateMachineOwner { }//状态机宿主标记
//角色状态机
public class StateMachine 
{
    private StateBase currentState;//当前状态
    private IStateMachineOwner owner;//声明接口指针
    private Dictionary<Type,StateBase> stateDic = new Dictionary<Type,StateBase>();//状态字典

    //有参构造函数
    public StateMachine(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    //进入状态实例
    public void EnterState<T>() where T : StateBase,new()
    { 
        if (currentState != null && currentState.GetType() == typeof(T)) return;//避免重复进入同一状态
        if (currentState != null)
            currentState.Exit();
        currentState = LoadState<T>();
        currentState.Enter();
    }
    private StateBase LoadState<T>() where T: StateBase,new()
    {
        Type type = typeof(T);
        if (!stateDic.TryGetValue(type,out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDic.Add(type, state);
        }
        return state; 
    }

    public void Stop()
    {
        if(currentState != null)
            currentState.Exit();
        foreach (var state in stateDic.Values)
            state.Destroy();
        stateDic.Clear();
    }
}
