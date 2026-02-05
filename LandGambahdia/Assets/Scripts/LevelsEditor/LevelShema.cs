using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class LevelShema
{
    private string _ids_level = "";
    private int _number;
    private string _name;
    private int _boardSize;
    private List<VictoryCondition> _conditions = new List<VictoryCondition>();
    private List<VictoryBonus> _bonuses = new List<VictoryBonus>();
    private int[] _terrain = null;
    private List<int> _tmpTails = new List<int>();

    public int NumberLevel { get { return _number; } }
    public string Name { get { return _name; } }
    public int BoardSize { get { return _boardSize; } }
    public string IDS_LEVEL { get { return _ids_level; } }

    public int CountConditions {  get { return _conditions.Count; } }
    public int CountBonuse { get { return _bonuses.Count; } }

    public LevelShema() { }
    public LevelShema(string csv, char sep = '#', char sepVL = '=')
    {
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (ar.Length >= 7)
        {
            _ids_level = ar[0];
            if (int.TryParse(ar[1], out int num))
            {
                _number = num;
            }
            _name = ar[2];
            if (int.TryParse(ar[3], out int sz))
            {
                _boardSize = sz;
            }
            string[] arCond = ar[4].Split(sepVL);
            foreach (string cond in arCond)
            {
                _conditions.Add(new VictoryCondition(cond));
            }
            string[] arBonus = ar[5].Split(sepVL);
            foreach (string bonus in arBonus)
            {
                _bonuses.Add(new VictoryBonus(bonus));
            }
            int i, zn;
            string s;
            for (i = 0; i < ar[6].Length; i += 8)
            {
                s = ar[6].Substring(i, 8);
                zn = Convert.ToInt32(s, 16);
                _tmpTails.Add(zn);
            }
            _terrain = _tmpTails.ToArray();
        }
    }

    public void AddCondions(VictoryCondition vc)
    {
        _conditions.Add(vc);
    }

    public void AddBonus(VictoryBonus vb)
    {
        _bonuses.Add(vb);
    }

    public void SetBoardSize(int sz)
    {
        if ((sz == 35) || (sz == 70)) _boardSize = sz;
    }

    public void SetNameLevel(string nm)
    {
        _name = nm;
    }

    public void SetNumber(int num)
    {
        _number = num;
    }

    public int[] GetTerrain()
    {
        int[] res = new int[_terrain.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = _terrain[i];
        }
        return res;
    }

    public void SetTerain(int[] ter)
    {
        _terrain = ter;
    }

    public void UpdateTerainTails(int tailInfo)
    {
        bool isNew = true;
        for (int i = 0; i < _tmpTails.Count; i++) 
        {
            if ((tailInfo & 0xffff) == (_tmpTails[i] & 0xffff))
            {
                _tmpTails[i] = tailInfo;
                isNew = false;
                break;
            }
        }
        if (isNew) _tmpTails.Add(tailInfo);
    }

    public void RemoveTerainTail(int tailInfo)
    {
        _tmpTails.Remove(tailInfo);
    }

    public string ToCsvString(char sep = '#', char sepVL = '=')
    {
        StringBuilder sb = new StringBuilder($"Level{sep}{_number}{sep}{_name}{sep}{_boardSize}{sep}");
        foreach(VictoryCondition vc in _conditions) { sb.Append(vc.ToCsvString(';') + sepVL); }
        sb.Append(sep);
        foreach(VictoryBonus vb in _bonuses) { sb.Append(vb.ToCsvString(';') + sepVL); }
        sb.Append(sep);
        foreach(int zn in _tmpTails) { if ((zn & 0xff0000) > 0) sb.Append($"{zn:X08}"); }
        //if (_terrain != null) foreach (int zn in _terrain) { sb.Append($"{zn:X08}"); }
        sb.Append(sep);
        return sb.ToString();
    }
}

[Serializable]
public class VictoryCondition
{
    private int _count;
    private string _name;
    public int Count { get { return _count; } }
    public string NameCondition { get { return _name; } }

    public VictoryCondition() { }
    public VictoryCondition(int n, string nm)
    {
        _count = n;
        _name = nm;
    }

    public VictoryCondition(string csv, char sep = ';')
    {
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (ar.Length >= 2)
        {
            _name = ar[0];
            if (int.TryParse(ar[1], out int zn))
            {
                _count = zn;
            }
        }
    }

    public string ToCsvString(char sep = ';')
    {
        return $"{_name}{sep}{_count}{sep}";
    }
}

[Serializable]
public class VictoryBonus
{
    private string _title;
    private int _count;
    private string _nameBonus;

    public string NameBonus { get { return _nameBonus; } }
    public int Count { get { return _count; } }
    public string Title {  get { return _title; } }
    public VictoryBonus() { }
    public VictoryBonus(string t, int n, string nm)
    {
        _title = t;
        _count = n;
        _nameBonus = nm;
    }
    public VictoryBonus(string csv, char sep = ';')
    {
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (ar.Length >= 3)
        {
            _title = ar[0];
            if (int.TryParse(ar[1], out int zn))
            {
                _count = zn;
            }
            _nameBonus += ar[2];
        }
    }

    public string ToCsvString(char sep = ';')
    {
        return $"{_title}{sep}{_count}{sep}{_nameBonus}{sep}";
    }
}
