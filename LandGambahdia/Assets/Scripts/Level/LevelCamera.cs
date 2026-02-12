using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [SerializeField] private float _ofsX = 20f;
    [SerializeField] private float _ofsY = 21f;
    [SerializeField] private float _ofsZ = 20f;
    [SerializeField] private float _halfSz = 10f;
    [SerializeField] private float _minSz = 8f;
    [SerializeField] private float _maxSz = 29f;
    [SerializeField] private float _compZ = 0.2f;
    [SerializeField] private float _lerpRate = 0.1f;

    private Camera _camera;
    private int _quadrant = 0;
    private Vector3 _selectTailPos = new Vector3(0, 2f, 0);

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        // Получить вращение колеса мыши
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAmount < 0) ChangeSize(0.5f);
        if (scrollAmount > 0) ChangeSize(-0.5f);
        ChangeCameraPos();
    }

    public void ChangeSize(float value)
    {
        float newSize = _camera.orthographicSize + value;
        newSize = Mathf.Clamp(newSize, _minSz, _maxSz);

        if (Mathf.Abs(newSize - _camera.orthographicSize) > 0.01f)
        {
            _camera.orthographicSize = newSize;

            ChangeCameraPos();
            /*
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
            //print($"cs  delta={center}   dx={dx}   dz={dz}");
            transform.position = center;
            */
        }
    }

    private void ChangeCameraPos()
    {
        Vector3 center = new Vector3(_selectTailPos.x, _ofsY, _ofsZ + _selectTailPos.z);
        transform.position = Vector3.Lerp(transform.position, center, _lerpRate);
    }

    public void SetSelectTailPos(Vector3 pos)
    {
        _selectTailPos = pos;
        ChangeCameraPos();
    }
}
