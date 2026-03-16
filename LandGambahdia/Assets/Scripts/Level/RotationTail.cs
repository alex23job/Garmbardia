using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTail : MonoBehaviour
{
    [SerializeField] private GameObject _tail;
    [SerializeField] private float _rotationSpeed = 45f;
    /// <summary>
    /// Режим вращения: по кругу - false, от начала до конца и обратно - true
    /// </summary>
    [SerializeField] private bool _mode = false;
    [SerializeField] private float _angleStart;
    [SerializeField] private float _angleEnd;
    /// <summary>
    /// направление вращения: 0 - X, 1 - Y, 2 - Z
    /// </summary>
    [SerializeField] private int _axis = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_mode)
        {

        }
        else
        {
            Vector3 rot = _tail.transform.localRotation.eulerAngles;
            switch(_axis)
            {
                case 0: rot.x += Time.deltaTime * _rotationSpeed; if (rot.x > 360f) rot.x -= 360f; break;
                case 1: rot.y += Time.deltaTime * _rotationSpeed; if (rot.y > 360f) rot.y -= 360f; break;
                case 2: rot.z += Time.deltaTime * _rotationSpeed; if (rot.z > 360f) rot.z -= 360f; break;
            }
            _tail.transform.localRotation = Quaternion.Euler(rot);
        }
    }
}
