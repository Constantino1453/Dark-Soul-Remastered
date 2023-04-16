using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作送出去的event，必须挂载在animator的同级上~

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
        //清空此时的trigger（比如attack）
    }
}
