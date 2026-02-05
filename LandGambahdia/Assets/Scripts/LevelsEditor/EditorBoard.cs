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
        // ѕодписываемс€ на событи€
        _editorUI.OnLevelChanged += ViewCurrentLevel;
        _editorUI.OnSelectLandTail += SelectLandTail;
        _editorUI.OnSelectSpecTail += SelectSpecTail;
        _editorUI.OnDelOrRotTail += DelOrRotTail;

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
        _editorUI.OnDelOrRotTail -= DelOrRotTail;
    }

    private void FillLandTails()
    {
        _landTails = new LandTail[_landTailPrefabs.Length];
        for (int i = 0; i < _landTailPrefabs.Length; i++) 
        {
            _landTails[i] = _landTailPrefabs[i].GetComponent<LandTail>();
        }
    }

    private void DelOrRotTail(int num)
    {
        if (_currentLandTail != null)
        {
            if (num == 3)
            {   //  окно закрыли
                _currentLandTail = null;
            }            
            if (num == 2)
            {   //  окно обновл€ем с новым углом поворота
                LandTail landTail = _currentLandTail.GetComponent<LandTail>();
                landTail.RotateTail();
                if (landTail != null && _editorUI != null)
                {
                    _editorUI.ViewDelOrRotPanel(landTail.GetTailInfo(), landTail.IsRotate);
                    _levelShema.UpdateTerainTails(landTail.TailInfo);
                }
            }
            if (num == 1)
            {   //  удалили часть и окно закрыто
                LandTail landTail = _currentLandTail.GetComponent<LandTail>();
                _levelShema.RemoveTerainTail(landTail.TailInfo);
                _tails.Remove(_currentLandTail);
                Destroy(_currentLandTail, 0.01f);
                _currentLandTail = null;
                _editorUI.InterUndo(_tails.Count > 0);
            }
            print($"DelOrRotTail   num={num}    curTail=< {_currentLandTail} >");
        }
    }

    private void BuildTail(int num, int type = 9)
    {
        int i, x, y;
        float mult = 1f;
        Vector3 pos = new Vector3(0, 0.5f, 0);
        if (_levelShema.BoardSize == 35) mult = 2f;
        for (i = 0; i < _landTails.Length; i++)
        {
            if (_landTails[i].CmpID(num, type))
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
                    //print($"SelectSpecTail  pos={pos}    parentPos={tail.transform.position}   ceilPos={_selectCeil.transform.position}");
                    if (_levelShema.BoardSize == 35) tail.transform.localScale = _levelTailScale;
                    LandTail landTail = tail.GetComponent<LandTail>();
                    if (landTail != null) landTail.SetBoardAndPosition(_board, y, x);
                    AddTail(tail, y, x);                    
                }
                break;
            }
        }
    }

    private void AddTail(GameObject tail, int y, int x)
    {
        bool isNew = true;
        LandTail landTail = tail.GetComponent<LandTail>();
        foreach(GameObject go in _tails)
        {
            if (go.GetComponent<LandTail>().CmpPosition(y, x))
            {
                isNew = false;
                break;
            }
        }
        if (isNew)
        {
            _tails.Add(tail);
            _editorUI.InterUndo(true);
            _levelShema.UpdateTerainTails(landTail.TailInfo);
            _selectCeil = null;
        }
        else
        {   // в этой клетке уже что-то есть

        }
    }

    private void SelectSpecTail(int num)
    {
        BuildTail(num);
    }

    /// <summary>
    /// ‘ункци€ определени€ префаба местности по выбранным из панели постройки параметрам 
    /// </summary>
    /// <param name="type">1 - трава/гора, 2 - трава/вода, 3 - песок/трава, 4 - песок/вода, 5 - песок/гора</param>
    /// <param name="num">0 - весь 1й, 1 - 50/50, 2 - угол 1й, 3 - угол 2й, 4 - весь 2й </param>
    private void SelectLandTail(int type, int num)
    {
        int l1 = -1, l2 = -1, landID = -1;
        switch(type)
        {
            case 1:
                l1 = 0; l2 = 1;
                break;
            case 2:
                l1 = 0; l2 = 2;
                break;
            case 3:
                l1 = 3; l2 = 0;
                break;
            case 4:
                l1 = 3; l2 = 2;
                break;
            case 5:
                l1 = 3; l2 = 1;
                break;
        }
        switch(num)
        {
            case 0:
                landID = l1; 
                break;
            case 1:
                landID = l1 + (l2 << 2) + (1 << 4);
                break;
            case 2:
                landID = l1 + (l2 << 2) + (2 << 4);
                break;
            case 3:
                landID = l2 + (l1 << 2) + (2 << 4);
                break;
            case 4:
                landID = l2;
                break;
        }
        //print($"l1={l1}  l2={l2}  landID={landID}(0x{landID:X08})");
        BuildTail(landID, 0);
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
            CreateTerrain();
            _editorUI.InterBtnArr(true);
        }
    }

    private void CreateTerrain()
    {
        ClearTails();
        int[] terrain = _levelShema.GetTerrain();
        int i, j, x, y, num, rot;
        Vector3 pos = new Vector3(0, 0.5f, 0);
        if (_levelShema.BoardSize == 35)
        {
            for (i = 0; i < terrain.Length; i++)
            {
                x = terrain[i] & 0xff;
                y = (terrain[i] >> 8) & 0xff;
                num = (terrain[i] >> 16) & 0xff;
                rot = (terrain[i] >> 24) & 0x3;
                print($"0x{terrain[i]:X08}  x={x}  y={y}  num={num}  rot={rot}");
                for (j = 0; j < _landTails.Length; j++)
                {
                    if (_landTails[j].CmpID(num % 90, (num >= 90 ? 9 : 0)))
                    {
                        //pos.x = _ofsX + 2 * x + 1f;
                        //pos.z = _ofsY - 2 * y - 1f;
                        pos.x = _ofsX + x * 0.5f + 1f;
                        pos.z = _ofsY - y * 0.5f - 1f;
                        GameObject tail = Instantiate(_landTailPrefabs[j], pos, Quaternion.identity);
                        tail.transform.parent = transform;
                        tail.transform.localScale = _levelTailScale;
                        LandTail landTail = tail.GetComponent<LandTail>();
                        if (landTail != null) landTail.SetBoardAndPosition(_board, y, x);
                        if (landTail.IsRotate) for (j = 0; j < rot; j++) landTail.RotateTail();
                        _tails.Add(tail);
                        break;
                    }
                }
            }
        }
        if (_levelShema.BoardSize == 70)
        {
            for (i = 0; i < terrain.Length; i++)
            {
                x = terrain[i] & 0xff;
                y = (terrain[i] >> 8) & 0xff;
                num = (terrain[i] >> 16) & 0xff;
                rot = (terrain[i] >> 24) & 0x3;
                for (j = 0; j < _landTails.Length; j++)
                {
                    if (_landTails[j].CmpID(num % 90, (num >= 90 ? 9 : 0)))
                    {
                        pos.x = _ofsX + x + 0.5f;
                        pos.z = _ofsY - y - 0.5f;
                        GameObject tail = Instantiate(_landTailPrefabs[j], pos, Quaternion.identity);
                        tail.transform.parent = transform;
                        //tail.transform.localScale = _levelTailScale;
                        LandTail landTail = tail.GetComponent<LandTail>();
                        if (landTail != null) landTail.SetBoardAndPosition(_board, y, x);
                        if (landTail.IsRotate) for (j = 0; j < rot; j++) landTail.RotateTail();
                        _tails.Add(tail);
                        break;
                    }
                }
            }
        }
        _editorUI.InterUndo(_tails.Count > 0);
    }

    private void ClearTails()
    {
        for (int i = _tails.Count; i > 0; i--)
        {
            Destroy(_tails[i], 0.01f);
        }
        _tails.Clear();
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
        _currentLandTail = tail;
        LandTail landTail = tail.GetComponent<LandTail>();
        if (landTail != null && _editorUI != null)
        {
            _editorUI.ViewDelOrRotPanel(landTail.GetTailInfo(), landTail.IsRotate);
        }
    }

    public void CeilSelect(GameObject ceil)
    {
        _selectCeil = ceil;
        //print($"Ceil position={_selectCeil.transform.position}");
    }
}
