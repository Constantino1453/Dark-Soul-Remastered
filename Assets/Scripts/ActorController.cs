using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]//防止移植时缺少Rigidbody
public class ActorController : MonoBehaviour
{

    public GameObject model;
    public CameraController camcon;
    //public PlayerInput pi;
    public IUserInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.7f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 3.0f;

    [Space(10)]
    [Header("==== Friction Settings ====")]
    //解决在空中碰到墙面而被摩擦力黏住的bug
    //进Ground摩擦1，出Ground摩擦0
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;


    //[SerializeField] //将private显示在编辑器上（private不用手动拖，故不用public）
    public Animator animator;
    private Rigidbody rigid;//使用刚体碰撞后，即使不勾选ybot-Animator-Apply Root Motion也可以移动~
    private Vector3 planarVec;//储存玩家操作
    private Vector3 thrustVec;//冲量 
    private bool canAttack;//避免跳跃动画中攻击
    private bool lockPlanar = false;//lockPlanar:锁定刚体速度向量
    private bool trackDirection = false;//如真，模型应追踪planarvector之方向
    private CapsuleCollider col;//胶囊碰撞
    //private float lerpTarget;//animator-layers的01调整
    private Vector3 deltaPosition;//现在通过调整速度移动的，但有时候需要获取移动量来移动，故借助DeltaPosition

    public bool leftIsShield = true;//模拟左手拿盾信号

    public delegate void OnActionDelegate();//电报员小姐，我们希望不同模块之间的信号由电报员小姐传递，以减少曝露
    public event OnActionDelegate OnAction;//用于挂载OnActionDelegate那样不吃参数也不返回的函数
    //delegate-event model:委托事件模型

