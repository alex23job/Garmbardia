using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TechnologyRepository : MonoBehaviour
{
    private List<Technology> _technologies = new List<Technology>(); // Репозиторий всех технологий

    public List<Technology> Technologies => _technologies;

    private void Start()
    {
        _technologies.Clear();
        _technologies.Add(new Technology("Камнедобыча", 1, new string[] { "Каменоломня" }, new string[] { "Шахтное дело" }));
        _technologies.Add(new Technology("Рыболовство", 1, new string[] { "Домик рыбака" }, new string[] { "Садоводство" }));
        _technologies.Add(new Technology("Лесоповал", 1, new string[] { "Лесопилка" }, new string[] { "Деревообработка" }));
        _technologies.Add(new Technology("Шахтное дело", 5, new string[] { "Шахта" }, new string[] { "Обработка металла" }));
        _technologies.Add(new Technology("Садоводство", 3, new string[] { "Сад" }, new string[] { "Овощеводство" }));
        _technologies.Add(new Technology("Деревообработка", 5, new string[] { "Дом плотника" }, new string[] { "Мебельное дело" }));
        _technologies.Add(new Technology("Обработка металла", 10, new string[] { "Кузница" }, new string[] { "Инструменты" }));
        _technologies.Add(new Technology("Овощеводство", 5, new string[] { "Ферма" }, new string[] { "Зерноводство" }));

        _technologies[0].AddSciencePoints(1);
    }
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

    public bool CheckTechnologyByTitle(string title)
    {
        foreach (Technology techno in _technologies)
        {
            if (techno.Title == title) return techno.IsResearched;
        }
        return false;
    }

    public bool CheckContainsEntity(string entity)
    {
        foreach(Technology techno in _technologies)
        {
            if (techno.CheckContainsEntity(entity)) return true;
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
    private string _title;              // Название технологии
    private int _sciencePointsCost;     // Стоимость открытия (очки науки)
    private string[] _unlocksEntity;    // Массив зданий и ресурсов, которые разблокирует данная технология
    private string[] _nextTechnologies;   // Последующие технологии, которые становятся доступными после открытия
    private int _investedSciencePoints; // Сколько очков науки уже вложено
    private bool _isResearched;         // Открыта ли технология

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

    public bool CheckContainsEntity(string entity)
    {
        return _unlocksEntity.Contains(entity);
    }
}
