using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    //Debug.Log(""); 放在方法里可以用于调试(没继承monobehaviour连print都不能用)

    public bool IsPressing = false;//与按键的01信号一致
    public bool OnPressed = false;//按下瞬间1其余0
    public bool OnRealeased = false;//松开瞬间1其余0
    public bool IsExtending = false;//松开瞬间后一段时间吐出IsE.信号
    public bool IsDelaying = false;//按下一段时间转为true，以判断“长按”

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
        //已经被StartTimer取代！

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

        if(curState != lastState)//有变化！
        {
            if (curState)//按下
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }
            else//松开
            {
                OnRealeased = true;
                StartTimer(extTimer, extendingDuration);//启动结束后计时器
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
        //下面一句话就好~~~
        IsExtending = (extTimer.state == MyTimer.STATE.RUN);
        IsDelaying = (delayTimer.state == MyTimer.STATE.RUN);


    }

    private void StartTimer(MyTimer timer,float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
