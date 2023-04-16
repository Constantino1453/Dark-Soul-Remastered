using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public float offset = 0.1f;//胶囊沉降补偿

    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    private void Awake()
    {
        radius = capcol.radius - 0.05f;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //两点为胶囊上下球心
        point1 = transform.position + transform.up * (radius - offset);//位置+向上单位向量*(胶囊半径-沉降补偿)
        point2 = transform.position + transform.up * (capcol.height - radius - offset);

        Collider[] outputCols = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));//第4个参数指定Layer
        if (outputCols.Length != 0)
        {
            SendMessageUpwards("IsGround");
            //   foreach(var col in outputCols)
            //   {
            //       print("get collision:" + col.name);
            //   }

        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
}
