using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionSciencePoints : MonoBehaviour
{
    [SerializeField] private int _countPointsInMonth = 1;

    public int CountPointsInMonth => _countPointsInMonth;
}
