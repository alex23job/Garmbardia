using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingControl : MonoBehaviour
{
    /// <summary>
    /// (0-3 биты) 16 видов зданий дл€ (4-7 биты) каждой из 16ти категорий
    /// </summary>
    [SerializeField] private int _id = -1;
    [SerializeField] private int _rot = 0;
    [SerializeField] private bool _isRotate = false;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private String _nameBuilding;
    [SerializeField] private int _price;
    [SerializeField] private float _radius;

    /// <summary>
    /// (0-3 биты) 16 видов зданий дл€ (4-7 биты) каждой из 16ти категорий
    /// </summary>
    public int BuildingID { get { return _id; } }
    public int BuildingRot { get { return _rot; } }
    public bool IsRotate { get { return _isRotate; } }
    public Sprite BuildingSprite { get { return _sprite; } }
    public string NameBuilding { get => _nameBuilding; }
    public int Price { get => _price; }
    public float Radius { get => _radius; }

    /// <summary>
    /// 0-7 биты - столбец, 8-15 биты - строка, 16-23 - _id, 24-25 - _rot => 0 - 0, 1 - 90, 2 - 180, 3 - 270 
    /// </summary>
    private int _buildingInfo = -1;
    /// <summary>
    /// 0-7 биты - столбец, 8-15 биты - строка, 16-23 - _id, 24-25 - _rot => 0 - 0, 1 - 90, 2 - 180, 3 - 270 
    /// </summary>
    public int BuildingInfo { get { return _buildingInfo; } }


    private LevelBoard _levelBoard = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{        
    //}

    public void SetBoardAndPosition(LevelBoard lb, int row, int col)
    {
        _levelBoard = lb;
        _buildingInfo = (_id << 16) + (row << 8) + col;
        //print($"x={col} y={row} tailInfo={_tailInfo}(0x{_tailInfo:X08})    pos={transform.position}");
    }

    public BuildingInfo GetBuildingInfo()
    {
        return new BuildingInfo(_id,_rot, _isRotate, _nameBuilding, _sprite, _price);
    }

    public bool CmpPosition(int row, int col)
    {
        return (_buildingInfo & 0xffff) == ((row << 8) + col);
    }

    public void RotateTail()
    {
        _rot++;
        _rot %= 4;
        _buildingInfo = (_rot << 24) + (_buildingInfo & 0xffffff);
        //print($"Rotate tail rot={_rot} tailInfo={_tailInfo}(0x{_tailInfo:X08})");
        transform.Rotate(0, 90, 0, Space.World);
    }

    private void OnMouseUp()
    {
        // ѕровер€ем, попал ли клик в UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // »гнорируем клик, если он попал в UI
            return;
        }

        if (_levelBoard != null) _levelBoard.TailSelect(gameObject);
    }
}

[Serializable]
public class BuildingInfo
{
    private int _id;
    private int _rot;
    private bool _isRot;
    private string _name;
    private Sprite _sprite;
    private int _price;

    public int Id { get => _id; }
    public int Rot { get => _rot; }
    public bool IsRot { get => _isRot; }
    public string Name { get => _name; }
    public Sprite Sprite { get => _sprite; }
    public int Price { get => _price; }

    public BuildingInfo() { }
    public BuildingInfo(int id, int rot, bool isRot, string nm, Sprite spr, int pr)
    {
        _id = id;
        _rot = rot;
        _isRot = isRot;
        _name = nm;
        _sprite = spr;
        _price = pr;
    }
}
