using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseRequirement : MonoBehaviour
{
    [SerializeField] private int _maxCitizens = 0;
    [SerializeField] private int _oneCitizenNalog = -1;

    // идентификаторы потребностей для повышения уровня жилых домов
    // каждый уровень имеет 2-4 потребности
    public static int[] _totalRequirments = new int[]
        { /*0,*/ 1, 2, 10, 11, 12, 20, 21, 22, 23, 30, 31, 32, 33, 34, 
            40, 41, 42, 43, 50, 51, 52, 53, 60, 61, 62, 63, 70, 71, 72, 73, 80, 81, 82, 83,
            90, 91, 92, 93, 100, 101, 102, 103, 110, 111, 112, 113, 120, 121, 122, 123, 130, 131, 132, 133, 140, 141, 142, 143, 150, 151, 152, 153 };
    private List<int> _requirments = new List<int>();

    private int _houseLevel = 0;
    private int _citizens = 0;
    public int HouseLevel {  get => _houseLevel; }
    public int Citizens { get => _citizens; } 
    public int MaxCitizens { get => _maxCitizens; }
    public int FreePlaces { get { return _maxCitizens - _citizens; } }
    public int Nalog { get { return _citizens * _oneCitizenNalog; } }

    public List<GameObject> _houseCitizens = new List<GameObject>();

    private LevelControl _levelControl = null;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelControl(LevelControl levelControl)
    {
        _levelControl = levelControl;
    }

    public bool CheckLevelRequirments()
    {
        bool isYes = false;
        int i, j, maxRequirmentsNumber = (_houseLevel + 1) * 10;
        for (i = 0; i < _totalRequirments.Length; i++)
        {
            if (_totalRequirments[i] < maxRequirmentsNumber)
            {
                isYes = false;
                for (j = 0; j < _requirments.Count; j++)
                {
                    if (_requirments[j] == _totalRequirments[i])
                    {
                        isYes = true;
                        break;
                    }
                }
                if (isYes == false) return false;
            }
        }
        return true;
    }

    public void AddRequirement(int req, int row, int col, float radius)
    {
        if (_requirments.Contains(req)) return;
        else
        {
            BuildingControl bc = gameObject.GetComponent<BuildingControl>();
            if (bc != null)
            {
                int myRow = (bc.BuildingInfo >> 8) & 0xff;
                int myCol = bc.BuildingInfo & 0xff;
                if ((Mathf.Abs(myRow - row) <= radius) && (Mathf.Abs(myCol - col) <= radius))
                {
                    _requirments.Add(req);
                }
                print($"AddRequirement : radius={radius} <myRow={myRow} row={row}>  <myCol={myCol} col={col}   countReq={_requirments.Count}>");
            }
        }
    }

    public int[] GetRequirements()
    {
        return _requirments.ToArray();
    }

    public void CopyLevelAndRequirements(int level, int[] reqs)
    {
        _houseLevel = level;
        foreach (int req in reqs) _requirments.Add(req);
    }

    public int AddCitizen(int count)
    {
        if (count + _citizens > _maxCitizens)
        {
            _citizens = _maxCitizens;
            return count + _citizens - _maxCitizens;
        }
        _citizens += count;
        return 0;
    }

    public List<GameObject> GetAllCitizens()
    {
        return _houseCitizens;
    }

    public void CopyAllCitizen(List<GameObject> allCitizens)
    {
        _houseCitizens = allCitizens;
    }

    public void PushCitizen(GameObject citizen)
    {
        _houseCitizens.Add(citizen);
        if (_levelControl != null) _levelControl.ChangeCitizen(gameObject, citizen);
    }

    public GameObject PopCitizen()
    {
        if (_houseCitizens.Count > 0)
        {
            GameObject citizen = _houseCitizens[0];
            _houseCitizens.RemoveAt(0);
            return citizen;
        }
        return null;
    }
}
