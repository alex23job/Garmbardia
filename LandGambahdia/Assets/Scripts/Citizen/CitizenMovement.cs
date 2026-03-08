using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CitizenMovement : MonoBehaviour
{
    [SerializeField] private GameObject _cart;
    private LevelControl _levelControl = null;

    private bool _isNew = true;
    private bool _isFree = false;
    private bool _isWorker = false;
    private bool _isUsed = false;
    private Vector3 _startPosition;
    private Vector3 _target = Vector3.zero;
    private List<Vector3> _path = new List<Vector3>();
    private int _currentPoint = 0;
    private float _movementSpeed = 3f;
    private float _rotationSpeed = 5f;
    private float stoppingDistance = 0.2f;

    private Animator anim;
    private Rigidbody rb;

    public bool IsUsed { get => _isUsed; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetParams(LevelControl level, Vector3 start, List<Vector3> pt, bool isCart = true, float moveSpeed = 3f, float rotSpeed = 5f)
    {
        transform.position = start;
        _path = pt;
        _levelControl = level;
        _movementSpeed = moveSpeed;
        _rotationSpeed = rotSpeed;
        if (_path.Count > 0)
        {
            _currentPoint = 1;
            _target = _path[0];
            _isUsed = true;
            if (isCart == false) anim.SetBool("IsWalk", true);
            else anim.SetBool("IsCartWalk", true);
        }
    }

    public void SetPathToLabor(List<Vector3> pt)
    {
        _path = pt;
        if (_path.Count > 0)
        {
            _currentPoint = 1;
            _target = _path[0];
            _isUsed = true;
            anim.SetBool("IsCartWalk", false);
            anim.SetBool("IsWalk", true);
            _cart.SetActive(false);
            Invoke("ClearKinematic", 2f);
        }
    }

    public void SetPathToProduct(List<Vector3> pt)
    {
        _path = pt;
        if (_path.Count > 0)
        {
            _currentPoint = 1;
            _target = _path[0];
            _isUsed = true;
            anim.SetBool("IsCartWalk", false);
            anim.SetBool("IsWalk", true);
            _cart.SetActive(false);
            Invoke("ClearKinematic", 2f);
        }
    }

    private void ClearKinematic()
    {
        rb.isKinematic = false;
    }

    public void MoveCitizen(float dt)
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
            if (_path[_currentPoint - 1].x == _path[_currentPoint].x)
            {
                if (_path[_currentPoint - 1].z < _path[_currentPoint].z) _target.x += 0.15f; else _target.x -= 0.15f;
            }
            else
            {
                if (_path[_currentPoint - 1].x < _path[_currentPoint].x) _target.z -= 0.15f; else _target.z += 0.15f; 
            }
            _currentPoint++;
        }
        else
        {   //  сообщить что прошли путь до конца
            _isUsed = false;
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsCartWalk", false);
        }
    }
    private void MoveTowardsWaypoint(float dt)
    {
        Vector3 dir = _target - transform.position; dir.y = 0f;
        //rb.MovePosition(transform.position + dir.normalized * _movementSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + _movementSpeed * dt * dir.normalized);
        //rb.AddForce(dir.normalized * _movementSpeed * dt, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("House"))
        {
            //rb.isKinematic = true;
            HouseRequirement houseRequirement = other.gameObject.GetComponent<HouseRequirement>();
            if (houseRequirement != null && _isNew)
            {
                _isNew = false;
                houseRequirement.AddCitizen(1);
                houseRequirement.PushCitizen(gameObject);
            }
        }
        if (other.CompareTag("LaborExch"))
        {
            //rb.isKinematic = true;
            if ((_levelControl != null) && (_isFree == false))
            {
                _isFree = true;
                _levelControl.AddingFreeWorker(gameObject);
            }
        }
        if (other.CompareTag("Product"))
        {
            //rb.isKinematic = true;
            ProductionControl pc = other.gameObject.GetComponent<ProductionControl>();
            if ((pc != null) && (_isWorker == false))
            {
                _isWorker = true;
                pc.AddWorker(gameObject);
            }
        }
    }
}
