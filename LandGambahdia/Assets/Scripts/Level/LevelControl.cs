using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private LevelUI _levelUI;
    [SerializeField] private LevelBoard _levelBoard;

    private LevelShema _levelShema;

    // Start is called before the first frame update
    void Start()
    {
        _levelShema = LevelList.Instance.GetShemaLevel(GameManager.Instance.currentPlayer.currentLevel);
        if (_levelShema != null)
        {
            if (_levelUI != null) _levelUI.ViewLevelInfo(_levelShema);
            if (_levelBoard != null) _levelBoard.ViewCurrentLevel(_levelShema);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
