using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDoorUI : MonoBehaviour
{
    [SerializeField] private Button _btn1;
    [SerializeField] private Button _btn2;
    [SerializeField] private Button _btn3;

    public void ViewPanel(int mode)
    {
        if (mode == 1)
        {
            _btn1.gameObject.SetActive(true);
            _btn2.gameObject.SetActive(false);
            _btn3.gameObject.SetActive(false);
        }
        if (mode == 2)
        {
            _btn1.gameObject.SetActive(false);
            _btn2.gameObject.SetActive(true);
            _btn3.gameObject.SetActive(false);
        }
        if (mode == 4)
        {
            _btn1.gameObject.SetActive(false);
            _btn2.gameObject.SetActive(false);
            _btn3.gameObject.SetActive(true);
        }
        if (mode == 6)
        {
            _btn1.gameObject.SetActive(false);
            _btn2.gameObject.SetActive(true);
            _btn3.gameObject.SetActive(true);
        }
    }
}
