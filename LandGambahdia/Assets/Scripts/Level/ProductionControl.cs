using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionControl : MonoBehaviour
{
    [SerializeField] private GameObject[] _products;
    [SerializeField] private int _maxWorkersCount = 1;
    [SerializeField] private float _oneResourceCompleteTime = 1f;
    [SerializeField] private int[] _outResourses;
    [SerializeField] private int[] _inpResourses;

    private int _workerCount = 1;
    private int _producedCount = 0;
    private int _secondCount = 0;

    public string Workers { get { return $"{_workerCount}/{_maxWorkersCount}"; } }
    public string OneResourceCompleteTime { get { return $"производят 1 шт. за {CalcCompleteTime()} сек"; } }

    public int Vacancy { get { return _maxWorkersCount - _workerCount; } }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _products.Length; i++) _products[i].SetActive(false);
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
    public bool AddWorker()
    {
        if (_workerCount < _maxWorkersCount)
        {
            _workerCount++;
            return true;
        }
        return false;
    }

    public bool RemoveWorker()
    {
        if (_workerCount > 0)
        {
            _workerCount--;
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
