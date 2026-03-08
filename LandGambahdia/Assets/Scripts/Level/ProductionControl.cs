using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionControl : MonoBehaviour
{
    [SerializeField] private GameObject[] _products;
    [SerializeField] private GameObject[] _dopProducts;
    [SerializeField] private int _maxWorkersCount = 1;
    [SerializeField] private float _oneResourceCompleteTime = 1f;
    [SerializeField] private int[] _outResourses;
    [SerializeField] private int[] _inpResourses;

    private int _workerCount = 0;
    private int _producedCount = 0;
    private int _secondCount = 0;
    private float _maxDeltaDop = 0.2f;
    private float _deltaDopY = 0f;
    private Vector3[] _startDopProducts = null;

    private List<GameObject> _workers = new List<GameObject>();

    public string Workers { get { return $"{_workerCount}/{_maxWorkersCount}"; } }
    public string OneResourceCompleteTime { get { return $"производят 1 шт. за {CalcCompleteTime()} сек"; } }

    public int Vacancy { get { return _maxWorkersCount - _workerCount; } }

    // Start is called before the first frame update
    void Start()
    {
        int i;
        for (i = 0; i < _products.Length; i++) _products[i].SetActive(false);
        if (_dopProducts != null && _dopProducts.Length > 0)
        {
            _startDopProducts = new Vector3[_dopProducts.Length];
            for (i = 0; i < _dopProducts.Length; i++)
            {
                _startDopProducts[i] = _dopProducts[i].transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string CalcCompleteTime()
    {
        if (_workerCount == 0) return "-";
        else return $"{(_oneResourceCompleteTime / _workerCount):F2}";
    }
    public bool AddWorker(GameObject worker)
    {
        if (_workerCount < _maxWorkersCount)
        {
            _workerCount++;
            _workers.Add(worker);
            return true;
        }
        return false;
    }

    public bool RemoveWorker()
    {
        if (_workerCount > 0)
        {
            _workerCount--;
            _workers.RemoveAt(0);
            return true;
        }
        return false;
    }

    public void AddSecond()
    {
        if (_workerCount > 0)
        {
            _secondCount++;
            if (_secondCount > _oneResourceCompleteTime / _workerCount)
            {
                if (_producedCount < _products.Length)
                {
                    _products[_producedCount].gameObject.SetActive(true);
                    _producedCount++;
                    _secondCount = 0;
                }
            }
            if (_dopProducts != null && _dopProducts.Length > 0)
            {
                if (_deltaDopY < _maxDeltaDop) _deltaDopY += 0.001f; else _deltaDopY = 0;
                for (int i = 0; i < _dopProducts.Length; i++)
                {
                    Vector3 pos = _startDopProducts[i];
                    pos.y += _deltaDopY;
                    _dopProducts[i].transform.position = pos;
                }
            }
        }
    }

    public bool TakeResourse()
    {
        if (_producedCount > 0)
        {
            _producedCount--;
            _products[_producedCount].gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public int[] GetInpResoursesID()
    {
        int[] res = new int[_inpResourses.Length];
        for (int i = 0; i < _inpResourses.Length; i++) res[i] = _inpResourses[i];
        return res;
    }

    public int[] GetOutResoursesID()
    {
        int[] res = new int[_outResourses.Length];
        for (int i = 0; i < _outResourses.Length; i++) res[i] = _outResourses[i];
        return res;
    }
}
