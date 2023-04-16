using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : IActorManagerInterface
{
    //public ActorManager am;

    public float HPMax = 50.0f;
    public float HP = 15.0f;

    [Header("1st order state flags")]
    public bool isGround;
    public bool isJump;
    public bool isFall;
    public bool isRoll;
    public bool isJab;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    public bool isBlocked;
    public bool isDefense;
    public bool isCounterBack; //决定状态
    public bool isCounterBackEnable; //决定动画事件
    
    //用一阶旗标凑出来的二阶旗标
    [Header("1st order state flags")]
    public bool isAllowDefense;
    public bool isImmortal;
    public bool isCounterBackSuccess;
    public bool isCounterBackFailure;

    private void Start()
    {
        HP = Mathf.Clamp(HP, 0, HPMax);
    }

    private void Update()
    {
        isGround = am.ac.CheckState("ground");
        isJump = am.ac.CheckState("jump");
        isFall = am.ac.CheckState("fall");
        isRoll = am.ac.CheckState("roll");
        isJab = am.ac.CheckState("jab");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");//state machine内的状态可以用tag读取
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isBlocked = am.ac.CheckState("blocked");
        //isDefense = am.ac.CheckState("defense1hand","defense");//不同layer用两个参数
        isCounterBack = am.ac.CheckState("counterBack");
        //isCounterBack = true;//开挂用
        isCounterBackSuccess = isCounterBackEnable;//上面两个判断反击动画
        isCounterBackFailure = isCounterBack && !isCounterBackEnable;//下面这两个条件判断扣血

        isAllowDefense = isGround || isBlocked;//唯二可能防御的状态
        isDefense = isAllowDefense && am.ac.CheckState("defense1hand", "defense");
        isImmortal = isRoll || isJab;//两种无敌状态
    }
    public void AddHp(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HPMax);//掐头去尾函数www，低于0帰0，高于max帰max
    }


    public void Test()
    {
        print("Yes sweet heart~~" + HP);
    }
}
