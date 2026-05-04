using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Toggle))]
public class ToggleColorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly Color ColorOff = new Color32(0x80, 0x80, 0x80, 0xFF);
    private static readonly Color ColorOn = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

    private Toggle _toggle;
    private Image _targetImage;
    private bool _isHovered;

    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _targetImage = _toggle.targetGraphic as Image;

        _toggle.transition = Selectable.Transition.None;

        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    void Start() => Refresh();

    void OnDestroy() => _toggle.onValueChanged.RemoveListener(OnValueChanged);

    public void OnPointerEnter(PointerEventData _)
    {
        _isHovered = true;
        Refresh();
    }

    public void OnPointerExit(PointerEventData _)
    {
        _isHovered = false;
        Refresh();
    }

    void OnValueChanged(bool _) => Refresh();

    void Refresh()
    {
        if (_targetImage == null) return;
        _targetImage.color = (_toggle.isOn || _isHovered) ? ColorOn : ColorOff;
    }
}