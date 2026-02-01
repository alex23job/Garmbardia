using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EditorUI : MonoBehaviour
{
    [SerializeField] private Button[] tailButtons;
    [SerializeField] private CameraControl _cameraControl;

    private Color _baseColor = new Color(0.7f, 1f, 0.9f, 1f), _selectColor = new Color(1f, 0.9f, 0.7f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        SelectTail(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SelectTail(int num)
    {
        int i;
        for (i = 0; i < tailButtons.Length; i++)
        {
            tailButtons[i].gameObject.GetComponent<Image>().color = (i != num) ? _baseColor : _selectColor; 
        }
        _cameraControl.SetQuadrant(num);
    }

    public void SelectSize(bool value)
    {
        if (value) _cameraControl.ChangeSize(0.5f);
        else _cameraControl.ChangeSize(-0.5f);
    }
}
