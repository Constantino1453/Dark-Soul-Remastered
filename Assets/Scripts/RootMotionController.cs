using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //���ڽ�������Դ��ƶ��������⣨���äĿ��ѡapply root motion������ģ�ͺͽ��ҷ����״����

    /*
    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update(){}
    */

    private void OnAnimatorMove()
    {
        /*
        Vector3 temp = animator.deltaPosition;
        print(temp);//temp.x or temp.y ...
        //���Կ���animator���ƶ���
        */

        //�˴�smu�ĵڶ�������ΪObject������������Ҫ��Vector����ʹ�á�װ�� ���䡱�ķ�ʽ������Object����Vector����
        SendMessageUpwards("OnUpdateRootMotion", (object)animator.deltaPosition);
    }
}
