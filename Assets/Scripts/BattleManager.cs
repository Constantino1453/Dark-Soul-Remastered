using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]//挂载者若无胶囊碰撞则会为它立刻生成一个
public class BattleManager : IActorManagerInterface
{
    //public ActorManager am;

    private CapsuleCollider defCol;
    private void Start()
    {
        defCol = GetComponent<CapsuleCollider>();
        defCol.center = new Vector3(0, 1.0f,0);// = Vector.up * 1.0f;
        defCol.height = 1.6f;
        defCol.radius = 0.3f;
        defCol.isTrigger = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        if (targetWc == null)//如果不用这个判断，可以把获取wc的代码写在判断tag里
        {
            return;
        }

        GameObject attacker = targetWc.wm.am.ac.model.gameObject;//找到handler
        GameObject receiver = am.ac.model.gameObject;//老师以上两行无ac.model疑似读出的是摄像头~
        /*
        Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;

        //假定敌方攻击
        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);//我方相对于在敌方正面
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);//敌方相对于我方正面
        float counterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);//两方正面交角，0为同向，180为反向

        //向上汇报！
        bool attackValid = (attackingAngle1 < 60); //只有在敌方正面左右60度才能受击
        bool counterValid = (counterAngle1 < 45 && Mathf.Abs(counterAngle2 - 180) <45 ); //敌方只有在我方正面左右45度且地方与我方面面相觑时才能弹反
        */
        if (col.tag == "Weapon") {
            //if(attackingAngle1 <= 60)
            //{
            //    am.TryDoDamage(targetWc);
            //}
            am.TryDoDamage(targetWc, 
                CheckAngleTarget(receiver,attacker), 
                CheckAnglePlayer(receiver,attacker)
                );
        }
    }

    public static bool CheckAnglePlayer(GameObject player, GameObject target,float playerAngleLimit = 45.0f) 
    {
        Vector3 counterDir = target.transform.position - player.transform.position;
        float counterAngle1 = Vector3.Angle(player.transform.forward, counterDir);//敌方相对于我方正面
        float counterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward);//两方正面交角，0为同向，180为反向
        bool counterValid = (counterAngle1 < playerAngleLimit && Mathf.Abs(counterAngle2 - 180) < 45); //敌方只有在我方正面左右45度且地方与我方面面相觑时才能弹反
        return counterValid;
    }

    public static bool CheckAngleTarget(GameObject player, GameObject target, float targetAngleLimit = 60.0f)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position;
        float attackingAngle1 = Vector3.Angle(target.transform.forward, attackingDir);//我方相对于在敌方正面
        bool attackValid = (attackingAngle1 < targetAngleLimit); //只有在敌方正面左右60度才能受击
        return attackValid;
    }
}
