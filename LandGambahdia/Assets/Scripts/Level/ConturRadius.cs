using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConturRadius : MonoBehaviour
{
    [SerializeField] private GameObject _contur;

    private float _mult = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ViewContur(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSize(float mult)
    {
        _mult = mult;
        _contur.transform.localScale = _contur.transform.localScale * mult;
    }

    public void ViewContur(bool value)
    {
        _contur.SetActive(value);
    }
}
