using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBoard : MonoBehaviour
{
    [SerializeField] private GameObject _ceilPrefab;
    [SerializeField] private float _ofsX;
    [SerializeField] private float _ofsY;
    [SerializeField] private int _sizeBoard;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateGrid()
    {
        int i, j;
        Vector3 pos = new Vector3(0, 0.2f, 0);
        for (i = 0; i < _sizeBoard; i++)
        {
            pos.z = _ofsY - i - 0.5f;
            for (j = 0; j < _sizeBoard; j++)
            {
                pos.x = _ofsX + j + 0.5f;
                GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);
                ceil.transform.parent = transform;
            }
        }
        //for (i = 0; i < _sizeBoard; i += 7)
        //{
        //    pos.z = _ofsY - i - 3.5f;
        //    for (j = 0; j < _sizeBoard; j += 7)
        //    {
        //        pos.x = _ofsX + j + 3.5f;
        //        GameObject ceil = Instantiate(_ceilPrefab, pos, Quaternion.identity);                
        //        ceil.transform.parent = transform;
        //        ceil.transform.localScale = new Vector3(9.7f, 1f, 9.7f);
        //    }
        //}
    }
}
