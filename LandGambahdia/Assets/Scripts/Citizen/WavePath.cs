using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WavePath
{
    private int[] arQu;
    private int[] endPath;
    private int startNum = -1, endNum = -1;
    private int sizeRow, maxZn = 10000;
    private List<int> path = new List<int>();
    private int countRoads = 0;

    public List<int> GetPath(int start, int[] end, int[] pole, int szRow)
    {
        path.Clear();
        startNum = start;
        endPath = end;
        endNum = -1;
        sizeRow = szRow;
        arQu = new int[pole.Length];
        for (int i = 0; i < pole.Length; i++)
        {
            if (pole[i] == 0)
            {
                arQu[i] = maxZn;
                countRoads++;
            }
            else arQu[i] = -1;
        }
        arQu[startNum] = 0;countRoads++;
        if (FindPath()) return path;
        return null;
    }

    private bool FindPath()
    {
        arQu[startNum] = 0;
        CreateWave();
        if (GetEndPath() != -1)
        {
            int step = arQu[endNum], i = endNum, x, y;
            path.Add(endNum);
            while (step > 0)
            {
                x = i % sizeRow; y = i / sizeRow;
                if ((x > 0) && (arQu[i - 1] != -1) && (arQu[i - 1] < arQu[i])) { step = arQu[i - 1]; path.Add(i - 1); i--; continue; }
                if ((x < sizeRow - 1) && (arQu[i + 1] != -1) && (arQu[i + 1] < arQu[i])) { step = arQu[i + 1]; path.Add(i + 1); i++; continue; }
                if ((y > 0) && (arQu[i - sizeRow] != -1) && (arQu[i - sizeRow] < arQu[i])) { step = arQu[i - sizeRow]; path.Add(i - sizeRow); i -= sizeRow; continue; }
                if ((y < sizeRow - 1) && (arQu[i + sizeRow] != -1) && (arQu[i + sizeRow] < arQu[i])) { step = arQu[i + sizeRow]; path.Add(i + sizeRow); i += sizeRow; continue; }
            }
            path.Reverse();

            /*StringBuilder sb = new StringBuilder("Path -> ");
            for (i = 0; i < path.Count; i++)
            {
                sb.Append($"{path[i]} ");
            }
            Debug.Log(sb.ToString());*/
            return true;
        }
        return false;
    }

    private void CreateWave()
    {
        int step = 1, i, x, y, countWave;
        /*StringBuilder sb2 = new StringBuilder("Start ");
        for (i = 0; i < arQu.Length; i++) sb2.Append($"{arQu[i]} ");
        Debug.Log(sb2.ToString());*/
        while (step < countRoads)
        {
            for (i = 0; i < arQu.Length; i++)
            {
                if ((arQu[i] == -1) || (arQu[i] >= step)) continue;
                x = i % sizeRow;y = i / sizeRow;
                if ((x > 0) && (arQu[i - 1] != -1) && (arQu[i - 1] == maxZn)) arQu[i - 1] = step;
                if ((x < sizeRow - 1) && (arQu[i + 1] != -1) && (arQu[i + 1] == maxZn)) arQu[i + 1] = step;
                if ((y > 0) && (arQu[i - sizeRow] != -1) && (arQu[i - sizeRow] == maxZn)) arQu[i - sizeRow] = step;
                if ((y < sizeRow - 1) && (arQu[i + sizeRow] != -1) && (arQu[i + sizeRow] == maxZn)) arQu[i + sizeRow] = step;
            }
            step++;
            for (i = 0, countWave = 0; i < arQu.Length; i++)
            {
                if ((arQu[i] != -1) && (arQu[i] != maxZn)) countWave++;
            }
            /*StringBuilder sb1 = new StringBuilder($"step={step}   ");
            for (i = 0; i < arQu.Length; i++) sb1.Append($"{arQu[i]} ");
            Debug.Log(sb1.ToString());*/

            if (countWave >= countRoads) break;
        }
        /*StringBuilder sb = new StringBuilder();
        //for (i = 0; i < arQu.Length; i++) sb.Append($"{arQu[i]}{((i % 20 == 19) ? "\n" : " ")}");
        for (i = 0; i < arQu.Length; i++) sb.Append($"{arQu[i]} ");
        Debug.Log(sb.ToString());*/
    }

    private int GetEndPath()
    {
        int minZn = 10000;
        for (int i = 0; i < endPath.Length; i++)
        {
            if (arQu[endPath[i]] != maxZn)
            {
                if (endNum == -1) { minZn = arQu[endPath[i]]; endNum = endPath[i]; }
                else
                {
                    if (minZn > arQu[endPath[i]]) { minZn = arQu[endPath[i]]; endNum = endPath[i]; }
                }
                //Debug.Log($"endNum={endNum} i={i} endPath[i]={endPath[i]} arQu[{endPath[i]}]={arQu[endPath[i]]}");
            }
        }
        return endNum;
    }
}
