using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers//拓展方法
{
    //public static void hihi(this Transform trans,string say)//方法挂载到transform类上,第一个参数不用填
    //{
    //    Debug.Log(trans.name + " say: " + say);
    //}

    public static Transform DeepFind(this Transform parent, string targetName)
    {
        Transform tempTrans = null;
        foreach (Transform child in parent) //如果用var则不能用.name
        {
            if (child.name == targetName)
            {
                //Debug.Log("i got " + child.name);//如果else里没有非空判断返回，则明明get到了，但结果还是空
                return child;
            }
            else
            {
                tempTrans = DeepFind(child, targetName);//深度优先搜索
                if (tempTrans != null)
                {
                    return tempTrans;
                }
            }
        }
        return null;
    }
}
