using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EditorBoard : MonoBehaviour
{
    [SerializeField] private GameObject _ceilPrefab;
    [SerializeField] private float _ofsX;
    [SerializeField] private float _ofsY;
    [SerializeField] private int _sizeBoard;
    [SerializeField] private EditorUI _editorUI;

    [SerializeField] private GameObject[] _landTailPrefabs;

    private LandTail[] _landTails = null;
    private LevelShema _levelShema = null;
    private Vector3 _levelTailScale = new Vector3(1f, 1f, 1f);
    private float _sizeMult = 1.0f;
    private EditorBoard _board = null;

    private GameObject _currentLandTail = null;
    private GameObject _selectCeil = null;
    private List<GameObject> _tails = new List<GameObject>();

    private void Awake()
    {
        _board = GetComponent<EditorBoard>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // Подписываемся на события
        _editorUI.OnLevelChanged += ViewCurrentLevel;
        _editorUI.OnSelectLandTail += SelectLandTail;
        _editorUI.OnSelectSpecTail += SelectSpecTail;

        //CreateGrid();
        FillLandTails();
        Invoke("SetPrizrak", 0.01f);
    }

    private void SetPrizrak()
    {
        _editorUI.SetPrizrak(transform.GetChild(0).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        _editorUI.OnLevelChanged -= ViewCurrentLevel;
        _editorUI.OnSelectLandTail -= SelectLandTail;
        _editorUI.OnSelectSpecTail -= SelectSpecTail;
    }

    private void FillLandTails()
    {
        _landTails = new LandTail[_landTailPrefabs.Length];
        for (int i = 0; i < _landTailPrefabs.Length; i++) 
        {
            _landTails[i] = _landTailPrefabs[i].GetComponent<LandTail>();
        }
    }

    private void SelectSpecTail(int num)
    {
        int i, x, y;
        float mult = 1f;
        Vector3 pos = new Vector3(0, 0.5f, 0);
        if (_levelShema.BoardSize == 35) mult = 2f;
        for (i = 0; i < _landTails.Length; i++)
        {
            if (_landTails[i].CmpID(num))
            {
                if (_selectCeil != null)
                {
                    x = Mathf.RoundToInt((_selectCeil.transform.position.x - _ofsX - mult * 0.5f) * mult);
                    y = Mathf.RoundToInt((_ofsY - _selectCeil.transform.position.z - mult * 0.5f) * mult);
                    GameObject tail = Instantiate(_landTailPrefabs[i]);
                    pos.x = _selectCeil.transform.position.x;
                    pos.z = _selectCeil.transform.position.z;
                    tail.transform.position = pos;
                    tail.transform.parent = transform;
                    print($"SelectSpecTail  pos={pos}    parentPos={tail.transform.position}   ceilPos={_selectCeil.transform.position}");
                    if (_levelShema.BoardSize == 35) tail.transform.localScale = _levelTailScale;
                    LandTail landTail = tail.GetComponent<LandTail>();
                    if (landTail != null) landTail.SetBoardAndPosition(_board, y, x);
                    _tails.Add(tail);
                }
                break;
            }
        }
    }

    private void SelectLandTail(int type, int num)
    {
        //throw new NotImplementedException();
    }

    private void ViewCurrentLevel(LevelShema level)
    {
        if (level != null)
        {
            _levelShema = level;
            if (_levelShema != null)
            {
                if (_levelShema.BoardSize == 35) _levelTailScale = new Vector3(2.75f, 100f, 2.75f);
                if (_levelShema.BoardSize == 70) _levelTailScale = new Vector3(1f, 1f, 1f);
            }
            CreateGrid();
        }
    }

    private void CreateGrid()
    {
        int i, j;
        Vector3 pos = new Vector3(0, 0.2f, 0);
        if (_levelShema.BoardSize == 70)
        {   //  70 x 70
            for (i = 0; i < _sizeBoard; i++)
            {
                pos.z = _ofsY - i - 0.5f;
                for (j = 0; j < _sizeBoard; j++)
                {
                    pos.x = _ofsX + j + 0.5f;
                    GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);
                    ceil.transform.parent = transform;
                    ceil.GetComponent<CeilControl>().SetBoard(_board);
                }
            }
        }
        else
        {   //  35 x 35
            _sizeMult = 2.0f;
            for (i = 0; i < _sizeBoard; i += 2)
            {
                pos.z = _ofsY - i - 1f;
                for (j = 0; j < _sizeBoard; j += 2)
                {
                    pos.x = _ofsX + j + 1f;
                    GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);
                    ceil.transform.parent = transform;
                    ceil.transform.localScale = _levelTailScale;
                    ceil.GetComponent<CeilControl>().SetBoard(_board);
                }
            }
        }
        //for (i = 0; i < _sizeBoard; i += 7)
        //{
        //    pos.z = _ofsY - i - 3.5f;
        //    for (j = 0; j < _sizeBoard; j += 7)
        //    {
        //        pos.x = _ofsX + j + 3.5f;
        //        GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);                
        //        ceil.transform.parent = transform;
        //        ceil.transform.localScale = new Vector3(9.7f, 1f, 9.7f);
        //    }
        //}
        //_sizeBoard = 50;
        //for (i = 0; i < _sizeBoard; i++)
        //{
        //    pos.z = _ofsY - 1.4f * i - 0.7f;
        //    for (j = 0; j < _sizeBoard; j++)
        //    {
        //        pos.x = _ofsX + 1.4f * j + 0.7f;
        //        GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);
        //        ceil.transform.parent = transform;
        //        ceil.transform.localScale = new Vector3(1.9f, 1f, 1.9f);
        //    }
        //}
    }

    //public void CreateLevel()
    //{

    //}

    public void TailSelect(GameObject tail)
    {

    }

    public void CeilSelect(GameObject ceil)
    {
        _selectCeil = ceil;
        print($"Ceil position={_selectCeil.transform.position}");
    }
}
