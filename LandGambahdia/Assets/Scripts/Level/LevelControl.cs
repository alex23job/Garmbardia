using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private LevelUI _levelUI;
    [SerializeField] private LevelBoard _levelBoard;
    [SerializeField] private LevelCamera _levelCamera;

    private LevelShema _levelShema;
    private LevelControl _levelControl;

    private List<GameObject> _buildingList = new List<GameObject>();

    private void Awake()
    {
        _levelControl = GetComponent<LevelControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _levelShema = LevelList.Instance.GetShemaLevel(GameManager.Instance.currentPlayer.currentLevel);
        if (_levelShema != null)
        {
            if (_levelUI != null) _levelUI.ViewLevelInfo(_levelShema);
            if (_levelBoard != null) _levelBoard.ViewCurrentLevel(_levelShema, _levelControl);
        }

        // Подписываемся на события
        _levelUI.OnSelectBuilding += OnSelectBuilding;
    }

    private void OnDisable()
    {
        //  Отписываемся от событий
        _levelUI.OnSelectBuilding -= OnSelectBuilding;
    }

    private void OnSelectBuilding(int type, int num)
    {
        GameObject build = _levelBoard.CreateBuilding(type, num);
        if (build != null) _buildingList.Add(build); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TailSelect(GameObject tail)
    {
        if (_levelCamera != null)
        {
            _levelCamera.SetSelectTailPos(tail.transform.position);
        }
    }

    public void TranslateBuildingInfo(BuildingInfo[] arr)
    {
        _levelUI.SetBuildingInfo(arr);
    }
}
