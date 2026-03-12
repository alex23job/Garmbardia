using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WorkerMovement : MonoBehaviour
{
    [SerializeField] private GameObject _cart;
    [SerializeField] private GameObject _prod = null;

    private bool _isUsed = false;
    private Vector3 _target = Vector3.zero;
    private List<Vector3> _path = new List<Vector3>();
    private int _currentPoint = 0;
    private float _movementSpeed = 3f;
    private float _rotationSpeed = 5f;
    private float stoppingDistance = 0.2f;

    private int _resourseID = -1;

    /// <summary>
    /// 0 - inpResourse, 1 - outResourse
    /// </summary>
    private int _resourseMode = -1;
    private int _resourseCount = 0;

    private Animator anim;
    private Rigidbody rb;

    public int ResourseID { get => _resourseID; } 

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLoopPath(List<Vector3> pt, int resID, int resMode, GameObject goRes = null)
    {
        print($"SetLoopPath path.Count={pt.Count} resID={resID}  resMode={resMode}");
        _resourseID = resID;
        _resourseMode = resMode;
        if (goRes != null)
        {   //  нужно сделать образ ресурса для перевозки в повозке

        }
        _path = pt;
        if (_path.Count > 0)
        {
            _currentPoint = 1;
            _target = _path[0];
            _isUsed = true;
            anim.SetBool("IsCartWalk", true);
            anim.SetBool("IsWalk", false);
            _cart.SetActive(true);
        }

    }

    public void MoveWorker(float dt)
    {
        if (_isUsed == false) return;
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 tg = new Vector2(_target.x, _target.z);
        if (Vector2.Distance(pos, tg) < stoppingDistance)
        {
            NextPoint();
        }
        else
        {
            LookAtWaypoint(dt);
            MoveTowardsWaypoint(dt);
        }
    }
    private void LookAtWaypoint(float dt)
    {
        Vector3 dir = _target - transform.position; dir.y = 0f;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _rotationSpeed * dt);
    }

    private void NextPoint()
    {
        if (_currentPoint < _path.Count)
        {
            _target = _path[_currentPoint];
            if (_currentPoint < _path.Count - 1) rb.isKinematic = false;
            if (_currentPoint > 0)
            {
                if (_path[_currentPoint - 1].x == _path[_currentPoint].x)
                {
                    if (_path[_currentPoint - 1].z < _path[_currentPoint].z) _target.x += 0.15f; else _target.x -= 0.15f;
                }
                else
                {
                    if (_path[_currentPoint - 1].x < _path[_currentPoint].x) _target.z -= 0.15f; else _target.z += 0.15f;
                }
            }
            _currentPoint++;
        }
        else
        {   //  сообщить что прошли путь до конца
            _currentPoint = 0;
            //_isUsed = false;
            //anim.SetBool("IsWalk", false);
            //anim.SetBool("IsCartWalk", false);
        }
    }
    private void MoveTowardsWaypoint(float dt)
    {
        Vector3 dir = _target - transform.position; dir.y = 0f;
        rb.MovePosition(transform.position + _movementSpeed * dt * dir.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Product"))
        {
            ProductionControl pc = other.gameObject.GetComponent<ProductionControl>();
            if (pc != null)
            {
                if (pc.CheckInputResourseByID(_resourseID) && _resourseMode == 0)
                {   //  доставлен ресурс для производства (например, Ткань в Ателье)
                    if (_resourseCount > 0)
                    {   // нужно добавить к ресурсам для производства

                    }
                    _resourseCount = 0;
                }
                if (pc.CheckOutputResourseByID(_resourseID) && _resourseMode == 1)
                {   //  загрузим выработанный ресурс
                    if (pc.TakeResourse()) _resourseCount = 1;
                }
            }
            MultiProduct mp = other.gameObject.GetComponent<MultiProduct>();
            if (mp != null)
            {
                if (mp.CheckInputResourseByID(_resourseID) && _resourseMode == 1)
                {   //  доставлен ресурс на склад или в амбар, в порт и т.п.
                    mp.AddingResourseByID(_resourseID); //  пока не проверяем что _resourseCount == 1
                    _resourseCount = 0;
                }
                if (mp.CheckOutputResourseByID(_resourseID) && _resourseMode == 0)
                {   //  пытаемся получить ресурс со склада, амбара, порта и т.д.
                    if (mp.TakeResourseByID(_resourseID, 1) == 1) _resourseCount = 1;
                }
            }
        }
    }
}
