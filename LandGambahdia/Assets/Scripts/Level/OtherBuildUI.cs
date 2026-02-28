using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherBuildUI : MonoBehaviour
{
    [SerializeField] private GameObject _otherBuildPanel;

    [SerializeField] private Text _txtNameBuildLabel;
    [SerializeField] private Text _txtNameBuild;
    [SerializeField] private Text _txtInfo1;
    [SerializeField] private Text _txtInfo2;
    [SerializeField] private Text _txtInfo3;

    private string[] _category = { "Жилое здание:", "Муниципальные здание:", "Медицинское здание:", "Производственное здание:", "Дороги и мосты:", "Здание образования:", "Культура и спорт:", "Развлечения и украшенния:" };

    public void ViewBuildingInfo(GameObject build)
    {
        _otherBuildPanel.SetActive(true);
        BuildingControl bc = build.GetComponent<BuildingControl>();
        int index = (bc.BuildingID >> 5) & 0x7; 
        _txtNameBuildLabel.text = _category[index];
        _txtNameBuild.text = bc.NameBuilding;
        int prosperity = bc.Prosperity;
        if (prosperity < 0 ) prosperity = 0;
        _txtInfo1.text = $"Процветание : {prosperity}   Содержание (в год): {bc.ServiceCost}";
        ProductionSciencePoints psp = build.GetComponent<ProductionSciencePoints>();
        string sn = "";
        if (psp != null) sn = $"Производит очки наук (в месяц): {psp.CountPointsInMonth} шт.";
        _txtInfo2.text = sn;
        _txtInfo3.text = "";
    }
}
