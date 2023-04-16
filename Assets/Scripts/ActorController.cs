using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]//��ֹ��ֲʱȱ��Rigidbody
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
    //����ڿ�������ǽ�����Ħ�����ס��bug
    //��GroundĦ��1����GroundĦ��0
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;


    //[SerializeField] //��private��ʾ�ڱ༭���ϣ�private�����ֶ��ϣ��ʲ���public��
    public Animator animator;
    private Rigidbody rigid;//ʹ�ø�����ײ�󣬼�ʹ����ѡybot-Animator-Apply Root MotionҲ�����ƶ�~
    private Vector3 planarVec;//������Ҳ���
    private Vector3 thrustVec;//���� 
    private bool canAttack;//������Ծ�����й���
    private bool lockPlanar = false;//lockPlanar:���������ٶ�����
    private bool trackDirection = false;//���棬ģ��Ӧ׷��planarvector֮����
    private CapsuleCollider col;//������ײ
    //private float lerpTarget;//animator-layers��01����
    private Vector3 deltaPosition;//����ͨ�������ٶ��ƶ��ģ�����ʱ����Ҫ��ȡ�ƶ������ƶ����ʽ���DeltaPosition

    public bool leftIsShield = true;//ģ�������ö��ź�

    public delegate void OnActionDelegate();//�籨ԱС�㣬����ϣ����ͬģ��֮����ź��ɵ籨ԱС�㴫�ݣ��Լ�����¶
    public event OnActionDelegate OnAction;//���ڹ���OnActionDelegate�������Բ���Ҳ�����صĺ���
    //delegate-event model:ί���¼�ģ��

    void Awake()//awake before start~
    {
        camcon = GetComponentInChildren<CameraController>();//?
        IUserInput[] inputs = GetComponents<IUserInput>();
        //getcomponentֻget��һ��
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = (IUserInput)input;
                //����Ծ�������źŴӳ�����������������ã�
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
        //������ΰ��µ��ô�������1���������FixedUpdate

        if (!camcon.lockState)
        {
        //float targetRunMulti = (pi.run ? 2.0f : 1.0f);
        animator.SetFloat("forward", pi.Dmag
            * Mathf.Lerp(animator.GetFloat("forward"), (pi.run ? 2.0f : 1.0f), 0.5f));
            //�˴�lerp���䱣֤��·�л�Ϊ�ܲ�ʱ����仯̫��
            animator.SetFloat("right", 0);
        }
        else//����״̬��
        {
            //Dvec is global ,so we need transform it
            Vector3 localDvec = transform.InverseTransformVector (pi.Dvec);
            animator.SetFloat("forward", localDvec.z * (pi.run ? 2.0f : 1.0f));
            animator.SetFloat("right", localDvec.x * (pi.run ? 2.0f : 1.0f));
            //�ı�����
            if(!camcon.isAI && camcon.changeLockEnable)
            {
                if (pi.Jright < -0.8f)
                { camcon.ChangeLock(true); Invoke("ChangeLockEnable", 0.2f); }
                if (pi.Jright > 0.8f)
                { camcon.ChangeLock(false); Invoke("ChangeLockEnable", 0.2f); }
            }
        }

        animator.SetBool("defense", pi.defense);//???
        //����漰��װ��������Ҫ��attack�����layerȨ��

        //if (pi.jump && rigid.velocity.magnitude > 1.0f)//roll or jab 
        //{
        //    animator.SetTrigger("roll");
        //}//���������ֱ��~
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

        if ((pi.rb || pi.lb) && (CheckState("ground") || (CheckStateTag("attackR") || CheckStateTag("attackL")) && canAttack)) //���ӵĹ�������www
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
            //�����ֹ���

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
            if(pi.inputEnable)//��ֹ����Ĵ���AM�и��Ž����������ӽ�����
            {
                if (pi.Dmag > 0.1f)
                { //�����������ɿ��������ǿ��ת��ǰ��
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
              model.transform.forward = transform.forward;//����ͽ���ͬ��
            }
            else
            {
                model.transform.forward = planarVec.normalized;//������planarVecͬ��
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



        //planarVec = pi.Dmag * model.transform.forward* walkSpeed //����*����
        //    * (pi.run?runMultiplier:1.0f);//�ܲ��ж�

    }

    void FixedUpdate()//update60fps,fixedupdate50fps;����update��rigidbodyֻ�ܵ���
    {
        //rigid.position += planarVec * Time.fixedDeltaTime;
        //��֡��� Time.fixedDeltaTime != Time.DeltaTime

        rigid.position += deltaPosition;

        //rigid.velocity = planarVec;
        //��������ٶȶ���λ�ã����ó�DeltaTime������д������rigidbody�Դ��ĵ���������ʧ��
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        //��Ӧд�����:�����ӳ��� 
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
    //��ʦ��ʱ�汾��unity��û�����api��������animator.GetCurrentAnimatorStateInfo(0).isTag("skill")

    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        //��animator ��tag���ж�
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
        //��ִ��һ���޷��󳷲������ǰᵽOnJabUpdate
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("jabVelocity");//��ģ����Եķ���ӷ����ٶ�
    }


    public void OnAttack1handAEnter()
    {
        pi.inputEnable = false;
        //lockPlanar = true;

        //lerpTarget = 1.0f;//��ʼ����ʱȨ��1 //ûͼ���ϣ�


    }

    public void OnAttack1handAUpdate()
    {
        thrustVec = model.transform.forward * animator.GetFloat("attack1handAVelocity");//��ģ����Եķ���ӷ����ٶ�
        /*
        //float currentWeight = animator.GetLayerWeight(animator.GetLayerIndex("attack"));
        //float currentWeight = Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget,0.1f);

        animator.SetLayerWeight(animator.GetLayerIndex("attack"),
            Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget, 0.4f));
        //we know animator.GetLayerIndex("attack")==1,but animator.GetLayerIndex("attack") is much more readable~ 
        //ûͼ����
         */
    }
    /*
    public void OnAttackIdleEnter()
    {
        pi.inputEnable = true;
        
        //lockPlanar = false;
        //animator.SetLayerWeight(animator.GetLayerIndex("attack"), 0);

        lerpTarget = 0.0f;//��ʼ����ʱȨ��0
        //ûͼ����
        
}//ûidle���ö����ϣ�

public void OnAttackIdleUpdate()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("attack"),
            Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    //ûͼ���ϣ�
    */

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnHitEnter()
    {
        pi.inputEnable = false;//�ܻ�ʱ��������
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
        model.SendMessage("WeaponDisable");//�Է���һ�������װ~
    }

    public void OnDieEnter()
    {
        pi.inputEnable = false;//�ܻ�ʱ��������
        planarVec = new Vector3(0, planarVec.y, 0);//Mr.F:  = Vector3.zero;
        model.SendMessage("WeaponDisable");//�Է���һ�������װ~
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

    public void OnCounterBackExit()//�ܷ�ʱ�ܻ����ڶܷ�������ضܷ�״̬
    {
        model.SendMessage("CounterBackDisable");
    }

    public void OnUpdateRootMotion(object _deltaPosition)
    {
        //����ʱд��(Vector3)_deltaPosition
        if (CheckState("attack1handC") || CheckState("attack1handB")) //�ڶ������� ͼ�� �Ѿ��ò�������
        {
            deltaPosition += (0.2f * deltaPosition + 0.8f * (Vector3)_deltaPosition);
            //+=deltap ��������˳��~
        }

    }

    public void OnLockEnter() 
    {
        pi.inputEnable = false;
        planarVec = Vector3.zero;//Mr.F:  = Vectore.zero;
        model.SendMessage("WeaponDisable");//�Է���һ�������װ~
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
        animator.SetBool(boolName, _set);//����ͬ�����ˣ���Ӳ
    }

    public void OnDefenseExit()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("defense"), 0);//I add it myself to avoid defense Layer keeping1
    }
}
// ���ֻ���˼·:lerp/slerp SmoothDamp Ȩ�ط���ʽ
