using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CitizenMovement : MonoBehaviour
{
    private LevelControl _levelControl = null;

    private bool _isNew = true;
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

    public void MoveCitizen()
    {
        if (_isUsed == false) return;
        Vector2 pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 tg = new Vector2(_target.x, _target.z);
        if (Vector3.Distance(pos, tg) < stoppingDistance)
        {
            NextPoint();
        }
        else
        {
            LookAtWaypoint();
            MoveTowardsWaypoint();
        }
    }
    private void LookAtWaypoint()
    {
        Vector3 dir = _target - transform.position; dir.y = 0f;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _rotationSpeed * Time.deltaTime);
    }

    private void NextPoint()
    {
        if (_currentPoint < _path.Count)
        {
            _target = _path[_currentPoint];
            _currentPoint++;
        }
        else
        {   //  сообщить что прошли путь до конца
            _isUsed = false;
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsCartWalk", false);
        }
    }
    private void MoveTowardsWaypoint()
    {
        Vector3 dir = _target - transform.position; dir.y = 0f;
        rb.MovePosition(transform.position + dir.normalized * _movementSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("House"))
        {
            rb.isKinematic = true;
            HouseRequirement houseRequirement = other.gameObject.GetComponent<HouseRequirement>();
            if (houseRequirement != null && _isNew)
            {
                _isNew = false;
                houseRequirement.AddCitizen(1);
                houseRequirement.PushCitizen(gameObject);
            }
        }
    }
}
