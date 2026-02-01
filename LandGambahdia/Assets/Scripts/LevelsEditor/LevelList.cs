using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//[Serializable]
//public class NoteSet : MonoBehaviour
//{
//    private List<NoteItem> notes = new List<NoteItem>();

//    public static NoteSet Instance;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            Invoke("LoadNotes", 0.001f);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void LoadNotes()
//    {
//        if (File.Exists("NoteDataDubl.txt"))
//        {
//            string csv = File.ReadAllText("NoteDataDubl.txt", System.Text.Encoding.UTF8);
//            CreateNotes(csv);
//            //BinaryFormatter bf = new BinaryFormatter();
//            ////Debug.Log(Application.persistentDataPath + "/MySaveData.dat");
//            //FileStream file = File.Open("NoteData.txt", FileMode.Open);
//            //MyNoteData data = (MyNoteData)bf.Deserialize(file);
//            //file.Close();
//            //Debug.Log(data.ToString());
//            //CreateNotes(data.notesCsv);

//            Debug.Log($"Загружено {notes.Count} текстов записок");
//        }
//        else
//        {
//            Debug.Log("There is no save data! File NoteData.txt not found.");
//        }
//    }

//    private void CreateNotes(string csv, char sep = '#')
//    {
//        notes.Clear();
//        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
//        foreach (string item in ar)
//        {
//            if (item == "") continue;
//            NoteItem note = new NoteItem(item, '=');
//            if (note.IDS_NOTE != "") notes.Add(note);
//        }
//    }

//    public string GetNote(string ids, string lang)
//    {
//        foreach (NoteItem item in notes)
//        {
//            if (item.IDS_NOTE == ids)
//            {
//                string note = item.GetNote(lang);
//                if (note != "") return note;
//                else break;
//            }
//        }
//        return ids;
//    }
//}

[Serializable]
public class MyNoteData
{
    public string notesCsv = "";
}

[Serializable]
public class LevelList : MonoBehaviour
{
    public static LevelList Instance;

    public static List<LevelShema> levels = new List<LevelShema>();

    public static int CurrentLevel = 0;
    public static int MaxLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Invoke("LoadLevels", 0.001f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLevels()
    {
        if (File.Exists("Levels.txt"))
        {
            string csv = File.ReadAllText("Levels.txt", System.Text.Encoding.UTF8);
            CreateLevelsArr(csv);
            //BinaryFormatter bf = new BinaryFormatter();
            ////Debug.Log(Application.persistentDataPath + "/MySaveData.dat");
            //FileStream file = File.Open("NoteData.txt", FileMode.Open);
            //MyNoteData data = (MyNoteData)bf.Deserialize(file);
            //file.Close();
            //Debug.Log(data.ToString());
            //CreateNotes(data.notesCsv);

            Debug.Log($"Загружено уровней : {levels.Count}");
        }
        else
        {
            Debug.Log($"There is no save data! File Levels.txt not found. Число уровней : {levels.Count}");
        }
    }

    private void CreateLevelsArr(string csv, char sep = '^')
    {
        levels.Clear();
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in ar)
        {
            if (item == "") continue;
            LevelShema level = new LevelShema(item, '#', '=');
            if (level.IDS_LEVEL != "") levels.Add(level);
        }
    }

    public void SaveLevels()
    {
        using (StreamWriter writer = new StreamWriter("Levels.txt"))
        {
            writer.WriteLine(ToCsvString('^'), System.Text.Encoding.UTF8);
        }
    }
    private string ToCsvString(char sep = '^')
    {
        StringBuilder sb = new StringBuilder();
        foreach (LevelShema item in levels)
        {
            sb.Append($"{item.ToCsvString()}{sep}");
        }
        return sb.ToString();
    }

    public LevelShema CreateNewShema()
    {
        LevelShema level = new LevelShema();
        levels.Add(level);
        print("Создан пустой новый уровень");
        return level;
    }

    public LevelShema GetShemaLevel(int num)
    {
        foreach (LevelShema level in levels)
        {
            if (level.NumberLevel == num) return level;
        }
        return null;
    }

    public List<int> GetLevelsNumbers()
    {
        List<int> numbers = new List<int>();
        foreach (LevelShema level in levels)
        {
            numbers.Add(level.NumberLevel);
        }
        return numbers;
    }

    public List<int> GetLevelsNumbersAndDelLevel(int numLevel)
    {
        List<int> numbers = new List<int>();
        for (int i = levels.Count; i > 0; i--)
        {
            if (levels[i - 1].NumberLevel == numLevel)
            {
                levels.RemoveAt(i - 1);
                SaveLevels();
                break;
            }
        }
        foreach (LevelShema level in levels)
        {
            numbers.Add(level.NumberLevel);
        }
        return numbers;
    }
}
