using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Paste : MonoBehaviour, IPointerDownHandler
{
    public TMP_InputField yourTMPInputField;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            yourTMPInputField.text = GUIUtility.systemCopyBuffer;
        }
    }
}
