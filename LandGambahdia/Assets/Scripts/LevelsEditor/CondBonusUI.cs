using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CondBonusUI : MonoBehaviour
{
    [SerializeField] private Button _saveBtn;
    [SerializeField] private Button[] _condDelBtns;
    [SerializeField] private Text[] _condTexts;
    [SerializeField] private Button[] _condPrevNext;
    [SerializeField] private Dropdown _condDropdown;
    [SerializeField] private InputField _condCountInp;
    [SerializeField] private InputField _condNameInp;

    [SerializeField] private Button[] _bonusDelBtns;
    [SerializeField] private Text[] _bonusTexts;
    [SerializeField] private Button[] _bonusPrevNext;
    [SerializeField] private Dropdown _bonusDropdown;
    [SerializeField] private InputField _bonusCountInp;
    [SerializeField] private InputField _bonusNameInp;

    private LevelShema _levelShema = null;
    private List<VictoryCondition> _conditions = new List<VictoryCondition>();
    private List<VictoryBonus> _bonuses = new List<VictoryBonus>();

    private int _firstCondIndex = 0;
    private int _firstBonusIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _saveBtn.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelShema(LevelShema ls)
    {
        _levelShema = ls;
        ViewSettings();
    }

    private void ViewSettings()
    {
        if (_levelShema != null)
        {
            _conditions = _levelShema.GetConditions();
            _bonuses = _levelShema.GetBonuses();

            ViewConditions();
            ViewBonuses();
        }
        else gameObject.SetActive(false);
    }

    private void ViewConditions()
    {
        int i;
        for (i = 0; i < _condTexts.Length; i++)
        {
            if (_firstCondIndex + i < _conditions.Count)
            {
                _condTexts[i].text = $"{1 + _firstCondIndex + i}. {_conditions[i + _firstCondIndex].ToString()}";
                _condTexts[i].gameObject.SetActive(true);
                _condDelBtns[i].gameObject.SetActive(true);
            }
            else
            {
                _condTexts[i].gameObject.SetActive(false);
                _condDelBtns[i].gameObject.SetActive(false);
            }
        }
        for (i = 0; i < _condPrevNext.Length; i++) _condPrevNext[i].gameObject.SetActive(_conditions.Count > _condTexts.Length);
    }

    private void ViewBonuses()
    {
        int i;
        for (i = 0; i < _bonusTexts.Length; i++)
        {
            if (_firstBonusIndex + i < _bonuses.Count)
            {
                _bonusTexts[i].text = $"{1 + _firstBonusIndex + i}. {_bonuses[i + _firstBonusIndex].ToString()}";
                _bonusTexts[i].gameObject.SetActive(true);
                _bonusDelBtns[i].gameObject.SetActive(true);
            }
            else
            {
                _bonusTexts[i].gameObject.SetActive(false);
                _bonusDelBtns[i].gameObject.SetActive(false);
            }
        }
        for (i = 0; i < _bonusPrevNext.Length; i++) _bonusPrevNext[i].gameObject.SetActive(_bonuses.Count > _bonusTexts.Length);
    }

    public void SaveChanges()
    {
        if (_levelShema != null)
        {
            _levelShema.SetConditions(_conditions);
            _levelShema.SetBonuses(_bonuses);
        }
        gameObject.SetActive(false);
    }

    public void OnClickPrevNextCondition(int value)
    {
        if ((value > 0) && (_firstCondIndex < _conditions.Count - _condTexts.Length))
        {
            _firstCondIndex++;
        }
        if ((value < 0) && (_firstCondIndex > 0))
        {
            _firstCondIndex--;
        }
        ViewConditions();
    }

    public void OnClickPrevNextBonus(int value)
    {
        if ((value > 0) && (_firstBonusIndex < _bonuses.Count - _bonusTexts.Length))
        {
            _firstBonusIndex++;
        }
        if ((value < 0) && (_firstBonusIndex > 0))
        {
            _firstBonusIndex--;
        }
        ViewBonuses();
    }
}
