using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTailViewing : MonoBehaviour
{
    [SerializeField] private Image _fillQw;
    [SerializeField] private Image _halfQw;
    [SerializeField] private Image _angleQw;
    [SerializeField] private Image _rectSelect;
    [SerializeField] private Sprite[] _specLandTails;
    [SerializeField] private Sprite _spriteFillQw;

    /// <summary>
    /// ÷вета местностей : 0 - трава, 1 - гора, 2 - вода, 3 - песок
    /// </summary>
    private Color[] _landColors = new Color[4] { new Color(0.1f, 0.8f, 0.1f, 1f), new Color(0.6f, 0.6f, 0.6f, 1f), new Color(0.3f, 0.3f, 0.9f, 1f), new Color(1f, 0.7f, 0.3f, 1f) };

    private bool _isSelect = false;
    private bool _isSelectRect = false;
    private float _timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StopSelect();   
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSelect)
        {
            if (_timer > 0f) _timer -= Time.deltaTime;
            else
            {
                _timer = 0.25f;
                _isSelectRect = !_isSelectRect;
                _rectSelect.gameObject.SetActive(_isSelectRect);
            }
        }
    }

    public void SetLandTail(int num)
    {
        _halfQw.gameObject.SetActive(false);
        _angleQw.gameObject.SetActive(false);
        _isSelect = true;
        if (num >= 90)
        {
            _fillQw.color = Color.white;
            _fillQw.sprite = _specLandTails[num % 90];
        }
        else
        {
            _fillQw.sprite = _spriteFillQw;
            int type = (num >> 4) & 0x3;
            int l1 = num & 0x3;
            int l2 = (num >> 2) & 0x3;
            switch (type)
            {
                case 0:
                    _fillQw.color = _landColors[l1];
                    break;
                case 1:
                    _fillQw.color = _landColors[l1];
                    _halfQw.color = _landColors[l2];
                    _halfQw.gameObject.SetActive(true);
                    break;
                case 2:
                    _fillQw.color = _landColors[l1];
                    _angleQw.color = _landColors[l2];
                    _angleQw.gameObject.SetActive(true);
                    break;
            }
        }
        _fillQw.gameObject.SetActive(true);
    }

    public void StopSelect()
    {
        _isSelect = false;
        _rectSelect.gameObject.SetActive(false);
        _fillQw.gameObject.SetActive(false);
        _halfQw.gameObject.SetActive(false);
        _angleQw.gameObject.SetActive(false);
    }
}
