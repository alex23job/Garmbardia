using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _hintPanel;
    [SerializeField] private Text _txtHint;

    private string _hintText = "";

    public void OnPointerEnter(PointerEventData eventData)
    {        
        RectTransform rectHint = _hintPanel.GetComponent<RectTransform>();
        RectTransform rectButton = gameObject.GetComponent<RectTransform>();
        //Vector3 hintPos = new Vector3(transform.position.x + rectButton.rect.width / 2, transform.position.y + rectButton.rect.height / 2, _hintPanel.transform.position.z);
        Vector3 hintPos = new Vector3(transform.position.x + rectButton.rect.width / 3, transform.position.y + rectButton.rect.height * 0.9f, _hintPanel.transform.position.z);
        _hintPanel.transform.position = hintPos;
        _txtHint.text = _hintText;

        //print($"posBtn={transform.position} (w={rectButton.rect.width}, h={rectButton.rect.height})  posHint={hintPos} (w={rectHint.rect.width}, h={rectHint.rect.height})");
        _hintPanel.SetActive(true);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hintPanel.SetActive(false);
    }

    public void SetTextHint(string hint)
    {
        _hintText = hint;
    }
}
