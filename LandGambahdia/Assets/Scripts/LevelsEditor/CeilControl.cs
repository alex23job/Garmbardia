using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilControl : MonoBehaviour
{
    private EditorBoard _board = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetBoard(EditorBoard eb)
    {
        _board = eb;
    }

    private void OnMouseUp()
    {
        Invoke("TranslateToBoard", 0.05f);
    }

    private void TranslateToBoard()
    {
        if (_board != null) _board.CeilSelect(gameObject);
    }
}
