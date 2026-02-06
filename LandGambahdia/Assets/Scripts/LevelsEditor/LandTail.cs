using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private string[] _nameLandRu = new string[4] { "трава", "гора", "вода", "песок" };
    private string[] _nameLandEn = new string[4] { "grass", "mountain", "water", "sand" };
    private string[] _fillRu = new string[3] { "всё", "пополам", "угол" };
    private string[] _fillEn = new string[3] { "fill", "half", "angle" };


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

    public bool CmpPosition(int row, int col)
    {
        return (_tailInfo & 0xffff) == ((row << 8) + col);
    }

    public string GetTailInfo(string lang = "ru")
    {
        StringBuilder sb = new StringBuilder();
        int type = (_id >> 4) & 0x3, l1 = _id & 0x3, l2 = (_id >> 2) & 0x3;
        if (lang == "ru")
        {
            sb.Append($"Позиция ({(_tailInfo >> 8) & 0xff},{_tailInfo & 0xff}) ");
            if (type == 2) sb.Append($"{_fillRu[2]} {_nameLandRu[l1]}/{_nameLandRu[l2]}");
            else if (type == 1) sb.Append($"{_fillRu[1]} {_nameLandRu[l1]}/{_nameLandRu[l2]}");
            else sb.Append($"{_fillRu[0]} {_nameLandRu[l1]}");
            if (_isRotate) sb.Append($"Пов. {_rot * 90} гр.");
        }
        if (lang == "en")
        {
            sb.Append($"Position ({(_tailInfo >> 8) & 0xff},{_tailInfo & 0xff}) ");
            if (type == 2) sb.Append($"{_fillEn[2]} {_nameLandEn[l1]}/{_nameLandEn[l2]}");
            else if (type == 1) sb.Append($"{_fillEn[1]} {_nameLandEn[l1]}/{_nameLandEn[l2]}");
            else sb.Append($"{_fillEn[0]} {_nameLandEn[l1]}");
            if (_isRotate) sb.Append($"Rot. {_rot * 90} deg");
        }
        return sb.ToString();
    }

    public void RotateTail()
    {
        _rot++;
        _rot %= 4;
        _tailInfo = (_rot << 24) + (_tailInfo & 0xffffff);
        //print($"Rotate tail rot={_rot} tailInfo={_tailInfo}(0x{_tailInfo:X08})");
        transform.Rotate(0, 90, 0, Space.World);
    }

    private void OnMouseUp()
    {
        // Проверяем, попал ли клик в UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Игнорируем клик, если он попал в UI
            return;
        }

        if (_board != null) _board.TailSelect(gameObject);
    }
}
