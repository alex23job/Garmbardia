using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandTail : MonoBehaviour
{
    /// <summary>
    /// 0-1 - тип местности основной, 2-3 - тип местности дополнительный, 4-5 - вид местности
    /// 0 - трава, 1 - гора, 2 - вода, 3 - песок; 0 - весь, 1 - 50/50, 2 - угол 
    ///  > 90 - специальные типы
    /// </summary>
    [SerializeField] private int _id = -1;
    [SerializeField] private int _rot = 0;
    [SerializeField] private bool _isRotate = false;

    /// <summary>
    /// 0-1 - тип местности основной, 2-3 - тип местности дополнительный, 4-5 - вид местности
    /// 0 - трава, 1 - камень, 2 - вода, 3 - песок; 0 - весь, 1 - 50/50, 2 - угол 
    ///  > 90 - специальные типы
    /// </summary>
    public int TailID { get { return _id; } }
    public int TailRot { get { return _rot; } }
    public bool IsRotate { get { return _isRotate; } }
    /// <summary>
    /// 0-7 биты - столбец, 8-15 биты - строка, 16-23 - _id, 24-25 - _rot => 0 - 0, 1 - 90, 2 - 180, 3 - 270 
    /// </summary>
    public int TailInfo { get { return _tailInfo; } }

    private EditorBoard _board = null;
    /// <summary>
    /// 0-7 биты - столбец, 8-15 биты - строка, 16-23 - _id, 24-25 - _rot => 0 - 0, 1 - 90, 2 - 180, 3 - 270 
    /// </summary>
    private int _tailInfo = -1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{        
    //}

    public void SetBoardAndPosition(EditorBoard eb, int row, int col)
    {
        _board = eb;
        _tailInfo = (_id << 16) + (row << 8) + col;
        //print($"x={col} y={row} tailInfo={_tailInfo}(0x{_tailInfo:X08})    pos={transform.position}");
    }

    public bool CmpID(int num, int type = 9)
    {
        return (_id == (10 * type + num));
    }

    public void RotateTail()
    {
        _rot++;
        _rot %= 4;
        _tailInfo = (_rot << 24) + (_tailInfo & 0xffffff);
        print($"Rotate tail rot={_rot} tailInfo={_tailInfo}(0x{_tailInfo:X08})");
        transform.Rotate(0, 90, 0, Space.World);
    }

    private void OnMouseUp()
    {
        if (_board != null) _board.TailSelect(gameObject);
    }
}
