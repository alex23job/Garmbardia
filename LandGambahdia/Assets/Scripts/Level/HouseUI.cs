using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HouseUI : MonoBehaviour
{
    [SerializeField] private RequirmentsManager _requirmentsManager;
    [SerializeField] private HouseTypeManager _houseTypeManager;
    [SerializeField] private GameObject _housePanel;
    [SerializeField] private Text _houseName;
    [SerializeField] private Text _txtInfo1;
    [SerializeField] private Text _txtInfo2;
    //[SerializeField] private Text _levelNumber;
    //[SerializeField] private Text _category;
    //[SerializeField] private Text _countPopule;
    //[SerializeField] private Text _nalog;
    //[SerializeField] private Text _cost;

    [SerializeField] private Image[] _imgReqs;
    [SerializeField] private Image[] _imgNextReqs;

    [SerializeField] private Text _houseNext;
    [SerializeField] private Text _nextCategory;
    [SerializeField] private Text _nextPopule;
    [SerializeField] private Text _nextNalog;
    [SerializeField] private Text _nextCost;

    [SerializeField] private Button _nextBtn;
    [SerializeField] private Button _prevBtn;
    [SerializeField] private Sprite _noSprite;

    private int _curNextNumHouse = 0;

    private static string[] _houseCategory = new string[] { "работники", "мастера", "синьоры"};

    private int[] _currentHouseRequirments = null;


    // Start is called before the first frame update
    void Start()
    {
        //_nextBtn.interactable = false;
        //_prevBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewHouseInfo(GameObject house)
    {
        if (_housePanel != null) { _housePanel.SetActive(true); }
        BuildingControl bc = house.GetComponent<BuildingControl>();
        HouseRequirement hr = house.GetComponent<HouseRequirement>();
        if ((bc != null) && (hr != null))
        {
            _houseName.text = bc.NameBuilding;
            _txtInfo1.text = $"Уровень : {1 + hr.HouseLevel}     Категория жителей : {_houseCategory[bc.BuildingID / 5]}";
            _txtInfo2.text = $"Жители : {hr.Citizens}/{hr.MaxCitizens}   Налог : {hr.Nalog}/{hr.MaxNalog}   Содержание (год) : {bc.ServiceCost}";
            //_levelNumber.text = $"{1 + hr.HouseLevel}";
            //_category.text = _houseCategory[bc.BuildingID / 5];
            //_countPopule.text = $"{hr.Citizens}/{hr.MaxCitizens}";
            //_nalog.text = $"{hr.Nalog}/{hr.MaxNalog}";
            //_cost.text = bc.ServiceCost.ToString();
            int[] totalReq = HouseRequirement.GetLevelRequirments(hr.HouseLevel);
            int[] curReq = hr.GetRequirements();
            _currentHouseRequirments = curReq;
            for (int i = 0; i < _imgReqs.Length; i++)
            {
                if (i < totalReq.Length)
                {
                    Requirments req = _requirmentsManager.GetRequirmentsByID(totalReq[i]);
                    if (req.Id != -1) _imgReqs[i].sprite = req.Icon;
                    else _imgReqs[i].sprite = _noSprite;
                    GameObject imgYes = _imgReqs[i].gameObject.transform.GetChild(1).gameObject;
                    GameObject imgNo = _imgReqs[i].gameObject.transform.GetChild(2).gameObject;
                    if (curReq.Contains(totalReq[i]))
                    {
                        imgYes.SetActive(true);
                        imgNo.SetActive(false);
                    }
                    else
                    {
                        imgNo.SetActive(true);
                        imgYes.SetActive(false);
                    }
                    _imgReqs[i].gameObject.SetActive(true);
                }
                else
                {
                    _imgReqs[i].gameObject.SetActive(false);
                }
            }
            if ((1 + hr.HouseLevel) < _houseTypeManager.CountHouseTypes)
            {
                _curNextNumHouse = 1 + hr.HouseLevel;
                _nextBtn.interactable = (_curNextNumHouse < (_houseTypeManager.CountHouseTypes - 1));
                _prevBtn.interactable = (_curNextNumHouse > 0);
            }
            else
            {
                _curNextNumHouse = 0;
                _nextBtn.interactable = true;
                _prevBtn.interactable = false;
            }
        }
        else
        {
            _currentHouseRequirments = null;
        }
        UpdateNextHouseInfo();
        //if (_housePanel != null) { _housePanel.SetActive(true); }
    }

    public void OnNextButtonClick()
    {
        if (_curNextNumHouse < (_houseTypeManager.CountHouseTypes - 1))
        {
            _curNextNumHouse++;
            _prevBtn.interactable = true;
        }
        _nextBtn.interactable = (_curNextNumHouse < (_houseTypeManager.CountHouseTypes - 1));
        UpdateNextHouseInfo();
    }

    public void OnPrevButtonClick()
    {
        if (_curNextNumHouse > 0) 
        {
            _curNextNumHouse--;
            _nextBtn.interactable = true;
        }
        _prevBtn.interactable = (_curNextNumHouse > 0);
        UpdateNextHouseInfo();
    }

    private void UpdateNextHouseInfo()
    {
        HouseInfo houseInfo = _houseTypeManager.GetHouseInfoByID(_curNextNumHouse);
        int i;
        if (houseInfo.LevelID == -1)
        {   //  нет такого дома
            _houseNext.text = houseInfo.Name;
            _nextCategory.text = "";
            _nextCost.text = "0";
            _nextPopule.text = "0";
            _nextNalog.text = "0";
            for (i = 0; i < _imgNextReqs.Length; i++)
            {
                if (i > 0)
                {
                    _imgNextReqs[i].gameObject.SetActive(false);
                }
                else
                {
                    GameObject imgYes = _imgNextReqs[i].gameObject.transform.GetChild(1).gameObject;
                    GameObject imgNo = _imgNextReqs[i].gameObject.transform.GetChild(2).gameObject;
                    imgYes.SetActive(false);
                    imgNo.SetActive(false);
                    _imgNextReqs[i].sprite = _noSprite;
                    _imgNextReqs[i].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            _houseNext.text = $"{houseInfo.LevelID + 1}. {houseInfo.Name}";
            _nextCategory.text = $"Жители : {_houseCategory[houseInfo.LevelID / 5]}";
            _nextCost.text = $"Содержание в год : {houseInfo.ServiceCost}";
            _nextPopule.text = $"Число жителей : {houseInfo.MaxCitizen}";
            _nextNalog.text = $"Налог : {houseInfo.OneCitizenNalog * houseInfo.MaxCitizen}";
            int[] reqs = HouseRequirement.GetLevelRequirments(houseInfo.LevelID);
            for (i = 0; i < _imgNextReqs.Length; i++)
            {
                if (i < reqs.Length)
                {
                    Requirments req = _requirmentsManager.GetRequirmentsByID(reqs[i]);
                    if (req.Id != -1) _imgNextReqs[i].sprite = req.Icon;
                    else _imgNextReqs[i].sprite = _noSprite;
                    GameObject imgYes = _imgNextReqs[i].gameObject.transform.GetChild(1).gameObject;
                    GameObject imgNo = _imgNextReqs[i].gameObject.transform.GetChild(2).gameObject;
                    if (_currentHouseRequirments != null)
                    {
                        if (_currentHouseRequirments.Contains(reqs[i]))
                        {
                            imgYes.SetActive(true);
                            imgNo.SetActive(false);
                        }
                        else
                        {
                            imgYes.SetActive(false);
                            imgNo.SetActive(true);
                        }
                    }
                    else
                    {
                        imgYes.SetActive(false);
                        imgNo.SetActive(false);
                    }
                    _imgNextReqs[i].gameObject.SetActive(true);
                }
                else
                {
                    _imgNextReqs[i].gameObject.SetActive(false);
                }
            }
        }
    }
}