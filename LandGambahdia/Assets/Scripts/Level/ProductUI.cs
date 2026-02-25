using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductUI : MonoBehaviour
{
    [SerializeField] private GameObject _productBuildPanel;
    [SerializeField] private Text _txtNameBuild;

    public void ViewBuildingInfo(GameObject build)
    {
        _productBuildPanel.SetActive(true);
        BuildingControl bc = build.GetComponent<BuildingControl>();
        _txtNameBuild.text = bc.NameBuilding;
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