    void Awake()//awake before start~
    {
        camcon = GetComponentInChildren<CameraController>();//?
        IUserInput[] inputs = GetComponents<IUserInput>();
        //getcomponent只get到一个
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = (IUserInput)input;
                //将活跃的输入信号从抽象类数组里提出来用！
                break;
            }
        }


        animator = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (pi.lockon)
        {
            camcon.LockUnlock();
        }
        //如果单次按下调用次数超过1，则将其放入FixedUpdate

        if (!camcon.lockState)
        {
        //float targetRunMulti = (pi.run ? 2.0f : 1.0f);
        animator.SetFloat("forward", pi.Dmag
            * Mathf.Lerp(animator.GetFloat("forward"), (pi.run ? 2.0f : 1.0f), 0.5f));
            //此处lerp渐变保证走路切换为跑步时不会变化太快
            animator.SetFloat("right", 0);
        }
        else//锁定状态下
        {
            //Dvec is global ,so we need transform it
            Vector3 localDvec = transform.InverseTransformVector (pi.Dvec);
            animator.SetFloat("forward", localDvec.z * (pi.run ? 2.0f : 1.0f));
            animator.SetFloat("right", localDvec.x * (pi.run ? 2.0f : 1.0f));
            //改变锁定
            if(!camcon.isAI && camcon.changeLockEnable)
            {
                if (pi.Jright < -0.8f)
                { camcon.ChangeLock(true); Invoke("ChangeLockEnable", 0.2f); }
                if (pi.Jright > 0.8f)
                { camcon.ChangeLock(false); Invoke("ChangeLockEnable", 0.2f); }
            }
        }

        animator.SetBool("defense", pi.defense);//???
        //如果涉及换装备，则需要如attack般调节layer权重

        //if (pi.jump && rigid.velocity.magnitude > 1.0f)//roll or jab 
        //{
        //    animator.SetTrigger("roll");
        //}//不如下面的直接~
        if (pi.roll || rigid.velocity.magnitude >= 8.0f)
        {
            animator.SetTrigger("roll");
            canAttack = false;
        }


        if (pi.jump)
        {
            animator.SetTrigger("jump");
            canAttack = false;
        }

        if ((pi.rb || pi.lb) && (CheckState("ground") || (CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)) //复杂的攻击条件www
        {
            if (pi.rb)
            {
                animator.SetBool("R0L1", false);
                animator.SetTrigger("attack");
            }
            else if(pi.lb && !leftIsShield)
            {
                animator.SetBool("R0L1", true);
                animator.SetTrigger("attack");
            }
            //左右手攻击

            if ((CheckState("ground") ||CheckState("blocked")) && leftIsShield)
            {
                if (pi.defense)
                {
                    animator.SetBool("defense", pi.defense);
                    animator.SetLayerWeight(animator.GetLayerIndex("defense"), 1);
                }
                else { animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0); }
            }
            else { animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0); }

        }

        if((pi.rt || pi.lt) && (CheckState("ground") || CheckStateTag("attackR") || CheckStateTag("attackL") && canAttack))
        {
            if (pi.rt)
            {
                //do right heavy attack
            }
            else
            {
                if (!leftIsShield)
                {
                    //do left heavy attack
                }
                else
                {
                    animator.SetTrigger("counterBack");
                }
            }
        }


        if (!camcon.lockState)
        {
            if(pi.inputEnable)//防止里面的代码AM中干扰交互动画的视角修正
            {
                if (pi.Dmag > 0.1f)
                { //避免人物在松开方向键后强制转向前方
                    //slerp stands for Spherical linear interpolation
                   model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f); ;
                }
            }

            if (lockPlanar == false)
            {
                planarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.run ? runMultiplier : 1.0f);
            }

        }
        else
        {
            if (!trackDirection)
            {
              model.transform.forward = transform.forward;//人物和胶囊同向
            }
            else
            {
                model.transform.forward = planarVec.normalized;//人物与planarVec同向
            }
            if (lockPlanar == false)
            {
                planarVec = (pi.run ? runMultiplier : 1.0f) * walkSpeed * pi.Dvec * pi.Dmag;
            }
        }

        if (pi.action)
        {
            OnAction.Invoke();
        }



        //planarVec = pi.Dmag * model.transform.forward* walkSpeed //纯量*向量
        //    * (pi.run?runMultiplier:1.0f);//跑步判断

    }

    void FixedUpdate()//update60fps,fixedupdate50fps;故在update中rigidbody只能调参
    {
        //rigid.position += planarVec * Time.fixedDeltaTime;
        //两帧间隔 Time.fixedDeltaTime != Time.DeltaTime

        rigid.position += deltaPosition;

        //rigid.velocity = planarVec;
        //如果调整速度而非位置，则不用乘DeltaTime。这样写代价是rigidbody自带的地心引力消失了
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        //故应写成如此:向量加冲量 
        thrustVec = Vector3.zero;
        deltaPosition = Vector3.zero;

    }

    public bool CheckState(string stateName, string layerName = "Base Layer")
    {
        //int layIndex = animator.GetLayerIndex(layerName);
        //bool result = animator.GetCurrentAnimatorStateInfo(layIndex).IsName(stateName);
        //return result;

        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsName(stateName);
    }
    //老师当时版本的unity还没有相关api，现在有animator.GetCurrentAnimatorStateInfo(0).isTag("skill")

    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        //用animator 的tag来判断
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsTag(tagName);
    }





        /// <summary>
        /// Message processing block
        /// </summary>

        public void OnJumpEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
        trackDirection = true;
    }

    //public void OnJumpExit()
    //{
    //    pi.inputEnable=true;
    //    lockPlanar = false;
    //}

    public void IsGround()
    {
        animator.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        animator.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }

    public void OnRollEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, rollVelocity, 0);
        trackDirection=true;
    }

    public void OnJabEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        //thrustVec = model.transform.forward*(-jabVelocity);
        //仅执行一次无法后撤步，于是搬到OnJabUpdate
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("jabVelocity");//向模型面对的方向加反向速度
    }


    public void OnAttack1handAEnter()
    {
        pi.inputEnable = false;
        //lockPlanar = true;

        //lerpTarget = 1.0f;//开始攻击时权重1 //没图层嘞！


    }

    public void OnAttack1handAUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("attack1handAVelocity");//向模型面对的方向加反向速度
        /*
        //float currentWeight = animator.GetLayerWeight(animator.GetLayerIndex("attack"));
        //float currentWeight = Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget,0.1f);

        animator.SetLayerWeight(animator.GetLayerIndex("attack"),
            Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget, 0.4f));
        //we know animator.GetLayerIndex("attack")==1,but animator.GetLayerIndex("attack") is much more readable~ 
        //没图层嘞
         */
    }
    /*
    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        
        //lockPlanar = false;
        //animator.SetLayerWeight(animator.GetLayerIndex("attack"), 0);

        lerpTarget = 0.0f;//开始闲置时权重0
        //没图层嘞
        
}//没idle闲置动画嘞！

public void OnAttackIdleUpdate()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("attack"),
            Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    //没图层嘞！
    */

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnHitEnter()
    {
        pi.inputEnable = false;//受击时不得输入
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
        model.SendMessage("WeaponDisable");//以防万一，解除武装~
    }

    public void OnDieEnter()
    {
        pi.inputEnable = false;//受击时不得输入
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
        model.SendMessage("WeaponDisable");//以防万一，解除武装~
    }

    public void OnBlockedEnter() { 
        pi.inputEnable = false;
    }

    public void OnStuunedEnter() {
        pi.inputEnable = false;
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
    }

    public void OnCounterBackEnter() {
        pi.inputEnable = false;
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
    }

    public void OnCounterBackExit()//盾反时受击后在盾反结束后关盾反状态
    {
        model.SendMessage("CounterBackDisable");
    }

    public void OnUpdateRootMotion(object _deltaPosition)
    {
        //拆箱时写成(Vector3)_deltaPosition
        if (CheckState("attack1handC") || CheckState("attack1handB")) //第二个参数 图层 已经用不到了捏
        {
            deltaPosition += (0.2f * deltaPosition + 0.8f * (Vector3)_deltaPosition);
            //+=deltap 不如以上顺滑~
        }

    }

    public void OnLockEnter() 
    {
        pi.inputEnable = false;
        planarVec = Vector3.zero;//Mr.F:  = Vectore.zero;
        model.SendMessage("WeaponDisable");//以防万一，解除武装~
    }

    private void ChangeLockEnable()
    {
        camcon.changeLockEnable = true;
    }

    public void IssueTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetBool(string boolName,bool _set = true)
    {
        animator.SetBool(boolName, _set);//碰巧同名罢了（嘴硬
    }

    public void OnDefenseExit()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0);//I add it myself to avoid defense Layer keeping1
    }
}
// 三种缓动思路:lerp/slerp SmoothDamp 权重方程式
