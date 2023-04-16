using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterJoystick : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        //print(Input.GetAxis("Horizontal"));
        //print(Input.GetAxis("Vertical"));
        //print("Jright:" + Input.GetButton("axisX"));
        //print("5:" + Input.GetAxis("axis5Jup"));
        //print("4:" + Input.GetAxis("axis4Jright"));
        //
        //print("A:" + Input.GetButton("btn0A"));
        //print("B:" + Input.GetButton("btn1"));
        //print("C:" + Input.GetButton("btn2C"));
        //print("X:" + Input.GetButton("btn3"));
        //print("Y:" + Input.GetButton("btn4"));
        //print("Z:" + Input.GetButton("btn5"));
        //print("LB:" + Input.GetButton("btn4LB"));
        //print("RB:" + Input.GetButton("btn5RB"));

        print("RLT:" + Input.GetAxis("axisRLT"));
        //得到3rd axis值，默认0 左耳朵-1 右耳朵+1
        print("RLT get raw:" + Input.GetAxisRaw("axisRLT"));
        //测试发现getaxis 和 getaxisraw无平滑效果。。。

        //print("6th axis:" + Input.GetAxis("axis6padH"));
        //print("7th axis:" + Input.GetAxis("axis7padV"));
        //print("R3" + Input.GetButton("btn9R3"));
    }
}
