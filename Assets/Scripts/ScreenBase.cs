using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenBase : MonoBehaviour
{
    [Tooltip("Overlay screens (Pause) appear on top of the current screen")]
    public bool isOverlay;
    [SerializeField] RectTransform popPanel;

    CanvasGroup cg;
    RectTransform rt;
    const float InDur = 0.35f, OutDur = 0.25f, Slide = 0.15f;

    void Cache()
    {
        if (cg != null) return;
        cg = GetComponent<CanvasGroup>();
        rt = (RectTransform)transform;
    }

    public void HideInstant()
    {
        Cache();
        StopAllCoroutines();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    // dir: +1 forward (slide from right), -1 back (from left), 0 fade only
    public void Show(int dir)
    {
        Cache();
        StopAllCoroutines();                 // cancel any half-finished Hide (before activating)
        gameObject.SetActive(true);
        cg.interactable = false;             // blocks double taps during the transition
        cg.blocksRaycasts = true;
        cg.alpha = 0;

        if (isOverlay && popPanel)
        {
            if (UiSound.Instance != null) UiSound.Instance.Play(UiSound.Cue.Open);
            rt.anchoredPosition = Vector2.zero;
            popPanel.localScale = Vector3.one * 0.8f;
            this.FadeTo(cg, 1f, InDur, Easing.OutQuad);
            this.ScaleTo(popPanel, 1f, InDur, Easing.OutBack, () => { cg.interactable = true; OnShown(); });
        }
        else
        {
            rt.anchoredPosition = new Vector2(dir * rt.rect.width * Slide, 0);
            this.FadeTo(cg, 1f, InDur, Easing.OutQuad);
            this.MoveTo(rt, Vector2.zero, InDur, Easing.OutCubic, () => { cg.interactable = true; OnShown(); });
        }
    }

    public void Hide(int dir)
    {
        Cache();
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        this.FadeTo(cg, 0f, OutDur, Easing.InQuad);

        if (isOverlay && popPanel)
        {
            if (UiSound.Instance != null) UiSound.Instance.Play(UiSound.Cue.Close);
            this.ScaleTo(popPanel, 0.9f, OutDur, Easing.InQuad, () => gameObject.SetActive(false));
        }
        else
            this.MoveTo(rt, new Vector2(-dir * rt.rect.width * Slide, 0), OutDur, Easing.InCubic,
                        () => gameObject.SetActive(false));
    }

    protected virtual void OnShown() { }
}