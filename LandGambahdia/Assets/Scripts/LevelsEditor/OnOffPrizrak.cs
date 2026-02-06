using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnOffPrizrak : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _prizrak;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _prizrak.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _prizrak.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _prizrak.SetActive(false);
    }
}
