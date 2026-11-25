using UnityEngine;
using TMPro;

public class ResultsScreen : ScreenBase
{
    [SerializeField] RectTransform[] stars;
    [SerializeField] TMP_Text scoreText;
    public int earnedStars = 3;
    public int finalScore = 12450;

    void Awake()
    {
        foreach (var s in stars) s.localScale = Vector3.zero;   // hidden until the reward moment
    }

    protected override void OnShown()
    {
        if (UiSound.Instance != null) UiSound.Instance.Play(UiSound.Cue.Success);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].localScale = Vector3.zero;
            if (i < earnedStars)
                this.ScaleTo(stars[i], 1f, 0.5f, Easing.OutElastic, null, 0.2f * (i + 1));
        }
        this.Run(1.2f, Easing.OutCubic,
                 t => scoreText.text = Mathf.RoundToInt(finalScore * t).ToString("N0"));
    }
}