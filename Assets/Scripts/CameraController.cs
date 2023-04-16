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
            //.Confined:������ȥ���� .Locked:�������(escȡ������) .None:���޸������Ϊ
        }

        lockState = false;

    }
    private void Start()
    {
        //��Ϊ�˱���awake��ac����Ĭ����������Ǹ�input���ǻ�Ծ��input~
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
            //�����ź�򶺺ţ����¼�������������ʾ����
            //cameraHandle.transform.Rotate(Vector3.right,pi.Jup * -verticalSpeed * Time.deltaTime);
            //tempEulerX = cameraHandle.transform.localEulerAngles.x;
            tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -20, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(
                tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;

        }

        else//����������
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;//ģ��ָ��Ŀ�귽��
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);//�������ŵס�
        }
        if (!isAI)
        {
            camera0.transform.position = Vector3.Lerp(camera0.transform.position, transform.position, 0.1f);
            //�˴�Ҳ����smoothdamp :...=Vector3.SmoothDamp(..,..,ref cameraDampVelocity,cameraDampValue);
            //ǰ��������ͬ�� �����������ֱ�Ϊprivate Vector3\public float

            //camera0.transform.eulerAngles = transform.eulerAngles;
            //��Ӱ�����Է���LateUpdate�Ҳ����tempEulerX -= �������Time.fixedDeltaTime����Update��
            //���ǲ��������������(Animator-UpdateMode==AnimatePhysics)��cameraHandle����
            camera0.transform.LookAt(cameraHandle.transform);
        }

    }

    private void Update()
    {
        if (lockTarget != null)
        {
            if (!isAI)
            {
                //�Ȱ������������õ����½�
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position
                    + new Vector3(0, lockTarget.halfHeight, 0));
                //�����ڽŵ׼Ӱ�ߣ����������У�
            }
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)//û��aiҲҪ���������
            {
                LockProcessA(null, false, false, isAI);
            }

            //ActorManager targetAm = lockTarget.obj.GetComponent<ActorManager>();
            if (lockTarget.am != null) //����AM��������״̬~
            {
                if(lockTarget.am.sm.isDie) //��������
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
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;//box��������5��֮��
        Collider[] cols = Physics.OverlapBox(boxCenter,
            new Vector3(0.5f, 0.5f, 5f), model.transform.rotation,
            LayerMask.GetMask(isAI?"Player":"Enemy"));

        /*
        //���������򳡾���Ͷ�����߲������������ж���ע�⣬��Щ�����˳��δ���塣
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
                if (lockTarget != null && lockTarget.obj == col.gameObject)//���������ٴ�����ͬһ�������൱�ڽ���
                {
                    LockProcessA(null, false, false, isAI);
                    break;
                }
                LockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }
    }

    public void ChangeLock(bool isLeft)//ֻ��ȷ��������ȷ�������������������÷������һ������
    {
        changeLockEnable = false;
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);//model's eyes
        Collider[] cols1 = Physics.OverlapBox(modelOrigin2,
            new Vector3(6.0f, 6.0f, 10.0f), model.transform.rotation,
            LayerMask.GetMask("Enemy"));
        //�ҳ�����cols1���ҵ���ߵĻ��ұߵ�һ������
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
                colslen[i] = Vector3.Cross(model.transform.forward, cols1[i].transform.position).y;//������
            }
            if ((!isLeft && colslen.Max() <= 0) || (isLeft && colslen.Min() >= 0)) { return; }//�Ҳ������˳�~

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

    private class LockTarget//��˽����ɱ������������
    {
        public GameObject obj;
        public float halfHeight;//unity�а�߱���߶���������
        public ActorManager am;

        public LockTarget(GameObject _obj)//���캯��
        {
            obj = _obj;
            am = _obj.GetComponent<ActorManager>();
            //this.obj = _obj; //����ͬ��ʱ��this���֣��˴��С����˼�����
        }
        public LockTarget(GameObject obj, float _halfHeight) : this(obj)
        {
            this.halfHeight = _halfHeight;
            am = obj.GetComponent<ActorManager>();
        }//here is dif,ref to "S2 P26 15:15"
    }
}
