using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UiButtonSound : MonoBehaviour, IPointerEnterHandler
{
    Button button;
    AnimatedButton anim;
    bool locked;

    void Awake()
    {
        button = GetComponent<Button>();
        anim = GetComponent<AnimatedButton>();
        locked = GetComponent<LockedLevel>() != null;
    }

    void OnEnable() => button.onClick.AddListener(OnClick);
    void OnDisable() => button.onClick.RemoveListener(OnClick);

    void OnClick()
    {
        if (UiSound.Instance == null) return;
        if (locked) { UiSound.Instance.Play(UiSound.Cue.Error); return; }
        bool isBack = anim != null && (anim.action == AnimatedButton.Act.Back ||
                                       anim.action == AnimatedButton.Act.Root ||
                                       anim.action == AnimatedButton.Act.Quit);
        UiSound.Instance.Play(isBack ? UiSound.Cue.Back : UiSound.Cue.Click);
    }

    public void OnPointerEnter(PointerEventData e)       // touch screens have no hover
    {
        if (Application.isMobilePlatform || !button.interactable || UiSound.Instance == null) return;
        UiSound.Instance.Play(UiSound.Cue.Hover);
    }
}