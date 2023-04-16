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
        //bm = sensor.GetOrAddComponent<BattleManager>();//以上两次赋值可简写如此
        bm.am = this;
        */

        //绑定~
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
                    if (BattleManager.CheckAnglePlayer(ac.model, im.overlapEcastms[0].am.gameObject, 60.0f))//正面才能开箱
                    {
                        im.overlapEcastms[0].active = false;
                        transform.position = im.overlapEcastms[0].am.gameObject.transform.position 
                            + im.overlapEcastms[0].am.transform.TransformVector(im.overlapEcastms[0].offset);//offset在子物体，需转世界坐标
                        ac.model.transform.LookAt(im.overlapEcastms[0].am.transform, Vector3.up);
                        dm.playFrontStab("openBox", this, im.overlapEcastms[0].am);
                    }
                    break;
                case "leverUp":
                    if (BattleManager.CheckAnglePlayer(ac.model, im.overlapEcastms[0].am.gameObject, 270.0f))//我想在哪拉杆就在哪儿拉！
                    {
                        //im.overlapEcastms[0].active = false;//本行保障其一次执行
                        print("0");
                        transform.position = im.overlapEcastms[0].am.gameObject.transform.position
                            + im.overlapEcastms[0].am.transform.TransformVector(im.overlapEcastms[0].offset);//offset在子物体，需转世界坐标
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

    // 组合、绑定
    private T Bind<T>(GameObject go) where T : IActorManagerInterface //泛型方法是通过类型参数声明的方法 != 多态
    {
        T tempInstance;
        tempInstance = go.GetComponent<T>();
        if (tempInstance == null)
        {
            tempInstance = go.AddComponent<T>();
        }
        tempInstance.am = this;//将它的am设为自己~；temp是组件脚本，故am不是局部变量
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
        //if (sm.HP > 0)//防止死了以后播放死亡动画诈尸ww
        //{
        //    sm.AddHp(-5.0f);
        //}
        if (sm.isCounterBackSuccess)
        {
            if(attackValid)
            {
                targetWc.wm.am.Stunned();//让对方老大震惊~~
            }
        }
        else if (sm.isCounterBackFailure)
        {
            if (attackValid)
            {
                HitOrDie(-7.5f,false);//i add it myself
            }
        }
        else if (sm.isImmortal) { }//无敌的妹妹ww
        else if (sm.isDefense && attackValid)// && attackValid 老师没加，但是攻击失败为啥要防御呢ww
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
                if (doHitAnimation) { Hit(); }//关动画使得盾反失败有霸体？
                // do some VFX,like splatter blood etc.
            }
            else { Die(); }
        }
    }

    public void Die() { 
        ac.IssueTrigger("die");
        ac.pi.inputEnable = false;//高耦合，不好~
        if (ac.camcon.lockState)
        {
            ac.camcon.LockUnlock();
        }
        ac.camcon.enabled = false;//画面定格
    }


    public void LockUnlockActorController(bool _set)
    {
        ac.SetBool("lock", _set);
    }
}
