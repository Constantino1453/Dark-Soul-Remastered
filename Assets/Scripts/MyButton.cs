using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    //Debug.Log(""); ���ڷ�����������ڵ���(û�̳�monobehaviour��print��������)

    public bool IsPressing = false;//�밴����01�ź�һ��
    public bool OnPressed = false;//����˲��1����0
    public bool OnRealeased = false;//�ɿ�˲��1����0
    public bool IsExtending = false;//�ɿ�˲���һ��ʱ���³�IsE.�ź�
    public bool IsDelaying = false;//����һ��ʱ��תΪtrue�����жϡ�������

    public float extendingDuration = 0.15f;
    public float delayingDuration = 0.15f;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    extTimer.duration = 1.0f;
        //    extTimer.Go();
        //}
        //�Ѿ���StartTimerȡ����

        //StartTimer(extTimer, 1.0f);
        extTimer.Tick();
        //Debug.Log(extTimer.state);
        delayTimer.Tick();

        curState = input;

        IsPressing = curState;

        OnPressed = false;
        OnRealeased = false;
        IsExtending = false;
        IsDelaying = false;

        if(curState != lastState)//�б仯��
        {
            if (curState)//����
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }
            else//�ɿ�
            {
                OnRealeased = true;
                StartTimer(extTimer, extendingDuration);//�����������ʱ��
            }
        }

        lastState = curState;

        //if(extTimer.state == MyTimer.STATE.RUN)
        //{
        //    IsExtending = true;
        //}
        //else
        //{
        //    IsExtending = false;
        //}
        //����һ�仰�ͺ�~~~
        IsExtending = (extTimer.state == MyTimer.STATE.RUN);
        IsDelaying = (delayTimer.state == MyTimer.STATE.RUN);


    }

    private void StartTimer(MyTimer timer,float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
