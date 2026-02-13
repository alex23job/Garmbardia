using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBoard : MonoBehaviour
{
    [SerializeField] private GameObject _ceil;
    [SerializeField] private GameObject[] _landTailPrefabs;
    [SerializeField] private float _ofsX;
    [SerializeField] private float _ofsY;
    [SerializeField] private int _sizeBoard;
    [SerializeField] private GameObject[] _buildingPrefabs;

    private LandTail[] _landTails = null;
    private LevelShema _levelShema = null;
    private Vector3 _levelTailScale = new Vector3(1f, 1f, 1f);
    private List<GameObject> _tails = new List<GameObject>();
    private LevelBoard _levelBoard = null;
    private int[] _tailsID = null;

    private LevelControl _levelControl = null;

    private GameObject _selectTail = null;

    private void Awake()
    {
        _levelBoard = GetComponent<LevelBoard>();
        FillLandTails();
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("TranslateBuildingInfo", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TranslateBuildingInfo()
    {
        List<BuildingInfo> list = new List<BuildingInfo>();
        foreach (GameObject go in _buildingPrefabs)
        {
            BuildingControl bc = go.GetComponent<BuildingControl>();
            if (bc != null) list.Add(bc.GetBuildingInfo());
        }
        if (_levelControl != null) _levelControl.TranslateBuildingInfo(list.ToArray());
    }

    private void FillLandTails()
    {
        _landTails = new LandTail[_landTailPrefabs.Length];
        for (int i = 0; i < _landTailPrefabs.Length; i++)
        {
            _landTails[i] = _landTailPrefabs[i].GetComponent<LandTail>();
            //print(_landTails[i].GetTailInfo());
        }
    }

    public GameObject CreateBuilding(int cat, int build)
    {
        int buildID = (cat << 5) + build;
        print($"CreateBuilding id={buildID}");
        if (_selectTail != null)
        {
            LandTail landTail = _selectTail.GetComponent<LandTail>();
            if (landTail == null) return null;
            foreach (GameObject prefab in _buildingPrefabs)
            {
                BuildingControl bc = prefab.GetComponent<BuildingControl>();
                if (bc != null)
                {
                    if (bc.BuildingID == buildID)
                    {
                        if (_levelControl.CheckMany(bc.Price) == false)
                        {
                            _levelControl.ViewError("Недостаточно денег для постройки !");
                            return null;
                        }                        
                        int tailID = landTail.TailID;
                        int num = (tailID >> 16) & 0xff;
                        if (CheckBuilding(num, buildID) == false)
                        {
                            _levelControl.ViewError("Тип местности не соответствует виду постройки !");
                            return null;
                        }
                        int x = tailID & 0xff;
                        int y = (tailID >> 8) & 0xff;
                        Vector3 pos = _selectTail.transform.position;
                        pos.y = 1.5f;

                        _levelControl.ChangeMany(-bc.Price);
                        GameObject b = Instantiate(prefab, pos, Quaternion.identity);
                        if (_levelShema.BoardSize == 35) b.transform.localScale = new Vector3(2f, 2f, 2f);
                        BuildingControl nbc = b.GetComponent<BuildingControl>();
                        nbc.SetBoardAndPosition(_levelBoard, y, x);
                        ConturRadius cr = b.GetComponent<ConturRadius>();
                        if (cr != null) cr.SetSize(nbc.Radius);
                        _selectTail = null;
                        _ceil.SetActive(false);
                        return b;
                    }
                }
            }
        }
        return null;
    }

    private bool CheckBuilding(int landID, int buildingID)
    {
        if (landID == 24 || landID == 27)
        {   //  проверка на начало/конец моста
        }
        if (landID == 20 || landID == 23)
        {   //  проверка на постройку шахты
        }
        if (landID == 2)
        {   //  проверка на средину моста
        }
        if (landID == 90)
        {   //  проверка на постройку лесопилки
        }
        if (landID == 3)
        {   //  проверка на постройку стеклодува
        }
        if (landID == 96)
        {   //  это дорога
            if ((buildingID == 129) || (buildingID == 130)) return true;
            else return false;
        }
        if (landID > 0)
        {   //  это не трава
            return false;
        }
        return true;
    }

    public void ViewCurrentLevel(LevelShema level, LevelControl lc)
    {
        _levelControl = lc;
        if (level != null)
        {
            _levelShema = level;
            if (_levelShema != null)
            {
                if (_levelShema.BoardSize == 35) _levelTailScale = new Vector3(2.858f, 200f, 2.858f);
                if (_levelShema.BoardSize == 70) _levelTailScale = new Vector3(1f, 1f, 1f);
                print($"BoardSize={_levelShema.BoardSize}");
            }
            //CreateGrid();
            CreateTerrain();
            //_editorUI.InterBtnArr(true);
        }
    }
    private void CreateTerrain()
    {
        ClearTails();
        int[] terrain = _levelShema.GetTerrain();
        int i, j, x, y, num, rot;
        Vector3 pos = new Vector3(0, 0.5f, 0);
        _tailsID = new int[_levelShema.BoardSize * _levelShema.BoardSize];
        if (_levelShema.BoardSize == 35)
        {
            for (i = 0; i < terrain.Length; i++)
            {
                x = terrain[i] & 0xff;
                y = (terrain[i] >> 8) & 0xff;
                num = (terrain[i] >> 16) & 0xff;
                rot = (terrain[i] >> 24) & 0x3;
                //print($"0x{terrain[i]:X08}  x={x}  y={y}  num={num}  rot={rot}");
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
                        if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, y, x);
                        if (landTail.IsRotate) for (j = 0; j < rot; j++) landTail.RotateTail();
                        _tails.Add(tail);
                        //print($"y={y} x={x}  index={_levelShema.BoardSize * (y / 4) + x / 4}    max={_levelShema.BoardSize * _levelShema.BoardSize}");
                        _tailsID[_levelShema.BoardSize * (y / 4) + x / 4] = num;
                        break;
                    }
                }
            }
            //return;
            for (i = 0; i < _tailsID.Length; i++)
            {
                if (_tailsID[i] == 0)
                {
                    x = i % _levelShema.BoardSize;
                    y = i / _levelShema.BoardSize;
                    pos.x = _ofsX + x * 2f + 1f;
                    pos.z = _ofsY - y * 2f - 1f;
                    GameObject tail = Instantiate(_landTailPrefabs[7], pos, Quaternion.identity);
                    tail.transform.parent = transform;
                    tail.transform.localScale = _levelTailScale;
                    LandTail landTail = tail.GetComponent<LandTail>();
                    if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, y, x);
                    _tails.Add(tail);
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
                        if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, y, x);
                        if (landTail.IsRotate) for (j = 0; j < rot; j++) landTail.RotateTail();
                        _tails.Add(tail);
                        _tailsID[_levelShema.BoardSize * y + x] = num;  //  возможно нужно y/2 и x/2
                        break;
                    }
                }
            }
            for (i = 0; i < _tailsID.Length; i++)
            {
                if (_tailsID[i] == 0)
                {
                    x = i % _levelShema.BoardSize;
                    y = i / _levelShema.BoardSize;
                    pos.x = _ofsX + x + 0.5f;
                    pos.z = _ofsY - y - 0.5f;
                    GameObject tail = Instantiate(_landTailPrefabs[7], pos, Quaternion.identity);
                    tail.transform.parent = transform;
                    LandTail landTail = tail.GetComponent<LandTail>();
                    if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, y, x);
                    _tails.Add(tail);
                }
            }
        }
        //_editorUI.InterUndo(_tails.Count > 0);
    }
    private void ClearTails()
    {
        for (int i = _tails.Count; i > 0; i--)
        {
            Destroy(_tails[i - 1], 0.01f);
        }
        _tails.Clear();
    }

    public void TailSelect(GameObject tail)
    {
        if (_selectTail != null)
        {
            ConturRadius cr = _selectTail.GetComponent<ConturRadius>();
            if (cr != null) cr.ViewContur(false);
            LandTail land = _selectTail.GetComponent<LandTail>();
            if (land != null) _ceil.SetActive(false);
        }

        if (_levelControl  != null)
        {
            ConturRadius cr = tail.GetComponent<ConturRadius>();
            if (cr != null) cr.ViewContur(true);
            _levelControl.TailSelect(tail);
            _selectTail = tail;
            LandTail land = tail.GetComponent<LandTail>();
            if (land != null)
            {
                //int x = land.TailInfo & 0xff;
                //int y = (land.TailInfo >> 8) & 0xff;
                _ceil.transform.position = new Vector3(tail.transform.position.x, 1.5f, tail.transform.position.z);
                _ceil.SetActive(true);
            }
        }
    }

    private Vector3 GetPos(int x, int y, float h = 1.5f)
    {
        Vector3 pos = Vector3.zero;
        pos.y = h;
        if (_levelShema.BoardSize == 35)
        {
            pos.x = _ofsX + x * 0.5f + 1f;
            pos.z = _ofsY - y * 0.5f - 1f;
        }
        if (_levelShema.BoardSize == 70)
        {
            pos.x = _ofsX + x + 0.5f;
            pos.z = _ofsY - y - 0.5f;
        }
        return pos;
    }
}
