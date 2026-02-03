using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] private Button[] tailButtons;
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

    [SerializeField] private GameObject _msgPanel;
    [SerializeField] private Text _msgText;
 
    private Color _baseColor = new Color(0.7f, 1f, 0.9f, 1f), _selectColor = new Color(1f, 0.9f, 0.7f, 1f);
    private Color[] _landColor = new Color[4] { new Color(8f, 0.7f, 0.3f, 1f), new Color(0.1f, 0.8f, 0.1f, 1f), new Color(0.6f, 0.6f, 0.6f, 1f), new Color(0.3f, 0.3f, 0.9f, 1f) };

    private LevelShema _curLevel = null;
    private bool _isNew = true;
    private int _landTailsType = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectTail(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SelectTail(int num)
    {
        int i;
        for (i = 0; i < tailButtons.Length; i++)
        {
            tailButtons[i].gameObject.GetComponent<Image>().color = (i != num) ? _baseColor : _selectColor; 
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

    public void OnLandTypeClick(int num)
    {
        _landTailsType = num;
        Image img1 = _landTailBtn1.image;
        Image img5 = _landTailBtn5.image;
        switch (num)
        {
            case 1:
                _landTailText.text = "Трава - Река";
                img1.color = _landColor[1];
                img5.color = _landColor[3];
                break;
            case 2:
                _landTailText.text = "Трава - Гора";
                img1.color = _landColor[1];
                img5.color = _landColor[2];
                break;
            case 3:
                _landTailText.text = "Песок - Трава";
                img1.color = _landColor[0];
                img5.color = _landColor[1];
                break;
            case 4:
                _landTailText.text = "Песок - Море";
                img1.color = _landColor[0];
                img5.color = _landColor[3];
                break;
            case 5:
                _landTailText.text = "Песок - Гора";
                img1.color = _landColor[0];
                img5.color = _landColor[2];
                break;
        }
        _landTailPanel.SetActive(true);
    }

    public void OnLandTailClick(int num)
    {
        _landTailPanel.SetActive(false);
        OnSelectLandTail?.Invoke(_landTailsType, num);
    }

    public void OnSpecLandTailClick(int num)
    {
        _specTailPanel.SetActive(false);
        OnSelectSpecTail?.Invoke(num);
    }
}
