using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TechnologyRepository : MonoBehaviour
{
    private List<Technology> _technologies = new List<Technology>(); // –епозиторий всех технологий

    public Technology FindTechnologyByTitle(string title)
    {
        foreach (Technology techno in _technologies)
        {
            if (techno.Title == title) return techno;
        }
        return null;
    }

    public bool CheckEntity(string entity)
    {
        foreach (Technology techno in _technologies)
        {
            if (techno.CheckUnlockEntity(entity)) return true;
        }
        return false;
    }

    //public void ResearchTechnology(Technology tech)
    //{
    //    tech.IsResearched = true;
    //}

    //public void InvestSciencePoints(Technology tech, int points)
    //{
    //    tech.InvestedSciencePoints += points;
    //    if (tech.InvestedSciencePoints >= tech.SciencePointsCost)
    //    {
    //        ResearchTechnology(tech);
    //    }
    //}
}

[Serializable]
public class Technology
{
    private string _title;              // Ќазвание технологии
    private int _sciencePointsCost;     // —тоимость открыти€ (очки науки)
    private string[] _unlocksEntity;    // ћассив зданий и ресурсов, которые разблокирует данна€ технологи€
    private string[] _nextTechnologies;   // ѕоследующие технологии, которые станов€тс€ доступными после открыти€
    private int _investedSciencePoints; // —колько очков науки уже вложено
    private bool _isResearched;         // ќткрыта ли технологи€

    public string Title { get { return _title; } }                      
    public int SciencePointsCost { get { return _sciencePointsCost; } }             
    public string[] UnlocksEntity { get { return _unlocksEntity; } }
    public string[] NextTechnologies { get { return _nextTechnologies; } }         
    public int InvestedSciencePoints { get { return _investedSciencePoints; } }         
    public bool IsResearched { get { return _isResearched; } }   
    public int DeltaPoints { get { return _sciencePointsCost - _investedSciencePoints; } }
    
    public Technology() { }
    public Technology(string nm, int cost, string[] entity, string[] nextTechno)
    {
        _title = nm;
        _sciencePointsCost = cost;
        _unlocksEntity = entity;
        _nextTechnologies = nextTechno;
        _investedSciencePoints = 0;
        _isResearched = false;
    }

    public void AddSciencePoints(int count)
    {
        _investedSciencePoints += count;
        if (_sciencePointsCost == _investedSciencePoints) _isResearched = true;
    }

    public bool CheckUnlockEntity(string entity)
    {
        return _isResearched && _unlocksEntity.Contains(entity);
    }
}
