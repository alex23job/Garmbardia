using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUIControl : MonoBehaviour
{
    [SerializeField] private Image[] imgFones;
    [SerializeField] private GameObject selectLevelPanel;
    [SerializeField] private GameObject setingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewSelectLevelPanel()
    {
        selectLevelPanel.SetActive(true);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void LoadEditor()
    {
        SceneManager.LoadScene("EditorScene");
    }
}
