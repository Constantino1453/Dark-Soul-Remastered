using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]//���������޽�����ײ���Ϊ����������һ��
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
        if (targetWc == null)//�����������жϣ����԰ѻ�ȡwc�Ĵ���д���ж�tag��
        {
            return;
        }

        GameObject attacker = targetWc.wm.am.ac.model.gameObject;//�ҵ�handler
        GameObject receiver = am.ac.model.gameObject;//��ʦ����������ac.model���ƶ�����������ͷ~
        /*
        Vector3 attackingDir = receiver.transform.position - attacker.transform.position;
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;

        //�ٶ��з�����
        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);//�ҷ�������ڵз�����
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);//�з�������ҷ�����
        float counterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);//�������潻�ǣ�0Ϊͬ��180Ϊ����

        //���ϻ㱨��
        bool attackValid = (attackingAngle1 < 60); //ֻ���ڵз���������60�Ȳ����ܻ�
        bool counterValid = (counterAngle1 < 45 && Mathf.Abs(counterAngle2 - 180) <45 ); //�з�ֻ�����ҷ���������45���ҵط����ҷ���������ʱ���ܵ���
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
        float counterAngle1 = Vector3.Angle(player.transform.forward, counterDir);//�з�������ҷ�����
        float counterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward);//�������潻�ǣ�0Ϊͬ��180Ϊ����
        bool counterValid = (counterAngle1 < playerAngleLimit && Mathf.Abs(counterAngle2 - 180) < 45); //�з�ֻ�����ҷ���������45���ҵط����ҷ���������ʱ���ܵ���
        return counterValid;
    }

    public static bool CheckAngleTarget(GameObject player, GameObject target, float targetAngleLimit = 60.0f)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position;
        float attackingAngle1 = Vector3.Angle(target.transform.forward, attackingDir);//�ҷ�������ڵз�����
        bool attackValid = (attackingAngle1 < targetAngleLimit); //ֻ���ڵз���������60�Ȳ����ܻ�
        return attackValid;
    }
}
