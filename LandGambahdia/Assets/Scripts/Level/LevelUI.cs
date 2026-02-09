using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
