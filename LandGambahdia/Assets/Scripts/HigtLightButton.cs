using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HigtLightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image imgFone;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        imgFone.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Наведение курсора: показываем imgFone
        imgFone.enabled = true;
        print($"Enter to {gameObject.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Выход курсора: скрываем imgFone
        imgFone.enabled = false;
        print($"Exit from {gameObject.name}");
    }
}
