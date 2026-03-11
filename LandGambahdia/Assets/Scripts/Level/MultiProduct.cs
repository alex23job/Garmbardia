using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiProduct : MonoBehaviour
{
    [SerializeField] private int _maxWorkersCount = 1;
    [SerializeField] private int[] _outResourses;
    [SerializeField] private int[] _inpResourses;

    private ProductResourseRepository _productRepository;
    private RequirmentsManager _requirmentsManager;

    private int _workerCount = 0;
    private List<SimpleResourse> _arrResourses = new List<SimpleResourse>();
    private int _indexPosInBoard = -1;

    public string Workers { get { return $"{_workerCount}/{_maxWorkersCount}"; } }
    public int WorkersCount { get { return _workerCount; } }
    public int IndexPosInBoard { get { return _indexPosInBoard; } }
    public int Vacancy { get { return _maxWorkersCount - _workerCount; } }

    public SimpleResourse[] OutResourses { get { return GetOutResourses(); } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FillArrResourse()
    {
        if ((_outResourses != null) && (_outResourses.Length > 0) && (_productRepository != null) && (_requirmentsManager != null)) 
        {
            for (int i = 0; i < _outResourses.Length; i++)
            {
                ProductResourse pr = _productRepository.GetResourseByID(_outResourses[i]);
                if (pr.ID != -1)
                {
                    Requirments req = _requirmentsManager.GetRequirmentsByName(pr.Name);
                    _arrResourses.Add(new SimpleResourse(pr.ID, req.Id, pr.Icon, pr.Name, 0));
                }
            }
        }
    }

    public void SetParams(ProductResourseRepository prr, RequirmentsManager rm, int indexPos)
    {
        _requirmentsManager = rm;
        _productRepository = prr;
        _indexPosInBoard = indexPos;
        FillArrResourse();
    }

    private SimpleResourse[] GetOutResourses()
    {
        SimpleResourse[] res = new SimpleResourse[_arrResourses.Count];
        for (int i = 0; i < _arrResourses.Count; i++)
        {
            SimpleResourse item = _arrResourses[i];
            res[i] = new SimpleResourse(item.ResourseID, item.Requirement, item.Icon, item.NameResourse, item.Count);
        }
        return res;
    }

    public bool CheckRequirementByID(int idReq)
    {
        foreach(SimpleResourse item in _arrResourses)
        {
            if (item.Requirement == idReq && item.Count > 0) return true;
        }
        return false;
    }

    public int[] GetCheckRequirements()
    {
        List<int> res = new List<int>();
        foreach(SimpleResourse item in _arrResourses)
        {
            if (item.Count > 0) res.Add(item.Requirement);
        }
        return res.ToArray();
    }

    public void AddingResourseByID(int idRes, int count = 1)
    {
        for (int i = 0; i < _arrResourses.Count; i++)
        {
            if (_arrResourses[i].ResourseID == idRes) _arrResourses[i].AddResourse(count);
        }
    }

    public int TakeResourseByID(int idRes, int count = 1)
    {
        for (int i = 0; i < _arrResourses.Count; i++)
        {
            if (_arrResourses[i].ResourseID == idRes) return _arrResourses[i].GetResourse(count);
        }
        return 0;
    }
}

public struct SimpleResourse
{
    public int ResourseID;
    public int Count;
    public int Requirement;
    public Sprite Icon;
    public string NameResourse;

    public SimpleResourse(int id, int req, Sprite icon, string nm, int cnt = 0)
    {
        ResourseID = id;
        Count = cnt;
        Requirement = req;
        Icon = icon;
        NameResourse = nm;
    }

    public void AddResourse(int count = 1)
    {
        Count += count;
    }

    public int GetResourse(int count = 1)
    {
        if (Count >= count)
        {
            Count -= count;
            return count;
        }
        return 0;
    }
}
