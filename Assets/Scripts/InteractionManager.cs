using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    private CapsuleCollider interCol;//�ڳ�ʼ�׶��Ѿ���battlemanager������~

    public List<EventCasterManager> overlapEcastms = new List<EventCasterManager>();//���������غϵ�Ecastms��

    // Start is called before the first frame update
    void Start()
    {
        interCol = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach(var ecastm in ecastms)
        {
        //    print(ecastm.eventName);
            if (!overlapEcastms.Contains(ecastm))
            {
                overlapEcastms.Add(ecastm);//���û�оͼӽ���
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach (var ecastm in ecastms)
        {
            if (overlapEcastms.Contains(ecastm))
            {
                overlapEcastms.Remove(ecastm);//����оͰε�
            }
        }
    }
}
