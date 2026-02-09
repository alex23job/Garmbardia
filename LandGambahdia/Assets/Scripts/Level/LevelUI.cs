using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Text _levelInfo;


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

    public void OnSaveLevelClick()
    {

    }

    public void OnExitClick()
    {
        _pausePanel.SetActive(false);
        SceneManager.LoadScene("MainScene");
    }
}
