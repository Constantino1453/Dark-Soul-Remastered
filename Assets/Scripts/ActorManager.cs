using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;

    [Header("===== Auto Generated if Null =====")]
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    public DirectorManager dm;
    public InteractionManager im;
    void Awake()
    {
        ac = GetComponent<ActorController>();
        GameObject model = ac.model;
        GameObject sensor = transform.Find("sensor").gameObject;
        /*
        bm = sensor.GetComponent<BattleManager>();
        if(bm == null)
        {
            bm = sensor.AddComponent<BattleManager>();
        }
        //bm = sensor.GetOrAddComponent<BattleManager>();//�������θ�ֵ�ɼ�д���
        bm.am = this;
        */

        //��~
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
        dm = Bind<DirectorManager>(gameObject);
        bm = Bind<BattleManager>(sensor);
        im = Bind<InteractionManager>(sensor);

        ac.OnAction += DoAction;
        //ac.OnAction += Action2;
    }

    public void DoAction()
    {
        //print("do action yo~");
        if (im.overlapEcastms.Count != 0 && im.overlapEcastms[0].active == true) 
        {
            //print(im.overlapEcastms[0].eventName + ":let's see whats inside");
            //Corresponding(eventName) timeline shall be played here
            switch(im.overlapEcastms[0].eventName) 
            {
                case "frontStab":
                    ac.model.transform.LookAt(im.overlapEcastms[0].am.transform, Vector3.up);
                    dm.playFrontStab("frontStab", this, im.overlapEcastms[0].am);
                    //print(im.overlapEcastms[0].eventName);
                    break;
                case "openBox":
                    if (BattleManager.CheckAnglePlayer(ac.model, im.overlapEcastms[0].am.gameObject, 60.0f))//������ܿ���
                    {
                        im.overlapEcastms[0].active = false;
                        transform.position = im.overlapEcastms[0].am.gameObject.transform.position 
                            + im.overlapEcastms[0].am.transform.TransformVector(im.overlapEcastms[0].offset);//offset�������壬��ת��������
                        ac.model.transform.LookAt(im.overlapEcastms[0].am.transform, Vector3.up);
                        dm.playFrontStab("openBox", this, im.overlapEcastms[0].am);
                    }
                    break;
                case "leverUp":
                    if (BattleManager.CheckAnglePlayer(ac.model, im.overlapEcastms[0].am.gameObject, 270.0f))//�����������˾����Ķ�����
                    {
                        //im.overlapEcastms[0].active = false;//���б�����һ��ִ��
                        print("0");
                        transform.position = im.overlapEcastms[0].am.gameObject.transform.position
                            + im.overlapEcastms[0].am.transform.TransformVector(im.overlapEcastms[0].offset);//offset�������壬��ת��������
                        print("1");
                        ac.model.transform.LookAt(im.overlapEcastms[0].am.transform, Vector3.up);
                        print("2");
                        dm.playFrontStab("leverUp", this, im.overlapEcastms[0].am);
                        print("3");
                    }
                    break;
            }
        }
    }
    //public void Action2()
    //{
    //    print("a2 yo~");
    //}

    // ��ϡ���
    private T Bind<T>(GameObject go) where T : IActorManagerInterface //���ͷ�����ͨ�����Ͳ��������ķ��� != ��̬
    {
        T tempInstance;
        tempInstance = go.GetComponent<T>();
        if (tempInstance == null)
        {
            tempInstance = go.AddComponent<T>();
        }
        tempInstance.am = this;//������am��Ϊ�Լ�~��temp������ű�����am���Ǿֲ�����
        return tempInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIsCounterBack(bool value)
    {
        sm.isCounterBackEnable = value;
    }

    public void TryDoDamage(WeaponController targetWc,bool attackValid = true,bool counterValid = true)
    {
        //if (sm.HP > 0)//��ֹ�����Ժ󲥷���������թʬww
        //{
        //    sm.AddHp(-5.0f);
        //}
        if (sm.isCounterBackSuccess)
        {
            if(attackValid)
            {
                targetWc.wm.am.Stunned();//�öԷ��ϴ���~~
            }
        }
        else if (sm.isCounterBackFailure)
        {
            if (attackValid)
            {
                HitOrDie(-7.5f,false);//i add it myself
            }
        }
        else if (sm.isImmortal) { }//�޵е�����ww
        else if (sm.isDefense && attackValid)// && attackValid ��ʦû�ӣ����ǹ���ʧ��ΪɶҪ������ww
        {
            HitOrDie(-0.5f,false);
            //attack shall be blocked
            Blocked();
        }
        else
        {
            if (attackValid) 
            { 
                HitOrDie(-5);
            }
        }
    }

    public void Stunned()
    {
        ac.IssueTrigger("stunned");
    }
    public void Blocked()
    {
        ac.IssueTrigger("blocked");
    }

    public void Hit() { ac.IssueTrigger("hit");}

    /*
    public void HitOrDie(float heal)
    {
        if (sm.HP <= 0)
        {
            // already dead
        }
        else
        {
            sm.AddHp(heal);
            if (sm.HP > 0)
            {
                Hit();
            }
            else { Die(); }
        }
    }
    */

    public void HitOrDie(float heal,bool doHitAnimation = true)
    {
        if (sm.HP <= 0)
        {
            // already dead
        }
        else
        {
            sm.AddHp(heal);
            if (sm.HP > 0)
            {
                if (doHitAnimation) { Hit(); }//�ض���ʹ�öܷ�ʧ���а��壿
                // do some VFX,like splatter blood etc.
            }
            else { Die(); }
        }
    }

    public void Die() { 
        ac.IssueTrigger("die");
        ac.pi.inputEnable = false;//����ϣ�����~
        if (ac.camcon.lockState)
        {
            ac.camcon.LockUnlock();
        }
        ac.camcon.enabled = false;//���涨��
    }


    public void LockUnlockActorController(bool _set)
    {
        ac.SetBool("lock", _set);
    }
}
