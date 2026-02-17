using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    private int[] _buildsID = null;
    private Vector3 _spawnPos = Vector3.zero;
    private int _spawnPosZn = -1;

    public Vector3 SpawnPosition { get => _spawnPos; }

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
                        int tailInfo = landTail.TailInfo;
                        int num = (tailInfo >> 16) & 0xff;   //  это и есть tailID
                        int x = tailInfo & 0xff;
                        int y = (tailInfo >> 8) & 0xff;
                        if (CheckBuilding(num, buildID, y / 4, x / 4) == false)
                        {
                            _levelControl.ViewError("Тип местности не соответствует виду постройки !");
                            return null;
                        }
                        Vector3 pos = _selectTail.transform.position;
                        pos.y = 1.5f;

                        _levelControl.ChangeMany(-bc.Price);
                        GameObject b = Instantiate(prefab, pos, Quaternion.identity);
                        if (_levelShema.BoardSize == 35) b.transform.localScale = new Vector3(2f, 2f, 2f);
                        BuildingControl nbc = b.GetComponent<BuildingControl>();
                        nbc.SetBoardAndPosition(_levelBoard, y, x);
                        ConturRadius cr = b.GetComponent<ConturRadius>();
                        if (cr != null) cr.SetSize(nbc.Radius);
                        print($"CreateBuilding  y={y} x={x}  buildID={nbc.BuildingID}   y/4={y/4} x/4={x/4} index={_levelShema.BoardSize * (y / 4) + (x / 4)}");
                        if (_levelShema.BoardSize == 35) _buildsID[_levelShema.BoardSize * (y / 4) + (x / 4)] = nbc.BuildingID;
                        if (_levelShema.BoardSize == 70) _buildsID[_levelShema.BoardSize * (y / 2) + (x / 2)] = nbc.BuildingID;
                        _selectTail = null;
                        _ceil.SetActive(false);
                        return b;
                    }
                }
            }
        }
        return null;
    }

    public GameObject UpdateHouse(GameObject house)
    {
        BuildingControl obc = house.GetComponent<BuildingControl>();
        HouseRequirement houseRequirement = house.GetComponent<HouseRequirement>();
        int buildID = houseRequirement.HouseLevel + 1;
        foreach(GameObject prefab in _buildingPrefabs)
        {
            BuildingControl bc = prefab.GetComponent<BuildingControl>();
            if (bc != null)
            {
                if (bc.BuildingID == buildID)
                {   //  возможно нужно скорректировать buildID в массиве _buildsID
                    Vector3 pos = house.transform.position;
                    GameObject b = Instantiate(prefab, pos, Quaternion.identity);
                    b.transform.localScale = house.transform.localScale;
                    BuildingControl nbc = b.GetComponent<BuildingControl>();
                    nbc.SetBoardAndPosition(_levelBoard, (obc.BuildingInfo >> 8) & 0xff, obc.BuildingInfo & 0xff);
                    HouseRequirement nhr = b.GetComponent<HouseRequirement>();
                    nhr.CopyLevelAndRequirements(houseRequirement.HouseLevel + 1, houseRequirement.GetRequirements());
                    nhr.AddCitizen(houseRequirement.Citizens);
                    nhr.CopyAllCitizen(houseRequirement.GetAllCitizens());
                    return b;
                }
            }
        }
        return null;
    }

    private bool CheckBuilding(int landID, int buildingID, int row, int col)
    {
        if (landID == 24 || landID == 27)
        {   //  проверка на начало/конец моста
            if (buildingID == 131 || buildingID == 133) return true;
            else return false;
        }
        if (landID == 20 || landID == 23)
        {   //  проверка на постройку шахты
        }
        if (landID == 2)
        {   //  проверка на средину моста
            if (buildingID == 132 || buildingID == 134) return true;
            else return false;
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
        _buildsID = new int[_tailsID.Length];
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
                    if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, 4 * y, 4 * x);  //  * 4
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
                        _tailsID[_levelShema.BoardSize * (y / 2) + x / 2] = num;  //  возможно нужно y/2 и x/2
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
                    if (landTail != null) landTail.SetBoardAndPosition(_levelBoard, 2 * y, 2 * x);  //  * 2
                    _tails.Add(tail);
                }
            }
        }
        CalkStartSpawnPosition();
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

    private void CalkStartSpawnPosition()
    {
        int i, col, row, maxSz = _levelShema.BoardSize, minCol = 100, minRow = 100, maxCol = -1, maxRow = 0, center = maxSz / 2, zn = -1;
        for(i = 0; i < _tailsID.Length; i++)
        {
            if (_tailsID[i] == 96)
            {
                col = i % maxSz;
                row = i / maxSz;
                if (col > center && col > maxCol) { maxCol = col; zn = i; }
                if (col < center && col < minCol) { minCol = col; zn = i; }
                if (row > center && row > maxRow) { maxRow = row; zn = i; }
                if (row < center && row < minRow) { minRow = row; zn = i; }
            }
        }
        int start = -5;
        if (minRow == 0) start = zn;
        if (minCol == 0) start = zn;
        if (maxCol == maxSz - 1) start = zn;
        if (maxRow == maxSz - 1) start = zn;
        if (start != -5)
        {
            col = start % maxSz;
            row = start / maxSz;
            _spawnPos.y = 2f;
            if (maxSz == 35)
            {
                _spawnPos.x = _ofsX + col * 2f + 1f;
                _spawnPos.z = _ofsY - row * 2f - 1f;
            }
            if (maxSz == 70)
            {
                _spawnPos.x = _ofsX + col + 0.5f;
                _spawnPos.z = _ofsY - row - 0.5f;
            }
            _spawnPosZn = start;
        }
    }

    public List<Vector3> GetCurPath(Vector3 target)
    {
        List<Vector3> path = new List<Vector3>();
        WavePath wavePath = new WavePath();
        int[] pole = new int[_tailsID.Length];
        for (int i = 0; i < _tailsID.Length; i++)
        {
            pole[i] = -1;
            if (_tailsID[i] == 96) pole[i] = 0;
            if (_buildsID[i] >= 128 && _buildsID[i] < 135) pole[i] = 0;
            if (_buildsID[i] != 0) print($"Fill pole   _buildsID[{i}] = {_buildsID[i]}");
        }
        float dop = (_levelShema.BoardSize == 35) ? 1f : 0.5f;
        int div = (_levelShema.BoardSize == 35) ? 2 : 1;
        //int tgRow = Mathf.RoundToInt(_ofsY - target.z - dop) / (2 * div);
        //int tgCol = Mathf.RoundToInt(target.x - _ofsX - dop) / (2 * div);
        int tgRow = Mathf.RoundToInt(_ofsY - target.z - dop) / div;
        int tgCol = Mathf.RoundToInt(target.x - _ofsX - dop) / div;
        pole[tgRow * _levelShema.BoardSize + tgCol] = 0;
        List<int> pathZn = wavePath.GetPath(_spawnPosZn, new int[] { tgRow * _levelShema.BoardSize + tgCol }, pole, _levelShema.BoardSize);
        string strPathZn = (pathZn != null) ? pathZn.Count.ToString() : "NULL !!!";
        print($"GetCurPath(Vector3) target=><{target}>   tgRow={tgRow}  tgCol={tgCol}  index={tgRow * _levelShema.BoardSize + tgCol}   pathZn=<{pathZn}>   lenPath={strPathZn}");
        int row, col;
        if (pathZn != null && pathZn.Count > 0)
        {
            foreach (int zn in pathZn)
            {
                row = zn / _levelShema.BoardSize;
                col = zn % _levelShema.BoardSize;
                path.Add(new Vector3(_ofsX + 2 * col + dop, 1.5f, _ofsY - 2 * row - dop));
                //print($"zn={zn}   point={path[path.Count - 1]}");
            }
        }
        return path;
    }
    public List<Vector3> GetCurPath(int targetIndex)
    {
        List<Vector3> path = new List<Vector3>();
        WavePath wavePath = new WavePath();
        int[] pole = new int[_tailsID.Length];
        for (int i = 0; i < _tailsID.Length; i++)
        {
            pole[i] = -1;
            if (_tailsID[i] == 96) pole[i] = 0;
            if (_buildsID[i] >= 128 && _buildsID[i] < 135) pole[i] = 0;
            //if (_buildsID[i] != 0) print($"Fill pole   _buildsID[{i}] = {_buildsID[i]}");
        }
        float dop = (_levelShema.BoardSize == 35) ? 1f : 0.5f;
        int div = (_levelShema.BoardSize == 35) ? 2 : 1;
        pole[targetIndex] = 0;
        List<int> pathZn = wavePath.GetPath(_spawnPosZn, new int[] { targetIndex }, pole, _levelShema.BoardSize);
        string strPathZn = (pathZn != null) ? pathZn.Count.ToString() : "NULL !!!";
        print($"GetCurPath(int)  index={targetIndex}   pathZn=<{pathZn}>   lenPath={strPathZn}");
        int row, col;
        if (pathZn != null && pathZn.Count > 0)
        {
            foreach (int zn in pathZn)
            {
                row = zn / _levelShema.BoardSize;
                col = zn % _levelShema.BoardSize;
                path.Add(new Vector3(_ofsX + 2 * col + dop, 1.5f, _ofsY - 2 * row - dop));
                //print($"zn={zn}   point={path[path.Count - 1]}");
            }
        }
        return path;
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
