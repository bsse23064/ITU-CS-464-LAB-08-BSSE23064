using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Act { None, Go, Back, Root, Replace, Quit, LoadScene }
    public Act action = Act.None;
    public string target;

    Vector3 baseScale;
    Coroutine running;

    void Awake()
    {
        baseScale = Vector3.one;
        GetComponent<Button>().onClick.AddListener(Click);
    }

    void OnDisable() { transform.localScale = baseScale; }

    void Click()
    {
        var ui = UIManager.I;
        switch (action)
        {
            case Act.Go: ui.Go(target); break;
            case Act.Back: ui.Back(); break;
            case Act.Root: ui.Root(target); break;
            case Act.Replace: ui.Replace(target); break;
            case Act.Quit: QuitGame(); break;
        }
    }

    static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // stops Play mode in the editor
#else
        Application.Quit();                                // closes the built app
#endif
    }

    public void OnPointerDown(PointerEventData e) => Scale(0.94f, 0.08f, Easing.OutQuad);
    public void OnPointerUp(PointerEventData e) => Scale(1f, 0.25f, Easing.OutBack);
    public void OnPointerExit(PointerEventData e) => Scale(1f, 0.25f, Easing.OutBack);

    void Scale(float mult, float dur, System.Func<float, float> ease)
    {
        if (!isActiveAndEnabled) return;
        if (running != null) StopCoroutine(running);
        Vector3 from = transform.localScale, to = baseScale * mult;
        running = this.Run(dur, ease, t => transform.localScale = Vector3.LerpUnclamped(from, to, t));
    }
}