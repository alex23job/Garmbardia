using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductResourseRepository : MonoBehaviour
{
    [SerializeField] private ProductResourse[] _productResourses;
    [SerializeField] private LevelBoard _levelBoard;

    public int Count { get {  return _productResourses.Length; } }

    public ProductResourse GetResourseByID(int id)
    {
        foreach (var item in _productResourses)
        {
            if (item.ID == id) return item;
        }
        return new ProductResourse(-1, -1);
    }

    public bool CheckResourseAccessByID(int id, int y, int x)
    {
        if (_levelBoard != null)
        {
            int boardSize = _levelBoard.BoardSize;
            if (boardSize != -1)
            {
                int div = (boardSize == 35) ? 4 : 2;
                int row = y / div, col = x / div;
                int index = row * boardSize + col;
                foreach (var item in _productResourses)
                {
                    if (item.ID == id)
                    {
                        int type = item.LandOrBuildID / 1000;
                        int itemID = item.LandOrBuildID % 1000;
                        //print($"CheckResourseAccessByID  <id={id} y={y} x={x}> <boardSize={boardSize} div={div} row={row} col={col} index={index}> <type={type} itemID={itemID}>");
                        switch(type)
                        {
                            case 0:
                                if (itemID == 0) return _levelBoard.CheckDoor(row, col);    //  дорога
                                if (itemID == 2) return _levelBoard.CheckForest(row, col);  //  лес
                                if ((itemID == 1) || (itemID == 3) || (itemID == 4)) return _levelBoard.CheckLand(itemID, index);   //  в горе, у воды, на песке ?
                                break;
                            case 1:
                                return _levelBoard.CheckBuildingInRadius(itemID, row, col); //  нужное производство в радиусе и соединено дорогой
                                //break;
                        }
                    }
                } 
            }
        }
        return false;
    }
}

[Serializable]
public struct ProductResourse
{
    public int ID;
    public string Name;
    public Sprite Icon;
    /// <summary>
    /// тип местности или здания, в котором производится ресурс => значение = % 1000
    /// тип получения информации: /1000 = 0 - наличие в соседней клетке, 1 - тип местности этой клетки, 2 - тип производственного здания
    /// </summary>
    public int LandOrBuildID;

    public ProductResourse(int id, int lobi, string nm = "", Sprite sprite = null) 
    {
        ID = id;
        Name = nm;
        Icon = sprite;
        LandOrBuildID = lobi;
    }
}
