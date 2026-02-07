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
    [SerializeField] private Text _condTitle;

    [SerializeField] private Button[] _bonusDelBtns;
    [SerializeField] private Text[] _bonusTexts;
    [SerializeField] private Button[] _bonusPrevNext;
    [SerializeField] private Dropdown _bonusDropdown;
    [SerializeField] private InputField _bonusCountInp;
    [SerializeField] private InputField _bonusNameInp;
    [SerializeField] private Text _bonusTitle;

    private LevelShema _levelShema = null;
    private List<VictoryCondition> _conditions = new List<VictoryCondition>();
    private List<VictoryBonus> _bonuses = new List<VictoryBonus>();

    private int _firstCondIndex = 0;
    private int _firstBonusIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _saveBtn.interactable = false;
        List<string> condOptions = new List<string>() { "Процветание", "Население", "Деньги", "Время", "Здание", "Технология" };
        List<string> bonusOptions = new List<string>() { "Опыт", "Очки науки", "Деньги", "Здание", "Технология" };
        _condDropdown.options.Clear();
        _condDropdown.AddOptions(condOptions);
        _condDropdown.value = 0;
        _bonusDropdown.options.Clear();
        _bonusDropdown.AddOptions(bonusOptions);
        _bonusDropdown.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelShema(LevelShema ls)
    {
        _levelShema = ls;
        ClearInputFields();
        ViewSettings();
    }

    private void ClearInputFields()
    {
        _firstBonusIndex = 0;
        _firstCondIndex = 0;
        _condDropdown.value = 0;
        _bonusDropdown.value = 0;
        _condCountInp.text = "";
        _condNameInp.text = "";
        _bonusCountInp.text = "";
        _bonusNameInp.text = "";
        _saveBtn.interactable = false;
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

    private void ViewCountCondition()
    {
        _condTitle.text = $"Условия победы - {_conditions.Count}";
    }

    private void ViewCountBonuses()
    {
        _bonusTitle.text = $"Награды - {_bonuses.Count}";
    }

    private void ViewConditions()
    {
        ViewCountCondition();
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
        ViewCountBonuses();
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

    public void DelCondition(int value)
    {
        if (value + _firstCondIndex < _conditions.Count)
        {
            _conditions.RemoveAt(value + _firstCondIndex);
        }
        if ((_firstCondIndex > 0) && (_firstCondIndex > _conditions.Count - _condTexts.Length))
        {
            _firstCondIndex--;
        }
        ViewConditions();
        _saveBtn.interactable = true;
    }

    public void DelBonus(int value)
    {
        if (value + _firstBonusIndex < _bonuses.Count)
        {
            _bonuses.RemoveAt(value + _firstBonusIndex);
        }
        if ((_firstBonusIndex > 0) && (_firstBonusIndex > _bonuses.Count - _bonusTexts.Length))
        {
            _firstBonusIndex--;
        }
        ViewBonuses();
        _saveBtn.interactable = true;
    }

    public void AddCondition()
    {
        string category = _condDropdown.options[_condDropdown.value].text;
        string name = _condNameInp.text;
        if (int.TryParse(_condCountInp.text, out int count))
        {
            _conditions.Add(new VictoryCondition(category, count, name));
            if (_conditions.Count > _condTexts.Length) _firstCondIndex = _conditions.Count - _condTexts.Length;
            ViewConditions();
            _saveBtn.interactable = true;
        }
    }

    public void AddBonus()
    {
        string category = _bonusDropdown.options[_bonusDropdown.value].text;
        string name = _bonusNameInp.text;
        if (int.TryParse(_bonusCountInp.text, out int count))
        {
            _bonuses.Add(new VictoryBonus(category, count, name));
            if (_bonuses.Count > _bonusTexts.Length) _firstBonusIndex = _bonuses.Count - _bonusTexts.Length;
            ViewBonuses();
            _saveBtn.interactable = true;
        }
    }
}
