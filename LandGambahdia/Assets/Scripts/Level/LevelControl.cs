using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private HouseUI _houseUI;
    [SerializeField] private ScienceUI _cienceUI;
    [SerializeField] private LevelUI _levelUI;
    [SerializeField] private LevelBoard _levelBoard;
    [SerializeField] private LevelCamera _levelCamera;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private SpawnSitizen _spawnSitizen;

    [SerializeField] private int _countSecondInMonth = 300;

    private LevelShema _levelShema;
    private LevelControl _levelControl;

    private List<GameObject> _buildingList = new List<GameObject>();
    private List<GameObject> _houseList = new List<GameObject>();
    private List<int> _freePlacesIndex = new List<int>();

    private List<GameObject> _citizens = new List<GameObject>();
    private List<CitizenMovement> _citizenMovements = new List<CitizenMovement>();

    private List<VictoryCondition> _victoryConditions = new List<VictoryCondition>();
    private bool _isVictoryConditionsView = false;
    private float _speedGame = 1f;
    private int _many = 200;
    private int _scienceCount = 3;
    private List<ProductionSciencePoints> _productionSciencePoints = new List<ProductionSciencePoints>(); 

    private float _timer = 1f;
    private int _countMonth = 0;
    private int _countSecond = 0;

    private int _countProsperity = 0;
    private int _countCitizens = 0;
    private int _freePlaces = 0;
    private int _vacancy = 0;
    private int _totalServiceCost = 0;
    private int _totalNalog = 0;
    private bool _isWin = false;
    private bool _isLoss = false;

    private GameObject _selectBuild = null;

    private void Awake()
    {
        _levelControl = GetComponent<LevelControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _levelShema = LevelList.Instance.GetShemaLevel(GameManager.Instance.currentPlayer.currentLevel);
        if (_levelShema != null)
        {
            if (_levelUI != null) _levelUI.ViewLevelInfo(_levelShema);
            if (_levelBoard != null) _levelBoard.ViewCurrentLevel(_levelShema, _levelControl);
            _victoryConditions = _levelShema.GetConditions();
            _levelUI.ViewConditionPanel(false, _victoryConditions);
            if (_spawnSitizen != null && _levelBoard != null) _spawnSitizen.SetLevelParams(_levelControl, _levelBoard.SpawnPosition);

            //_levelUI.ViewWinPanel(_victoryConditions, _levelShema.GetBonuses());
            //_levelUI.ViewLossPanel(_victoryConditions);
        }
        _levelUI.SetSliderSpeed(_speedGame);
        _levelUI.ViewMany(_many);
        _levelUI.ViewScienceCount(_scienceCount);

        // Подписываемся на события
        _levelUI.OnSelectBuilding += OnSelectBuilding;
        _cienceUI.OnInvestedPointsClick += OnInvestedPointsClick;
    }

    private void OnDisable()
    {
        //  Отписываемся от событий
        _levelUI.OnSelectBuilding -= OnSelectBuilding;
        _cienceUI.OnInvestedPointsClick -= OnInvestedPointsClick;
    }

    // Update is called once per frame
    void Update()
    {
        if (_citizenMovements.Count > 0)
        {
            foreach(CitizenMovement movement in _citizenMovements)
            {
                movement.MoveCitizen();
            }
        }
        if (_timer > 0) _timer -= Time.deltaTime;
        else
        {
            _timer = 1f;
            AddSecond();
        }
    }

    public void OnConditionClick()
    {
        _isVictoryConditionsView = !_isVictoryConditionsView;
        if (_isVictoryConditionsView && _levelShema != null)
        {
            _levelUI.ViewConditionPanel(_isVictoryConditionsView, _victoryConditions);
        }
        else _levelUI.ViewConditionPanel(_isVictoryConditionsView);
    }

    public void OnSpeedSliderValueChanged()
    {
        _speedGame = _speedSlider.value;
        _levelUI.SetSliderSpeed(_speedGame);
    }

    public void AddingWoodDoor(GameObject door)
    {
        _buildingList.Add(door);
    }

    private void OnSelectBuilding(int type, int num)
    {
        GameObject build = _levelBoard.CreateBuilding(type, num);
        if (build != null)
        {
            if (type > 0)
            {
                _buildingList.Add(build);
                BuildingControl bc = build.GetComponent<BuildingControl>();
                if (bc != null)                    
                {
                    if (bc.Requirment != -1)
                    {
                        int row = (bc.BuildingInfo >> 8) & 0xff;
                        int col = bc.BuildingInfo & 0xff;
                        int multRadius = (_levelShema.BoardSize == 35) ? 4 : 2;
                        foreach (GameObject house in _houseList)
                        {
                            HouseRequirement houseRequirement = house.GetComponent<HouseRequirement>();
                            if (houseRequirement != null) houseRequirement.AddRequirement(bc.Requirment, row, col, bc.Radius * multRadius);
                        }
                        CheckHouses();
                    }
                    if (bc.Prosperity > 0)
                    {
                        _countProsperity += bc.Prosperity;
                        foreach (VictoryCondition vc in _victoryConditions)
                        {
                            if (vc.NameConditionCategory == "Процветание")
                            {
                                vc.SetValue(_countProsperity);
                            }
                        }
                    }
                }
                ProductionControl pc = build.GetComponent<ProductionControl>();
                if (pc != null)
                {
                    _vacancy += pc.Vacancy;
                }
                if (_spawnSitizen != null)
                {
                    _spawnSitizen.CalcInterval(_countProsperity, _vacancy, _freePlaces);
                }
                ProductionSciencePoints productionSciencePoints = build.GetComponent<ProductionSciencePoints>();
                if (productionSciencePoints != null)
                {
                    _productionSciencePoints.Add(productionSciencePoints);
                }
            }
            else
            {
                _houseList.Add(build);
                
                HouseRequirement houseRequirement = build.GetComponent<HouseRequirement>();
                if (houseRequirement != null)
                {
                    houseRequirement.SetLevelControl(_levelControl);
                    int indexHouse = GetIndexBuilding(build);
                    if (indexHouse != -1) for (int i = 0; i < houseRequirement.FreePlaces; i++) _freePlacesIndex.Add(indexHouse);
                    foreach (GameObject go in _buildingList)
                    {
                        BuildingControl bc = go.GetComponent<BuildingControl>();
                        if (bc != null && bc.Requirment != -1)
                        {
                            int row = (bc.BuildingInfo >> 8) & 0xff;
                            int col = bc.BuildingInfo & 0xff;
                            int multRadius = (_levelShema.BoardSize == 35) ? 4 : 2;
                            houseRequirement.AddRequirement(bc.Requirment, row, col, bc.Radius * multRadius);
                        }
                    }
                    _freePlaces += houseRequirement.FreePlaces;
                    if (_spawnSitizen != null)
                    {
                        _spawnSitizen.CalcInterval(_countProsperity, _vacancy, _freePlaces);
                    }
                }
            }
        }
    }

    public void TailSelect(GameObject tail)
    {
        if (_levelCamera != null)
        {
            _levelCamera.SetSelectTailPos(tail.transform.position);
            
            BuildingControl buildingControl = tail.GetComponent<BuildingControl>();
            if (buildingControl != null && _levelUI != null)
            {
                _selectBuild = tail;
                _levelUI.ViewActionsPanel(buildingControl.NameBuilding, buildingControl.IsRotate);
            }
        }
    }

    public void TailRemove()
    {
        if (_selectBuild != null)
        {
            
        }
    }

    public void TailViewInfo()
    {
        if (_selectBuild != null)
        {
            HouseRequirement houseRequirement = _selectBuild.GetComponent<HouseRequirement>();
            if (houseRequirement != null) _houseUI.ViewHouseInfo(_selectBuild);
            ProductionControl productionControl = _selectBuild.GetComponent<ProductionControl>();
            if (productionControl != null) { }  //  показ информации о производстенном здании
        }
    }

    public void TranslateBuildingInfo(BuildingInfo[] arr)
    {
        _levelUI.SetBuildingInfo(arr);
    }

    public void ViewError(string errTxt)
    {
        _levelUI.ViewErrorPanel(errTxt);
    }

    public bool ChangeMany(int count)
    {
        if (count > _many) return false;
        _many += count;
        _levelUI.ViewMany(_many);
        return true;
    }

    public bool CheckMany(int count)
    {
        return _many >= count;
    }

    private int OnInvestedPointsClick(int delta)
    {
        if (_scienceCount >= delta)
        {
            _scienceCount -= delta;
            _levelUI.ViewScienceCount(_scienceCount);
            return delta;
        }
        else
        {
            int res = _scienceCount;
            _scienceCount = 0;
            _levelUI.ViewScienceCount(_scienceCount);
            return res;
        }
    }

    private void AddSecond()
    {
        _countSecond++;
        if (_spawnSitizen != null)
        {
            GameObject nc = _spawnSitizen.SpawnCitizen();
            if (nc != null)
            {   //  передать маршрут движения к свободному жилью в жителя
                CitizenMovement movement = nc.GetComponent<CitizenMovement>();
                if (movement != null) _citizenMovements.Add(movement);
                _citizens.Add(nc);
            }
        }
        foreach(GameObject prod in _buildingList)
        {
            ProductionControl pc = prod.GetComponent<ProductionControl>();
            if (pc != null)
            {
                pc.AddSecond();
            }
        }
        if (_levelUI != null) _levelUI.ViewFillingMonth((float)_countSecond / (float)_countSecondInMonth);
        if (_countSecond >= _countSecondInMonth)
        {
            _countSecond = 0;
            _countMonth++;
            if (_levelUI != null) _levelUI.ViewCurrentTime(_countMonth);
            SumProsperity();
            CheckHouses();
            CalcProfit(_countMonth % 12 == 0);

            foreach (VictoryCondition vc in _victoryConditions)
            {
                if (vc.NameConditionCategory == "Время")
                {
                    vc.SetValue(_countMonth / 12);
                }
            }

            foreach(ProductionSciencePoints psp in _productionSciencePoints)
            {
                _scienceCount += psp.CountPointsInMonth;
            }
            if (_levelUI != null) _levelUI.ViewScienceCount(_scienceCount);

            if (TestEndGame())
            {   //  конец игры что-то нужно сделать кроме показа финишных панелей ?!
                if (_isWin) _levelUI.ViewWinPanel(_victoryConditions, _levelShema.GetBonuses());
                if (_isLoss) _levelUI.ViewLossPanel(_victoryConditions);
            }
            else
            {
                if (_spawnSitizen != null)
                {
                    _spawnSitizen.CalcInterval(_countProsperity, _vacancy, _freePlaces);
                }
            }
        }
    }

    private bool TestEndGame()
    {
        int countYes = 0;
        bool isEndTime = false;
        foreach (VictoryCondition vc in _victoryConditions)
        {
            if (vc.NameConditionCategory == "Деньги")
            {
                if (vc.Value > vc.Count) countYes++;
                if (vc.Value < 0) _isLoss = true;
            }
            else if (vc.NameConditionCategory == "Время")
            {
                if (vc.Value < vc.Count) countYes++;
                if (vc.Count <= vc.Value)
                {   //  Отведённое время закончилось и что ?
                    //  Если все остальные условия выполнены, то победа
                    isEndTime = true;
                    //if (countYes + 1 == _victoryConditions.Count) _isWin = true;
                }
            }
            else
            {
                if (vc.Value > vc.Count) countYes++;
            }
        }
        if (countYes == _victoryConditions.Count)
        {
            _isWin = true;
            return true;
        }
        if (isEndTime)
        {
            if (countYes + 1 == _victoryConditions.Count) _isWin = true;
            else _isLoss = true;
            return true;
        }
        if (_isLoss) return true;
        return false;
    }

    private void SumProsperity()
    {
        _countProsperity = 0;
        _totalServiceCost = 0;
        _vacancy = 0;
        foreach(GameObject build in _buildingList)
        {
            BuildingControl bc = build.GetComponent<BuildingControl>();
            if (bc != null && bc.Prosperity != -1) 
            {
                _countProsperity += bc.Prosperity;
                _totalServiceCost += bc.ServiceCost;
            }
            ProductionControl productionControl = build.GetComponent<ProductionControl>();
            if (productionControl != null)
            {
                _vacancy += productionControl.Vacancy;
            }
        }
        foreach(GameObject build in _houseList)
        {
            BuildingControl bc = build.GetComponent<BuildingControl>();
            if (bc != null && bc.Prosperity != -1)
            {
                _countProsperity += bc.Prosperity;
            }
        }
        foreach(VictoryCondition vc in _victoryConditions)
        {
            if (vc.NameConditionCategory == "Процветание")
            {
                vc.SetValue(_countProsperity);
            }
        }
    }

    private void CheckHouses()
    {
        _countCitizens = 0;
        _freePlaces = 0;
        _totalNalog = 0;
        for(int i = _houseList.Count; i > 0; i--)
        {
            HouseRequirement houseRequirement = _houseList[i - 1].GetComponent<HouseRequirement>();
            if (houseRequirement != null && houseRequirement.CheckLevelRequirments())
            {
                GameObject oldHouse = _houseList[i - 1];
                GameObject newHouse = _levelBoard.UpdateHouse(oldHouse);
                if (newHouse != null)
                {
                    _houseList[i - 1] = newHouse;
                    // надо перепрописать жителей, при увеличении мест - добавить в список свободных
                    HouseRequirement newHouseRequirement = newHouse.GetComponent<HouseRequirement>();
                    if (newHouseRequirement != null)
                    {
                        newHouseRequirement.SetLevelControl(_levelControl);
                        if (newHouseRequirement.FreePlaces > 0)
                        {
                            int index = GetIndexBuilding(newHouse);
                            if (index != -1)
                            {
                                for (int j = 0; j < newHouseRequirement.FreePlaces; j++) _freePlacesIndex.Add(index);
                            }
                        }
                    }
                    // при уменьшении - удалить из списка и отправить лишних жителей в другие дома со свободными местами или на выезд из города !!!
                    Destroy(oldHouse);
                }
            }
            houseRequirement = _houseList[i - 1].GetComponent<HouseRequirement>();
            _countCitizens += houseRequirement.Citizens;
            _freePlaces += houseRequirement.FreePlaces;
            _totalNalog += houseRequirement.Nalog;
        }
        foreach (VictoryCondition vc in _victoryConditions)
        {
            if (vc.NameConditionCategory == "Население")
            {
                vc.SetValue(_countCitizens);
            }
        }
    }

    private void CalcProfit(bool isYear)
    {
        if (isYear)
        {
            _many += _totalNalog - _totalServiceCost;
            if (_many < 0) _isLoss = true;
        }
        else
        {
            _many += _totalNalog;
        }
        bool isMany = false;
        foreach (VictoryCondition vc in _victoryConditions)
        {
            if (vc.NameConditionCategory == "Деньги")
            {
                vc.SetValue(_many);
                isMany = true;
            }
        }
        if ((isMany == false) && (_isLoss == true))
        {
            _victoryConditions.Add(new VictoryCondition("Деньги", 0, ""));
            _victoryConditions[_victoryConditions.Count - 1].SetValue(_many);
        }
        _levelUI.ViewMany(_many);
    }

    public bool GetPathToFreePlase(out List<Vector3> path)
    {
        path = new List<Vector3>();
        if (_freePlacesIndex.Count > 0)
        {
            int plaseIndex = _freePlacesIndex[0];
            _freePlacesIndex.RemoveAt(0);
            if (_levelBoard != null)
            {
                path = _levelBoard.GetCurPath(plaseIndex);
                return true;
            }
        }
        //foreach (GameObject house in _houseList)
        //{
        //    HouseRequirement houseRequirement = house.GetComponent<HouseRequirement>();
        //    if (houseRequirement != null && houseRequirement.FreePlaces > 0)
        //    {
        //        if (_levelBoard != null)
        //        {
        //            path = _levelBoard.GetCurPath(house.transform.position);
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    private int GetIndexBuilding(GameObject build)
    {
        BuildingControl buildingControl = build.GetComponent<BuildingControl>();
        if (buildingControl != null)
        {
            int div = (_levelShema.BoardSize == 35) ? 4 : 2;
            int indexHouse = (buildingControl.BuildingInfo & 0xff) / 4 + (((buildingControl.BuildingInfo >> 8) & 0xff) * _levelShema.BoardSize) / 4;
            return indexHouse;
        }
        return -1;
    }

    public void ChangeCitizen(GameObject house, GameObject citizen, bool isNew = true)
    {
        if (isNew)
        {
            _countCitizens++;
        }
        else
        {
            if (_countCitizens > 0) _countCitizens--;
        }
        print($"ChangeCitizen  count={_countCitizens}");
        foreach (VictoryCondition vc in _victoryConditions)
        {
            if (vc.NameConditionCategory == "Население")
            {
                vc.SetValue(_countCitizens);
            }
        }
        _levelUI.ViewConditionPanel(false, _victoryConditions);
    }
}
