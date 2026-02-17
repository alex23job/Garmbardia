using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSitizen : MonoBehaviour
{
    [SerializeField] private float _speedCitizen = 5;
    [SerializeField] private GameObject _prefabCitizen;
    [SerializeField] private int _monthPeriod = 300;
    [SerializeField] private LevelControl _levelControl;

    private int _countSpawnCitizens = 0;
    private int _countSecond = 0;
    private int _interval = 0;
    private Vector3 _startPos = Vector3.zero;

    public Vector3 StartPosition { get => _startPos; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelParams(LevelControl lc, Vector3 pos)
    {
        _levelControl = lc;
        _startPos = pos;
        print($"StartPos = {_startPos}");
    }

    /// <summary>
    /// Генерация нового подселенца в случае превышения интервала времени их появления
    /// проверка идет раз в секунду
    /// </summary>
    /// <returns>Новый поселенец или null</returns>
    public GameObject SpawnCitizen()
    {
        _countSecond++;
        if (_interval > 0 && _countSecond > _interval)
        {
            _countSecond = 0;
            List<Vector3> path = new List<Vector3>();
            if (_levelControl != null && _levelControl.GetPathToFreePlase(out path))
            {
                print("Новый житель приехал !");
                GameObject citizen = Instantiate(_prefabCitizen, _startPos, Quaternion.identity);
                CitizenMovement citizenMovement = citizen.GetComponent<CitizenMovement>();
                if (citizenMovement != null)
                {
                    citizenMovement.SetParams(_levelControl, _startPos, path, true, 5f, 5f);
                }
                return citizen;
            }            
        }
        return null;
    }

    public void CalcInterval(int prosperity, int vacancy, int freePlaces)
    {
        float coef = ((float)prosperity / 100f) * Mathf.Sqrt(freePlaces) * Mathf.Log10(1 + vacancy);
        print($"coef={coef}   prosperity={prosperity}({(float)prosperity / 100f})   freePlaces={freePlaces}({Mathf.Sqrt(freePlaces)})  vacancy={vacancy}({Mathf.Log10(1 + vacancy)})");
        if (freePlaces > 0)
        {
            if (coef < 1) _interval = Mathf.RoundToInt((float)_monthPeriod / (float)freePlaces);
            else _interval = Mathf.RoundToInt((float)_monthPeriod / coef);
        }
        else
        {
            _interval = 0;
        }
    }
}
