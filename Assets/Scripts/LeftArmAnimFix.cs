using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    //挂在模型上
    private Animator anim;
    private ActorController ac;
    public Vector3 a = new Vector3(0, -10, -3);

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }
    //需要从layers中勾选IKPass才会激活
    private void OnAnimatorIK(int layerIndex)
    {
        if (ac.leftIsShield)
        {
            if (anim.GetBool("defense") == false)
            {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                //获取左下臂人骨组件
                //leftLowerArm.localEulerAngles = Vector3.zero;
                leftLowerArm.localEulerAngles += a;
                anim.SetBoneLocalRotation(
                    HumanBodyBones.LeftLowerArm,
                    Quaternion.Euler(leftLowerArm.localEulerAngles));
                //Quaternion.Euler(leftLowerArm.localEulerAngles))欧拉角转四元素
            }
        }
    }
}
//OnUpdateRM & OnAnimatorIK 是调整模型最后的机会
