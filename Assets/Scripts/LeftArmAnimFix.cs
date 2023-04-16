using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    //����ģ����
    private Animator anim;
    private ActorController ac;
    public Vector3 a = new Vector3(0, -10, -3);

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }
    //��Ҫ��layers�й�ѡIKPass�Żἤ��
    private void OnAnimatorIK(int layerIndex)
    {
        if (ac.leftIsShield)
        {
            if (anim.GetBool("defense") == false)
            {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                //��ȡ���±��˹����
                //leftLowerArm.localEulerAngles = Vector3.zero;
                leftLowerArm.localEulerAngles += a;
                anim.SetBoneLocalRotation(
                    HumanBodyBones.LeftLowerArm,
                    Quaternion.Euler(leftLowerArm.localEulerAngles));
                //Quaternion.Euler(leftLowerArm.localEulerAngles))ŷ����ת��Ԫ��
            }
        }
    }
}
//OnUpdateRM & OnAnimatorIK �ǵ���ģ�����Ļ���
