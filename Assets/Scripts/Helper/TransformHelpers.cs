using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers//��չ����
{
    //public static void hihi(this Transform trans,string say)//�������ص�transform����,��һ������������
    //{
    //    Debug.Log(trans.name + " say: " + say);
    //}

    public static Transform DeepFind(this Transform parent, string targetName)
    {
        Transform tempTrans = null;
        foreach (Transform child in parent) //�����var������.name
        {
            if (child.name == targetName)
            {
                //Debug.Log("i got " + child.name);//���else��û�зǿ��жϷ��أ�������get���ˣ���������ǿ�
                return child;
            }
            else
            {
                tempTrans = DeepFind(child, targetName);//�����������
                if (tempTrans != null)
                {
                    return tempTrans;
                }
            }
        }
        return null;
    }
}
