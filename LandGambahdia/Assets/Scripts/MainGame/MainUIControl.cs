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
    [SerializeField] private Button[] btnSity;
 
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSityName()
    {
        List<LevelShemaInfo> levels = LevelList.Instance.GetLevelInfos();
        for (int i = 0; i < levels.Count; i++)
        {
            if (i < btnSity.Length)
            {
                Text txtSity = btnSity[i].gameObject.GetComponentInChildren<Text>();
                txtSity.text = $"{levels[i].LevelNumber}. {levels[i].LevelName}";
            }
        }
    }

    public void ViewSelectLevelPanel()
    {
        SetSityName();
        selectLevelPanel.SetActive(true);
    }

    public void LoadLevel(int num)
    {
        GameManager.Instance.currentPlayer.currentLevel = num;
        SceneManager.LoadScene("LevelScene");
    }

    public void LoadEditor()
    {
        SceneManager.LoadScene("EditorScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ViewRecord()
    {

    }
}
