using UnityEngine;
using UnityEngine.EventSystems;

public class OptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.MouseOnOptionButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MenuManager.Instance.MouseOnOptionButton = false;
    }
}
