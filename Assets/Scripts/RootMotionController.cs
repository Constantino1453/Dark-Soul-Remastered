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

    //用于解决动作自带移动量的问题（如果盲目勾选apply root motion则会出现模型和胶囊分离的状况）

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
        //可以看到animator的移动量
        */

        //此处smu的第二个参数为Object，但是我们想要传Vector；故使用“装箱 拆箱”的方式，藉由Object保存Vector参数
        SendMessageUpwards("OnUpdateRootMotion", (object)animator.deltaPosition);
    }
}
