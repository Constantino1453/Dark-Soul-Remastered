using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    //public ActorManager am;
    private Collider weaponColL;
    private Collider weaponColR;

    public GameObject whL;
    public GameObject whR;

    public WeaponController wcL;
    public WeaponController wcR;

    private void Start()
    {
        whL = transform.DeepFind("weaponHandleL").gameObject;
        whR = transform.DeepFind("weaponHandleR").gameObject;

        wcL = BindWeaponController(whL);
        wcR = BindWeaponController(whR);

        weaponColL = whL.GetComponentInChildren<Collider> ();
        weaponColR = whR.GetComponentInChildren<Collider> ();

        WeaponDisable ();
        //weaponCol = whR.GetComponentInChildren<Collider>();
        //print(transform.DeepFind("weaponHandleR")+ "wtf?");
    }

    public WeaponController BindWeaponController(GameObject targetObj)
    {
        WeaponController tempWc;
        tempWc = targetObj.GetComponent<WeaponController>();
        if (tempWc == null)
        {
            tempWc = targetObj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }

    //use 
    public void WeaponEnable()
    {
        if(am.ac.CheckStateTag("attackL")) { 
            weaponColL.enabled = true;
        }
        
        weaponColR.enabled = true;
        //print("WE");
    }
    public void WeaponDisable()
    {
        weaponColL.enabled = false;
        weaponColR.enabled = false;
        //print("WD");
        //左手一律关捏
    }

    public void CounterBackEnable()
    {
        am.SetIsCounterBack(true);
    }

    public void CounterBackDisable()
    {
        am.SetIsCounterBack(false);
    }
}
