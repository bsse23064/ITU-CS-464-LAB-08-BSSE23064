using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }     // ADDED

    [SerializeField] AudioSource a;
    [SerializeField] AudioSource b;
    [SerializeField] float fadeSeconds = 1.5f;
    AudioSource current;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }   // ADDED: one music player, survives loads
        Instance = this;
        DontDestroyOnLoad(gameObject);

        current = a;
        a.loop = b.loop = true;                                  // ADDED
        a.ignoreListenerPause = true;                            // music keeps playing under the pause menu
        b.ignoreListenerPause = true;
    }

    public void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;                                // ADDED
        if (current.clip == clip && current.isPlaying) return;
        AudioSource next = current == a ? b : a;
        next.clip = clip;
        next.volume = 0f;
        next.Play();
        StopAllCoroutines();
        StartCoroutine(Fade(current, next));
        current = next;
    }

    IEnumerator Fade(AudioSource from, AudioSource to)
    {
        float start = from.volume;
        for (float t = 0f; t < fadeSeconds; t += Time.unscaledDeltaTime)
        {
            float u = t / fadeSeconds;
            from.volume = start * Mathf.Cos(u * Mathf.PI / 2f);  // equal-power crossfade
            to.volume = Mathf.Sin(u * Mathf.PI / 2f);
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }
}