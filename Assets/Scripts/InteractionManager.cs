using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    private CapsuleCollider interCol;//在初始阶段已经由battlemanager生成了~

    public List<EventCasterManager> overlapEcastms = new List<EventCasterManager>();//和我有无重合的Ecastms？

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
                overlapEcastms.Add(ecastm);//如果没有就加进来
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
                overlapEcastms.Remove(ecastm);//如果有就拔掉
            }
        }
    }
}
