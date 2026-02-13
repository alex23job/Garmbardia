using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionControl : MonoBehaviour
{
    [SerializeField] private GameObject[] _products;
    [SerializeField] private int _maxWorkersCount = 1;
    [SerializeField] private float _oneResourceCompleteTime = 1f;

    private int _workerCount = 1;
    private int _producedCount = 0;
    private int _secondCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _products.Length; i++) _products[i].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
