using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirmentsManager : MonoBehaviour
{
    [SerializeField] private List<Requirments> _requirments = new List<Requirments>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Requirments GetRequirmentsByID(int id)
    {
        foreach (Requirments requirments in _requirments)
        {
            if (requirments.Id == id) return requirments;
        }
        return new Requirments(-1, "", "", null, "");
    }

    public Requirments GetRequirmentsByName(string nm)
    {
        foreach (Requirments requirments in _requirments)
        {
            if (requirments.Name == nm) return requirments;
        }
        return new Requirments(-1, "", "", null, "");
    }
}

[Serializable]
public struct Requirments
{
    [SerializeField] public int Id;
    [SerializeField] public string Name;
    [SerializeField] public string Description;
    [SerializeField] public Sprite Icon; // Поле, доступное в инспекторе
    [SerializeField] public string RequiredTechnology;
    //    public Production Facility Producer;

    public Requirments(int id, string nm, string des, Sprite spr = null, string tech="") 
    {
        Id = id;
        Name = nm;
        Description = des;
        Icon = spr;
        RequiredTechnology = tech;
    }
}

