using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductResourseRepository : MonoBehaviour
{
    [SerializeField] private ProductResourse[] _productResourses;

    public int Count { get {  return _productResourses.Length; } }

    public ProductResourse GetResourseByID(int id)
    {
        foreach (var item in _productResourses)
        {
            if (item.ID == id) return item;
        }
        return new ProductResourse(-1);
    }
}

[Serializable]
public struct ProductResourse
{
    public int ID;
    public string Name;
    public Sprite Icon;

    public ProductResourse(int id, string nm = "", Sprite sprite = null) 
    {
        ID = id;
        Name = nm;
        Icon = sprite;
    }
}
