using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����ͳ�ȥ��event�����������animator��ͬ����~

public class AnimatorTriggerController : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /*
    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update(){}
    */

    public void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
        //��մ�ʱ��trigger������attack��
    }
}
