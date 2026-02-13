using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private LevelUI _levelUI;
    [SerializeField] private LevelBoard _levelBoard;
    [SerializeField] private LevelCamera _levelCamera;
    [SerializeField] private Slider _speedSlider;

    [SerializeField] private int _countSecondInMonth = 300;

    private LevelShema _levelShema;
    private LevelControl _levelControl;

    private List<GameObject> _buildingList = new List<GameObject>();
    private List<GameObject> _houseList = new List<GameObject>();

    private bool _isVictoryConditionsView = false;
    private float _speedGame = 1f;
    private int _many = 100;

    private float _timer = 1f;
    private int _countMonth = 0;
    private int _countSecond = 0;

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
        }
        _levelUI.SetSliderSpeed(_speedGame);
        _levelUI.ViewMany(_many);

        // Подписываемся на события
        _levelUI.OnSelectBuilding += OnSelectBuilding;
    }

    private void OnDisable()
    {
        //  Отписываемся от событий
        _levelUI.OnSelectBuilding -= OnSelectBuilding;
    }

    // Update is called once per frame
    void Update()
    {
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
            _levelUI.ViewConditionPanel(_isVictoryConditionsView, _levelShema.GetConditions());
        }
        else _levelUI.ViewConditionPanel(_isVictoryConditionsView);
    }

    public void OnSpeedSliderValueChanged()
    {
        _speedGame = _speedSlider.value;
        _levelUI.SetSliderSpeed(_speedGame);
    }

    private void OnSelectBuilding(int type, int num)
    {
        GameObject build = _levelBoard.CreateBuilding(type, num);
        if (build != null)
        {
            if (type > 0) _buildingList.Add(build);
            else _houseList.Add(build);
        }
    }

    public void TailSelect(GameObject tail)
    {
        if (_levelCamera != null)
        {
            _levelCamera.SetSelectTailPos(tail.transform.position);
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

    private void AddSecond()
    {
        _countSecond++;
        foreach(GameObject prod in _buildingList)
        {
            ProductionControl pc = prod.GetComponent<ProductionControl>();
            if (pc != null)
            {
                pc.AddSecond();
            }
        }
        if (_countSecond >= _countSecondInMonth)
        {
            _countMonth++;
            _levelUI.ViewCurrentTime(_countMonth);
            CalcProfit();
        }
    }

    private void CalcProfit()
    {

    }
}
