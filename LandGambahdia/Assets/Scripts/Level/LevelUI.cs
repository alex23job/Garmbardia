using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    // Делегат для уведомления о выборе здания для постройки
    public delegate void SelectBuildingEventHandler(int type, int num);
    public event SelectBuildingEventHandler OnSelectBuilding;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _levelInfo;

    [SerializeField] private GameObject _buildingPanel;
    [SerializeField] private Button[] _buildingsBtn;
    
    private int _selectCategory = -1;
    private BuildingInfo[] _buildingInfos = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewLevelInfo(LevelShema ls)
    {
        _levelInfo.text = $"{ls.NumberLevel}. {ls.Name}";
    }

    public void SetBuildingInfo(BuildingInfo[] arr)
    {
        _buildingInfos = arr;
    }

    public void OnBuildingClick()
    {
        _buildingPanel.SetActive(true);
        _selectCategory = 0;
        UpdateBuildingPanel();
    }

    public void SelectCategory(int cat)
    {
        _selectCategory = cat;
        UpdateBuildingPanel();
    }

    private void UpdateBuildingPanel()
    {
        List<BuildingInfo> buildings = new List<BuildingInfo>();
        foreach (BuildingInfo buildingInfo in _buildingInfos)
        {
            if (((buildingInfo.Id >> 5) & 0x7) == _selectCategory)
            {
                buildings.Add(buildingInfo);
            }
        }
        //if (buildings.Count > 0)
        //{
            for (int i = 0; i < _buildingsBtn.Length; i++)
            {
                if (i < buildings.Count)
                {
                    _buildingsBtn[i].gameObject.SetActive(true);
                    Image img = _buildingsBtn[i].gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
                    if (img != null) img.sprite = buildings[i].Sprite;
                Text txtName = _buildingsBtn[i].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                if (txtName != null) txtName.text = $"{buildings[i].Name}";
                Text txtMany = _buildingsBtn[i].gameObject.transform.GetChild(2).gameObject.GetComponent<Text>();
                if (txtMany != null) txtMany.text = $"{buildings[i].Price}";
            }
            else
                {
                    _buildingsBtn[i].gameObject.SetActive(false);
                }
            }
        //}
    }

    public void SelectBuilding(int num)
    {
        _buildingPanel.SetActive(false);
        if (_selectCategory != -1)
        {
            OnSelectBuilding?.Invoke(_selectCategory, num);
            _selectCategory = -1;
        }
    }

    public void OnSaveLevelClick()
    {

    }

    public void OnExitClick()
    {
        _pausePanel.SetActive(false);
        SceneManager.LoadScene("MainScene");
    }
}
