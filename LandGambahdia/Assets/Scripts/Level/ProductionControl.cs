using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProductionControl : MonoBehaviour, IWorkerResourse
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

    public int CountOutResourse { get { return _outResourses.Length; } }
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
                CheckWorkersPath();
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

    private void CheckWorkersPath()
    {
        foreach (GameObject worker in _workers)
        {
            WorkerMovement wm = worker.GetComponent<WorkerMovement>();
            if (wm != null)
            {
                if (wm.ResourseID == -1)
                {
                    CitizenMovement cm = worker.GetComponent<CitizenMovement>();
                    if (cm != null) cm.SelectPathForWorker(gameObject);
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

    public int GetInputResourseID()
    {
        int i, j;
        bool isUsedRes = false;
        for (i = 0; i < _inpResourses.Length; i++)
        {
            if (_inpResourses[i] > 5)
            {
                isUsedRes = false;
                for (j = 0; j < _workers.Count; j++)
                {
                    WorkerMovement wm = _workers[j].GetComponent<WorkerMovement>();
                    if (wm.ResourseID == _inpResourses[i])
                    {
                        isUsedRes = true;
                        break;
                    }
                }
                if (isUsedRes == false) return _inpResourses[i];
            }
        }
        return -1;
    }

    public int GetOutputResourseID()
    {
        int i, j;
        bool isUsedRes = false;
        for (i = 0; i < _outResourses.Length; i++)
        {
            isUsedRes = false;
            for (j = 0; j < _workers.Count; j++)
            {
                WorkerMovement wm = _workers[j].GetComponent<WorkerMovement>();
                if (wm.ResourseID == _outResourses[i])
                {
                    isUsedRes = true;
                    break;
                }
            }
            if (isUsedRes == false) return _outResourses[i];
        }
        //  уже есть рабочие для всех ресурсов => дополнительные рабочие для вывоза
        int numOutRes = _workers.Count - GetCountInpResourses();
        if (numOutRes > 0 && _outResourses.Length > 0)
        {
            numOutRes %= _outResourses.Length;
            return _outResourses[numOutRes];
        }
        return -1;
    }

    private int GetCountInpResourses()
    {
        int i, res = 0;
        for (i = 0; i < _inpResourses.Length; i++) if (_inpResourses[i] > 5) res++;
        return res;
    }

    public bool CheckInputResourseByID(int id)
    {
        return _inpResourses.Contains(id);
    }

    public bool CheckOutputResourseByID(int id)
    {
        return _outResourses.Contains(id);
    }
}
