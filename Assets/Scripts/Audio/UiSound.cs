using UnityEngine;

public class UiSound : MonoBehaviour
{
    public enum Cue { Hover, Click, Back, Open, Close, Success, Error }
    public static UiSound Instance { get; private set; }

    [SerializeField] AudioSource source;          // Output = UI group
    [SerializeField] AudioClip[] hover, click, back, open, close, success, error;
    [SerializeField] float pitchRange = 0.5f;     // ± semitones, small for UI
    [SerializeField] float minVolume = 0.9f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        source.ignoreListenerPause = true;        // UI sounds still play while the game is paused
    }

    public void Play(Cue cue)
    {
        AudioClip[] set = cue == Cue.Hover ? hover : cue == Cue.Click ? click : cue == Cue.Back ? back
                        : cue == Cue.Open ? open : cue == Cue.Close ? close : cue == Cue.Success ? success : error;
        if (set == null || set.Length == 0) return;
        source.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        source.PlayOneShot(set[Random.Range(0, set.Length)], Random.Range(minVolume, 1f));
    }
}