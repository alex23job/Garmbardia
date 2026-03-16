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
    [SerializeField] private Image[] _images;

    private string[] _category = { "Жилое здание:", "Муниципальные здание:", "Медицинское здание:", "Производственное здание:", "Дороги и мосты:", "Здание образования:", "Культура и спорт:", "Развлечения и украшенния:" };

    public void ViewBuildingInfo(GameObject build)
    {
        _otherBuildPanel.SetActive(true);
        BuildingControl bc = build.GetComponent<BuildingControl>();
        int i, index = (bc.BuildingID >> 5) & 0x7; 
        _txtNameBuildLabel.text = _category[index];
        _txtNameBuild.text = bc.NameBuilding;
        int prosperity = bc.Prosperity;
        if (prosperity < 0 ) prosperity = 0;
        _txtInfo1.text = $"Процветание : {prosperity}   Содержание (в год): {bc.ServiceCost}";
        ProductionSciencePoints psp = build.GetComponent<ProductionSciencePoints>();
        string sn = "";
        if (psp != null) sn = $"Производит очки наук (в месяц): {psp.CountPointsInMonth} шт.";
        ProductionControl pc = build.GetComponent<ProductionControl>();
        if (pc != null) sn = $"Сотрудники {pc.Workers}";
        for (i = 0; i < _images.Length; i++) _images[i].gameObject.SetActive(false);
        MultiProduct mp = build.GetComponent<MultiProduct>();
        if (mp != null)
        {
            _txtInfo3.gameObject.SetActive(false);
            sn = $"Сотрудники {mp.Workers}";
            SimpleResourse[] resourses = mp.OutResourses;
            for (i = 0; i < _images.Length; i++)
            {
                if (i < resourses.Length)
                {
                    _images[i].gameObject.SetActive(true);
                    Image imgRes = _images[i].transform.GetChild(0).gameObject.GetComponent<Image>();
                    if (imgRes != null) imgRes.sprite = resourses[i].Icon;
                    Text txtCount = _images[i].transform.GetChild(1).gameObject.GetComponent<Text>();
                    if (txtCount != null) txtCount.text = resourses[i].Count.ToString();
                    Image imgYes = _images[i].transform.GetChild(3).gameObject.GetComponent<Image>();
                    Image imgNo = _images[i].transform.GetChild(4).gameObject.GetComponent<Image>();
                    if (resourses[i].Count > 0)
                    {
                        if (imgYes != null) imgYes.gameObject.SetActive(true);
                        if (imgNo != null) imgNo.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (imgYes != null) imgYes.gameObject.SetActive(false);
                        if (imgNo != null) imgNo.gameObject.SetActive(true);
                    }
                }
            }
        }
        _txtInfo2.text = sn;
        _txtInfo3.text = "";
    }
}
