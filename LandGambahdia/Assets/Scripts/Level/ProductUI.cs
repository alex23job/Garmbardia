using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductUI : MonoBehaviour
{
    [SerializeField] private ProductResourseRepository _productResourseRepository;
    [SerializeField] private GameObject _productBuildPanel;
    [SerializeField] private Text _txtNameBuild;
    [SerializeField] private Text _txtInfo1;
    [SerializeField] private Text _txtinfo2;

    [SerializeField] private Image[] _imgProds;
    [SerializeField] private Image[] _imgResourses;
    [SerializeField] private Sprite _noSprite;

    public void ViewBuildingInfo(GameObject build)
    {
        _productBuildPanel.SetActive(true);
        BuildingControl bc = build.GetComponent<BuildingControl>();
        _txtNameBuild.text = bc.NameBuilding;
        ProductionControl pc = build.GetComponent<ProductionControl>();
        if (pc != null)
        {
            _txtInfo1.text = $"Процветание : {bc.Prosperity}   Содержание (в год): {bc.ServiceCost}";
            _txtinfo2.text = $"Работники {pc.Workers} {pc.OneResourceCompleteTime}";
            int i, y = (bc.BuildingInfo >> 8) & 0xff, x = bc.BuildingInfo & 0xff;
            int[] prodID = pc.GetOutResoursesID();
            int[] resID = pc.GetInpResoursesID();
            int countYesRes = 0;
            for (i = 0; i < _imgResourses.Length; i++)
            {
                if (i < resID.Length)
                {
                    _imgResourses[i].gameObject.SetActive(true);
                    ProductResourse productResourse = _productResourseRepository.GetResourseByID(resID[i]);
                    if (productResourse.ID != -1)
                    {
                        _imgResourses[i].sprite = productResourse.Icon;
                        GameObject imgYes = _imgResourses[i].gameObject.transform.GetChild(1).gameObject;
                        GameObject imgNo = _imgResourses[i].gameObject.transform.GetChild(2).gameObject;
                        bool resYes = _productResourseRepository.CheckResourseAccessByID(resID[i], y, x);
                        imgYes.SetActive(resYes);
                        imgNo.SetActive(!resYes);
                        if (resYes) countYesRes++;
                    }
                    else
                    {
                        _imgResourses[i].sprite = _noSprite;
                    }
                }
                else
                {
                    _imgResourses[i].gameObject.SetActive(false);
                }
            }
            for (i = 0; i < _imgProds.Length; i++) 
            {
                if (i < prodID.Length)
                {
                    _imgProds[i].gameObject.SetActive(true);
                    ProductResourse product = _productResourseRepository.GetResourseByID(prodID[i]);
                    if (product.ID != -1)
                    {
                        _imgProds[i].sprite = product.Icon;
                    }
                    else
                    {
                        _imgProds[i].sprite = _noSprite;
                    }
                    GameObject imgYes = _imgProds[i].gameObject.transform.GetChild(1).gameObject;
                    GameObject imgNo = _imgProds[i].gameObject.transform.GetChild(2).gameObject;
                    imgYes.SetActive(countYesRes == resID.Length);
                    imgNo.SetActive(countYesRes != resID.Length);
                }
                else
                {
                    _imgProds[i].gameObject.SetActive(false);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
