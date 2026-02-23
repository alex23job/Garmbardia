using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceUI : MonoBehaviour
{
    // Делегат для уведомления о числе недостающих очков для изучения технологии и
    // получении возможного числа очков для добавления к имеющимся в этой технологии 
    public delegate int InvestedPointsEventHandler(int delta);
    public event InvestedPointsEventHandler OnInvestedPointsClick;

    [SerializeField] private Image[] _arrows;
    [SerializeField] private Image[] _scienceItems;
    [SerializeField] private TechnologyRepository _repository;

    private Color[] _itemColors = new Color[3] { new Color(0.7f, 1f, 0.7f, 1f), new Color(1f, 0.7f, 0.7f, 1f), new Color(1f, 1f, 0.7f, 1f) };    //  green, red, yellow

    private LevelUI _levelUI = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelUI(LevelUI levelUI)
    {
        _levelUI = levelUI;
    }

    public void UpdateSciences()
    {
        if (_repository == null) return;
        List<Technology> list = _repository.Technologies;
        int i, j;
        for(i = 0;  i < _scienceItems.Length; i++)
        {
            if (i < list.Count)
            {
                Technology techno = list[i];
                Button incBtn = _scienceItems[i].transform.GetChild(0).gameObject.GetComponent<Button>();
                if (incBtn != null) incBtn.interactable = !techno.IsResearched;

                Text title = _scienceItems[i].transform.GetChild(1).gameObject.GetComponent<Text>();
                if (title != null) title.text = techno.Title;

                Text progressCost = _scienceItems[i].transform.GetChild(6).gameObject.GetComponent<Text>();
                if (progressCost != null) progressCost.text = $"{techno.InvestedSciencePoints}/{techno.SciencePointsCost}";

                for (j = 0; j < 2; j++)
                {
                    Image img = _scienceItems[i].transform.GetChild(2 + j).gameObject.GetComponent<Image>();
                    Text txt = _scienceItems[i].transform.GetChild(4 + j).gameObject.GetComponent<Text>();
                    if (j < techno.UnlocksEntity.Length)
                    {
                        if (img != null) img.gameObject.SetActive(true);
                        if (_levelUI != null)
                        {
                            BuildingInfo info = _levelUI.GetBuildingInfo(techno.UnlocksEntity[j]);
                            if (info != null)
                            {
                                if (img != null) img.sprite = info.Sprite;
                            }
                        }

                        if (txt != null) 
                        {
                            txt.gameObject.SetActive(true);
                            txt.text = techno.UnlocksEntity[j]; 
                        }
                    }
                    else
                    {
                        //if (img != null) img.gameObject.SetActive(false);
                        //if (txt != null) txt.gameObject.SetActive(false);
                        if (img != null) _scienceItems[i].transform.GetChild(2 + j).gameObject.SetActive(false);
                        if (txt != null) _scienceItems[i].transform.GetChild(4 + j).gameObject.SetActive(false);
                    }
                }
                if (i < _arrows.Length)
                {
                    if (techno.IsResearched) _arrows[i].color = _itemColors[0];
                    else
                    {
                        if (techno.InvestedSciencePoints > 0) _arrows[i].color = _itemColors[2];
                        else _arrows[i].color = _itemColors[1];
                    }
                }
            }
        }
    }

    public void InvestPoints(int index)
    {
        if (_repository == null) return;
        List<Technology> list = _repository.Technologies;
        if (index < list.Count)
        {
            Technology techno = list[index];
            int addingPoints = (int)(OnInvestedPointsClick?.Invoke(techno.DeltaPoints));
            techno.AddSciencePoints(addingPoints);
            if (addingPoints > 0)
            {
                Button incBtn = _scienceItems[index].transform.GetChild(0).gameObject.GetComponent<Button>();
                if (incBtn != null) incBtn.interactable = !techno.IsResearched;

                Text progressCost = _scienceItems[index].transform.GetChild(6).gameObject.GetComponent<Text>();
                if (progressCost != null) progressCost.text = $"{techno.InvestedSciencePoints}/{techno.SciencePointsCost}";
                if (index < _arrows.Length)
                {
                    if (techno.IsResearched) _arrows[index].color = _itemColors[0];
                    else
                    {
                        if (techno.InvestedSciencePoints > 0) _arrows[index].color = _itemColors[2];
                        else _arrows[index].color = _itemColors[1];
                    }
                }

            }
        }
    }
}
