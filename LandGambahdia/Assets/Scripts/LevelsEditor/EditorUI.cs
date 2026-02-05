using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EditorUI : MonoBehaviour
{
    // Делегат для уведомления о смене уровня
    public delegate void LevelChangedEventHandler(LevelShema level);
    public event LevelChangedEventHandler OnLevelChanged;
    // Делегат для уведомления о выборе части местности
    public delegate void SelectLandTailEventHandler(int type, int num);
    public event SelectLandTailEventHandler OnSelectLandTail;
    // Делегат для уведомления о выборе специальной части
    public delegate void SelectSpecTailEventHandler(int num);
    public event SelectSpecTailEventHandler OnSelectSpecTail;
    // Делегат для уведомления о выборе поворота или удаления части местности
    public delegate void ActionTailEventHandler(int num);
    public event ActionTailEventHandler OnDelOrRotTail;

    [SerializeField] private Button[] _tailButtons;
    [SerializeField] private CameraControl _cameraControl;

    [SerializeField] private GameObject _createPanel;
    [SerializeField] private InputField _nameLevel;
    [SerializeField] private InputField _numLevel;
    [SerializeField] private InputField[] _conditions;
    [SerializeField] private InputField[] _bonuses;
    [SerializeField] private Toggle[] _togleSZs;

    [SerializeField] private GameObject _specTailPanel;
    [SerializeField] private GameObject _landTailPanel;
    [SerializeField] private Text _landTailText;
    [SerializeField] private Button _landTailBtn1;
    [SerializeField] private Button _landTailBtn5;

    [SerializeField] private GameObject _delRotPanel;
    [SerializeField] private Text _delRotInfoText;
    [SerializeField] private Button _delRotBtn;

    [SerializeField] private GameObject _msgPanel;
    [SerializeField] private Text _msgText;

    [SerializeField] private Text _txtAskDel;
    [SerializeField] private GameObject _askDelPanel;
    [SerializeField] private GameObject _selectLoadLevelPanel;
    [SerializeField] private GameObject[] _items;
    [SerializeField] private Scrollbar _scrollbar;
    private int _curIndexLevels = 0;
    private List<LevelShemaInfo> _levelShemaInfos = null;

    [SerializeField] private Button _undoBtn;
    [SerializeField] private Button[] _landBtns;

    private Color _baseColor = new Color(0.7f, 1f, 0.9f, 1f), _selectColor = new Color(1f, 0.9f, 0.7f, 1f);
    private Color[] _landColor = new Color[4] { new Color(0.1f, 0.8f, 0.1f, 1f), new Color(0.6f, 0.6f, 0.6f, 1f), new Color(0.3f, 0.3f, 0.9f, 1f), new Color(8f, 0.7f, 0.3f, 1f) };

    private LevelShema _curLevel = null;
    private bool _isNew = true;
    private int _landTailsType = 0;

    private GameObject _prizrak = null;

    // Start is called before the first frame update
    void Start()
    {
        InterUndo(false);
        InterBtnArr(false);
        SelectTail(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        // Получить вращение колеса мыши
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAmount < 0) _cameraControl.ChangeSize(0.5f);
        if (scrollAmount > 0) _cameraControl.ChangeSize(-0.5f);
    }

    public void InterUndo(bool value)
    {
        _undoBtn.interactable = value;
    }
    public void InterBtnArr(bool value)
    {
        foreach (var btn in _landBtns)
        {
            btn.interactable = value;
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SetPrizrak(GameObject prz)
    {
        _prizrak = prz;
        _prizrak.SetActive(false);
    }

    public void SelectTail(int num)
    {
        int i;
        for (i = 0; i < _tailButtons.Length; i++)
        {
            _tailButtons[i].gameObject.GetComponent<Image>().color = (i != num) ? _baseColor : _selectColor; 
        }
        _cameraControl.SetQuadrant(num);
    }

    public void SelectSize(bool value)
    {
        if (value) _cameraControl.ChangeSize(0.5f);
        else _cameraControl.ChangeSize(-0.5f);
    }

    public void CreateLevel()
    {
        if (_isNew)
        {
            _curLevel = LevelList.Instance.CreateNewShema();
            _isNew = false;
        }
        if (_nameLevel.text == "")
        {
            ViewInputError("Введите название уровня");
            return;
        }
        else _curLevel.SetNameLevel(_nameLevel.text);
        if (_numLevel.text == "")
        {
            ViewInputError("Введите номер уровня");
            return;
        }
        else
        {
            if (int.TryParse(_numLevel.text, out int num))
            {
                _curLevel.SetNumber(num);
            }
            else
            {
                ViewInputError($"Номер уровня должен быть числом, а не <{_numLevel.text}>");
                return;
            }
        }
        if (_togleSZs[0].isOn) { _curLevel.SetBoardSize(35); }
        if (_togleSZs[1].isOn) { _curLevel.SetBoardSize(70); }
        int count = -1;
        if (_conditions[0].text != "")
        {
            if (int.TryParse(_conditions[0].text, out count)) _curLevel.AddCondions(new VictoryCondition(count, "Процветание"));
            else
            {
                ViewInputError($"Укажите число в диапазоне 100-10000 для процветания вместо <{_conditions[0].text}>");
                return;
            }
        }
        if (_conditions[1].text != "")
        {
            if (int.TryParse(_conditions[1].text, out count)) _curLevel.AddCondions(new VictoryCondition(count, "Население"));
            else
            {
                ViewInputError($"Укажите число в диапазоне 100-10000 для населения вместо <{_conditions[1].text}>");
                return;
            }
        }
        if (_conditions[2].text != "")
        {
            if (int.TryParse(_conditions[2].text, out count)) _curLevel.AddCondions(new VictoryCondition(count, "Монеты"));
            else
            {
                ViewInputError($"Укажите число в диапазоне 100-10000 для монет вместо <{_conditions[2].text}>");
                return;
            }
        }
        if (_conditions[3].text != "")
        {
            if (int.TryParse(_conditions[3].text, out count)) _curLevel.AddCondions(new VictoryCondition(count, "Условие"));
            else
            {
                ViewInputError($"Укажите число в диапазоне 100-10000 для 4 условия вместо <{_conditions[3].text}>");
                return;
            }
        }
        if (_curLevel.CountConditions == 0)
        {
            ViewInputError("Не указано ни одного условия для победы! Задайте хотя бы одно такое условие!");
            return;
        }
        if (_bonuses[0].text != "")
        {
            string[] ar = _bonuses[0].text.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
            if (ar.Length >= 3)
            {
                if (int.TryParse(ar[1], out count)) _curLevel.AddBonus(new VictoryBonus(ar[0], count, ar[2]));
                else
                {
                    ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{ar[0]}><{ar[1]}><{ar[2]}>");
                    return;
                }
            }
            else
            {
                ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{_bonuses[0]}>");
                return;
            }
        }
        if (_bonuses[1].text != "")
        {
            string[] ar = _bonuses[1].text.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
            if (ar.Length >= 3)
            {
                if (int.TryParse(ar[1], out count)) _curLevel.AddBonus(new VictoryBonus(ar[0], count, ar[2]));
                else
                {
                    ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{ar[0]}><{ar[1]}><{ar[2]}>");
                    return;
                }
            }
            else
            {
                ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{_bonuses[1]}>");
                return;
            }
        }
        if (_bonuses[2].text != "")
        {
            string[] ar = _bonuses[2].text.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
            if (ar.Length >= 3)
            {
                if (int.TryParse(ar[1], out count)) _curLevel.AddBonus(new VictoryBonus(ar[0], count, ar[2]));
                else
                {
                    ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{ar[0]}><{ar[1]}><{ar[2]}>");
                    return;
                }
            }
            else
            {
                ViewInputError($"Ошибка формата ввода победного бонуса. Укажите через <;> категорию бонуса, число, название вместо <{_bonuses[2]}>");
                return;
            }
        }
        if (_curLevel.CountBonuse == 0)
        {
            ViewInputError($"Не указано ни одного победного бонуса. Укажите хотя бы в одном поле ввода бонусов через <;> категорию бонуса, число, название, например, < Технология; 1; Металлургия >");
            return;
        }
        _curLevel.SetTerain(new int[] { 0 });
        _createPanel.SetActive(false);
        InterBtnArr(true);
        // Уровень создан, надо как-то сообщить в EditorBoard о перерисовке уровня
        OnLevelChanged?.Invoke(_curLevel); // Уведомляем подписчиков
    }

    public void ViewCreatePanel()
    {
        _isNew = true;
        _createPanel.SetActive(true);
    }

    private void ViewInputError(string err)
    {
        _msgText.text = err;
        _msgPanel.SetActive(true);
    }

    public void SaveLevels()
    {
        LevelList.Instance.SaveLevels();
    }

    public void ChangePrizrak(bool value)
    {
        _prizrak.SetActive(value);
    }

    public void OnLandTypeClick(int num)
    {
        ChangePrizrak(true);
        _landTailsType = num;
        Image img1 = _landTailBtn1.image;
        Image img5 = _landTailBtn5.image;
        switch (num)
        {
            case 1:
                _landTailText.text = "Трава - Гора";
                img1.color = _landColor[0];
                img5.color = _landColor[1];
                break;
            case 2:
                _landTailText.text = "Трава - Река";
                img1.color = _landColor[0];
                img5.color = _landColor[2];
                break;
            case 3:
                _landTailText.text = "Песок - Трава";
                img1.color = _landColor[3];
                img5.color = _landColor[0];
                break;
            case 4:
                _landTailText.text = "Песок - Море";
                img1.color = _landColor[3];
                img5.color = _landColor[2];
                break;
            case 5:
                _landTailText.text = "Песок - Гора";
                img1.color = _landColor[3];
                img5.color = _landColor[1];
                break;
        }
        _landTailPanel.SetActive(true);
    }

    public void OnLandTailClick(int num)
    {
        ChangePrizrak(false);
        _landTailPanel.SetActive(false);
        OnSelectLandTail?.Invoke(_landTailsType, num);
    }

    public void OnSpecLandTailClick(int num)
    {
        ChangePrizrak(false);
        _specTailPanel.SetActive(false);
        OnSelectSpecTail?.Invoke(num);
    }

    public void ViewDelOrRotPanel(string info, bool isRot)
    {
        _delRotInfoText.text = info;
        _delRotBtn.interactable = isRot;
        _delRotPanel.SetActive(true);
    }

    public void OnActionTailClick(int value)
    {
        if (value == 1 || value == 3)
        {   //  удалить часть местности или закрыть окно
            _delRotPanel.SetActive(false);
        }
        OnDelOrRotTail?.Invoke(value);
    }
    public void ViewSelectLoadLevelPanel()
    {
        _levelShemaInfos = LevelList.Instance.GetLevelInfos();
        _curIndexLevels = 0;
        if (_levelShemaInfos.Count > _items.Length)
        {
            int index = Mathf.RoundToInt(_scrollbar.value * _levelShemaInfos.Count);
            if (index > _levelShemaInfos.Count - _items.Length) index = _levelShemaInfos.Count - _items.Length;
            _curIndexLevels = index;
        }
        _scrollbar.gameObject.SetActive(_levelShemaInfos.Count > _items.Length);
        _scrollbar.size = ((float)_items.Length) / _levelShemaInfos.Count;
        UpdateNumberLevelItems();
        _selectLoadLevelPanel.SetActive(true);
    }

    private void UpdateNumberLevelItems()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            GameObject item = _items[i]; //.gameObject;
            if (_curIndexLevels + i < _levelShemaInfos.Count)
            {
                Text[] arTxt = item.GetComponentsInChildren<Text>();
                //Text txtBtn = item.transform.GetChild(1).GetChild(0).GetComponent<Text>();

                if (arTxt != null && arTxt.Length >= 2)
                {
                    arTxt[0].text = $"{_levelShemaInfos[_curIndexLevels + i].LevelNumber:D02}.";
                    arTxt[1].text = _levelShemaInfos[_curIndexLevels + i].LevelName;
                }
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }
    public void SelectLoadLevel(int numItem)
    {
        //print($"NumItem => {numItem}");
        GameObject item = _items[numItem];
        Text[] arTxt = item.GetComponentsInChildren<Text>();
        if (arTxt != null && arTxt.Length >= 2 && int.TryParse(arTxt[0].text.Substring(0, 2), out int numLevel))
        {
            LevelShema tmp = LevelList.Instance.GetShemaLevel(new LevelShemaInfo(numLevel, arTxt[1].text));
            if (tmp != null)
            {
                _curLevel = tmp;
            }
        }
        // Уровень выбран, надо как-то сообщить в LevelBoard о перерисовке уровня
        OnLevelChanged?.Invoke(_curLevel); // Уведомляем подписчиков
        _selectLoadLevelPanel.SetActive(false);
    }

    public void SelectDeletingLevel(int numItem)
    {
        GameObject item = _items[numItem];
        Text[] arTxt = item.GetComponentsInChildren<Text>();
        if (arTxt != null && arTxt.Length >= 2 && int.TryParse(arTxt[0].text.Substring(0, 2), out int numLevel))
        {
            LevelShema tmp = LevelList.Instance.GetShemaLevel(new LevelShemaInfo(numLevel, arTxt[1].text));
            if (tmp != null)
            {
                _curLevel = tmp;
                ViewAskDelPanel();
            }
        }
    }

    private void ViewAskDelPanel()
    {
        _txtAskDel.text = $"Удалить {_curLevel.NumberLevel} уровень <{_curLevel.Name}> ?";
        _askDelPanel.SetActive(true);
    }

    public void DeletingLevel()
    {
        _askDelPanel.SetActive(false);
        _levelShemaInfos = LevelList.Instance.GetLevelInfosAfterDelLevel(new LevelShemaInfo(_curLevel.NumberLevel, _curLevel.Name));
        _curLevel = null;
        _curIndexLevels = 0;
        if (_levelShemaInfos.Count > _items.Length)
        {
            int index = Mathf.RoundToInt(_scrollbar.value * _levelShemaInfos.Count);
            if (index > _levelShemaInfos.Count - _items.Length) index = _levelShemaInfos.Count - _items.Length;
            _curIndexLevels = index;
        }
        _scrollbar.gameObject.SetActive(_levelShemaInfos.Count > _items.Length);
        _scrollbar.size = ((float)_items.Length) / _levelShemaInfos.Count;
        UpdateNumberLevelItems();
    }

    public void OnScrollValueChanged()
    {
        float zn = _scrollbar.value;
        int index = Mathf.RoundToInt(zn * _levelShemaInfos.Count);
        if (index > _levelShemaInfos.Count - 7) index = _levelShemaInfos.Count - 7;
        //print($"scrollValue = {value}   zn={zn}   index={index}");
        _curIndexLevels = index;
        UpdateNumberLevelItems();
    }
}
