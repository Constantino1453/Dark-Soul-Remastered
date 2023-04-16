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
    public bool isCounterBack; //����״̬
    public bool isCounterBackEnable; //���������¼�
    
    //��һ�����ճ����Ķ������
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
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");//state machine�ڵ�״̬������tag��ȡ
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isBlocked = am.ac.CheckState("blocked");
        //isDefense = am.ac.CheckState("defense1hand","defense");//��ͬlayer����������
        isCounterBack = am.ac.CheckState("counterBack");
        //isCounterBack = true;//������
        isCounterBackSuccess = isCounterBackEnable;//���������жϷ�������
        isCounterBackFailure = isCounterBack && !isCounterBackEnable;//���������������жϿ�Ѫ

        isAllowDefense = isGround || isBlocked;//Ψ�����ܷ�����״̬
        isDefense = isAllowDefense && am.ac.CheckState("defense1hand", "defense");
        isImmortal = isRoll || isJab;//�����޵�״̬
    }
    public void AddHp(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HPMax);//��ͷȥβ����www������0��0������max��max
    }


    public void Test()
    {
        print("Yes sweet heart~~" + HP);
    }
}
