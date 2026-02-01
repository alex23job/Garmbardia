using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float _ofsX = 20f;
    [SerializeField] private float _ofsY = 21f;
    [SerializeField] private float _ofsZ = 20f;
    [SerializeField] private float _halfSz = 10f;
    [SerializeField] private float _minSz = 8f;
    [SerializeField] private float _maxSz = 29f;
    [SerializeField] private float _compZ = 0.2f;

    private Camera _camera;
    private int _quadrant = 0;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void SetQuadrant(int num)
    //{
    //    _quadrant = num;
    //    Vector3 delta = new Vector3((num % 3) * (_ofsX * -1) + _ofsX, _ofsY, 0.5f * _ofsZ - (num / 3) * _ofsZ);
    //    delta.x += 0.6f * (_camera.orthographicSize - _minSz);
    //    delta.z -= 0.35f * (_camera.orthographicSize - _minSz);
    //    transform.position = delta;
    //}

    //public void ChangeSize(float value)
    //{
    //    //Vector3 pos = new Vector3((_quadrant % 3) * (_ofsX * -1) + _ofsX - value, _ofsY, 0.5f * _ofsZ - (_quadrant / 3) * _ofsZ - value);
    //    Vector3 pos = transform.position;
    //    float sz = _camera.orthographicSize + value;
    //    sz = Mathf.Clamp(sz, _minSz, _maxSz);
    //    if (Mathf.Abs(sz - _camera.orthographicSize) > 0.01f)
    //    {
    //        _camera.orthographicSize = sz;
    //        pos.x += value * 1.2f; pos.z -= value * 0.7f;
    //        transform.position = pos;
    //        print($"ortSz={sz}   pos={pos}   trPos={transform.position}   value={value}");
    //    }
    //}

    public void SetQuadrant(int num)
    {
        _quadrant = num;
        float col = num % 3;            // номер колонки (0, 1, 2)
        float row = num / 3;            // номер строки (0, 1, 2)
        float sz = _camera.orthographicSize;

        // Вычисляем положение камеры для выбранного квадранта
        //Vector3 delta = new Vector3(-col * _ofsX + _ofsX * 1.5f, _ofsY, row * _ofsZ - _ofsZ * 1.5f);
        //Vector3 delta = new Vector3(_ofsX - col * _ofsX, _ofsY, 1.5f * (_ofsZ - row * _ofsZ));
        float mult = _halfSz / (_maxSz - _minSz);
        float dx = (col - 1) * (sz - _minSz) * mult - (col - 1) * (_maxSz - sz) * 0.18f;
        float dz = (row - 1) * (sz - _minSz) * mult - _compZ * (2.2f * (row + 0.275f)) * (_maxSz - sz);

        //Vector3 delta = new Vector3(_ofsX + (col - 1) * 0.5f * (_maxSz - sz) + col * _halfSz, _ofsY, _ofsZ - (row - 1) * 0.5f * (_maxSz - sz) - row * _halfSz);
        Vector3 delta = new Vector3(_ofsX - dx + col * _halfSz, _ofsY, _ofsZ + dz - row * _halfSz);
        print($"sq  delta={delta}   dx={dx}   dz={dz}");
        transform.position = delta;
    }

    public void ChangeSize(float value)
    {
        float newSize = _camera.orthographicSize + value;
        newSize = Mathf.Clamp(newSize, _minSz, _maxSz);

        if (Mathf.Abs(newSize - _camera.orthographicSize) > 0.01f)
        {
            _camera.orthographicSize = newSize;

            // Пересчет позиции камеры, чтобы квадранты продолжали находиться в центре
            float col = _quadrant % 3;
            float row = _quadrant / 3;
            float mult = _halfSz / (_maxSz - _minSz);
            float dx = (col - 1) * (newSize - _minSz) * mult - (col - 1) * (_maxSz - newSize) * 0.18f;
            float dz = (row - 1) * (newSize - _minSz) * mult - _compZ * (2.2f * (row + 0.275f)) * (_maxSz - newSize);
            //Vector3 center = new Vector3(-col * _ofsX + _ofsX * 1.5f, _ofsY, row * _ofsZ - _ofsZ * 1.5f);
            //Vector3 center = new Vector3(_ofsX - col * _ofsX, _ofsY, 1.5f * (_ofsZ - row * _ofsZ));
            //Vector3 center = new Vector3(_ofsX + (col - 1) * 0.5f * (_maxSz - newSize) + col * _halfSz, _ofsY, _ofsZ - (row - 1) * 0.5f * (_maxSz - newSize) - row * _halfSz);
            Vector3 center = new Vector3(_ofsX - dx + col * _halfSz, _ofsY, _ofsZ + dz - row * _halfSz);
            print($"cs  delta={center}   dx={dx}   dz={dz}");
            transform.position = center;
        }
    }

}
