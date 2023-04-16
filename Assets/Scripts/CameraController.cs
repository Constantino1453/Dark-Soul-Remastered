using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    //public PlayerInput pi;
    private IUserInput pi;

    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 80.0f;
    public Image lockDot;
    public bool lockState;
    public bool changeLockEnable = true;

    public bool isAI = false;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;
    private GameObject model;
    private GameObject camera0;
    [SerializeField]
    private LockTarget lockTarget;

    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;
        if (!isAI)
        {
            camera0 = Camera.main.gameObject;
            changeLockEnable = true;
            lockDot.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            //.Confined:鼠标出不去窗口 .Locked:锁死鼠标(esc取消锁死) .None:不修改鼠标行为
        }

        lockState = false;

    }
    private void Start()
    {
        //我为了避免awake中ac到了默认最上面的那个input而非活跃的input~
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;
    }

    private void FixedUpdate()
    {
        if (lockTarget == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.deltaTime);
            //左括号后打逗号，上下键看函数重载提示！！
            //cameraHandle.transform.Rotate(Vector3.right,pi.Jup * -verticalSpeed * Time.deltaTime);
            //tempEulerX = cameraHandle.transform.localEulerAngles.x;
            tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -20, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(
                tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;

        }

        else//锁中物体辣
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;//模型指向目标方向
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);//“看到脚底”
        }
        if (!isAI)
        {
            camera0.transform.position = Vector3.Lerp(camera0.transform.position, transform.position, 0.1f);
            //此处也可用smoothdamp :...=Vector3.SmoothDamp(..,..,ref cameraDampVelocity,cameraDampValue);
            //前两个参数同上 后两个参数分别为private Vector3\public float

            //camera0.transform.eulerAngles = transform.eulerAngles;
            //摄影机可以放在LateUpdate里，也可让tempEulerX -= 后面乘上Time.fixedDeltaTime放在Update里
            //但是不如让摄像机看向(Animator-UpdateMode==AnimatePhysics)的cameraHandle如下
            camera0.transform.LookAt(cameraHandle.transform);
        }

    }

    private void Update()
    {
        if (lockTarget != null)
        {
            if (!isAI)
            {
                //先把蓝点坐标设置到左下角
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position
                    + new Vector3(0, lockTarget.halfHeight, 0));
                //蓝点在脚底加半高（即敌人正中）
            }
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)//没错，ai也要解除锁死？
            {
                LockProcessA(null, false, false, isAI);
            }

            //ActorManager targetAm = lockTarget.obj.GetComponent<ActorManager>();
            if (lockTarget.am != null) //挂了AM才有死亡状态~
            {
                if(lockTarget.am.sm.isDie) //死亡解锁
                {
                    LockProcessA(null, false, false, isAI); 
                }
            }
        }
    }

    private void LockProcessA(LockTarget _lockTarget, bool _lockDotEnable, bool _lockState, bool _isAI)
    {
        lockTarget = _lockTarget;
        if (!_isAI)
        {
            lockDot.enabled = _lockDotEnable;
        }
        lockState = _lockState;
    }


    public void LockUnlock()
    {
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);//model's eyes
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;//box在正方向5格之外
        Collider[] cols = Physics.OverlapBox(boxCenter,
            new Vector3(0.5f, 0.5f, 5f), model.transform.rotation,
            LayerMask.GetMask(isAI?"Player":"Enemy"));

        /*
        //作用类似向场景中投射射线并返回所有命中对象。注意，这些结果的顺序未定义。
        //RaycastAll (Ray ray, float maxDistance= Mathf.Infinity,
        //int layerMask= DefaultRaycastLayers,
        //QueryTriggerInteraction queryTriggerInteraction= QueryTriggerInteraction.UseGlobal);
        */




        if (cols.Length == 0)
        {
            LockProcessA (null, false, false, isAI);
        }
        else
        {
            foreach (var col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)//锁定了且再次锁定同一个东西相当于解锁
                {
                    LockProcessA(null, false, false, isAI);
                    break;
                }
                LockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }
    }

    public void ChangeLock(bool isLeft)//只需确定左右且确定已锁定，则能锁定该方向的另一个敌人
    {
        changeLockEnable = false;
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);//model's eyes
        Collider[] cols1 = Physics.OverlapBox(modelOrigin2,
            new Vector3(6.0f, 6.0f, 10.0f), model.transform.rotation,
            LayerMask.GetMask("Enemy"));
        //我尝试在cols1中找到左边的或右边的一个敌人
        if (cols1.Length == 0)
        {
            lockTarget = null;
            lockDot.enabled = false;
            lockState = false;
        }
        else
        {
            float[] colslen = new float[cols1.Length];
            for (int i = 0; i < cols1.Length; i++)
            {
                colslen[i] = Vector3.Cross(model.transform.forward, cols1[i].transform.position).y;//左负右正
            }
            if ((!isLeft && colslen.Max() <= 0) || (isLeft && colslen.Min() >= 0)) { return; }//找不到就退出~

            foreach (var col in cols1)
            {
                if (!isLeft && Vector3.Cross(model.transform.forward, col.transform.position).y > 0)
                {
                    lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
                    lockDot.enabled = true;
                    lockState = true;
                    break;
                }
                if (isLeft && Vector3.Cross(model.transform.forward, col.transform.position).y < 0)
                {
                    lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
                    lockDot.enabled = true;
                    lockState = true;
                    break;
                }

            }
        }

    }

    private class LockTarget//此私有类可保存多个相关数据
    {
        public GameObject obj;
        public float halfHeight;//unity中半高比身高读起来方便
        public ActorManager am;

        public LockTarget(GameObject _obj)//构造函数
        {
            obj = _obj;
            am = _obj.GetComponent<ActorManager>();
            //this.obj = _obj; //参数同名时用this区分，此处有——了即不用
        }
        public LockTarget(GameObject obj, float _halfHeight) : this(obj)
        {
            this.halfHeight = _halfHeight;
            am = obj.GetComponent<ActorManager>();
        }//here is dif,ref to "S2 P26 15:15"
    }
}
