using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTypeManager : MonoBehaviour
{
    [SerializeField] private List<HouseInfo> _houseInfos = new List<HouseInfo>();

    public int CountHouseTypes { get { return _houseInfos.Count; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HouseInfo GetHouseInfoByID(int id)
    {
        foreach (HouseInfo houseInfo in _houseInfos)
        {
            if (houseInfo.LevelID == id) return houseInfo;
        }
        return new HouseInfo(-1, "Неизвестный дом", 0, 0, 0);
    }
}

[Serializable]
public struct HouseInfo
{
    [SerializeField] public int LevelID;
    [SerializeField] public string Name;
    [SerializeField] public int MaxCitizen;
    [SerializeField] public int OneCitizenNalog;
    [SerializeField] public int ServiceCost;

    public HouseInfo(int id, string nm, int count, int nalog, int cost)
    {
        LevelID = id;
        Name = nm;
        MaxCitizen = count;
        OneCitizenNalog = nalog;
        ServiceCost = cost;
    }
}
