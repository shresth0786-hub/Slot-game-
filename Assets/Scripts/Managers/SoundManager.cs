using UnityEngine;

namespace SlotGame.Managers
{
    /// <summary>
    /// Simple sound manager for spin, win, jackpot, click.
    /// Plays AudioClips if assigned; gracefully handles missing clips.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Source")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Clips")]
        [SerializeField] private AudioClip spinClip;
        [SerializeField] private AudioClip reelStopClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip jackpotClip;
        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip buttonClip;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) { musicSource = gameObject.AddComponent<AudioSource>(); musicSource.loop = true; }
        }

        public void PlaySpin() => Play(spinClip);
        public void PlayReelStop() => Play(reelStopClip);
        public void PlayButton() => Play(buttonClip);
        public void PlayWin(bool jackpot)
        {
            Play(jackpot ? jackpotClip : winClip);
            // Optional: haptic
#if UNITY_ANDROID || UNITY_IOS
            if (jackpot) Handheld.Vibrate();
#endif
        }
        public void PlayLose() => Play(loseClip);

        private void Play(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
                sfxSource.PlayOneShot(clip);
        }

        public void SetMuted(bool muted)
        {
            if (sfxSource != null) sfxSource.mute = muted;
            if (musicSource != null) musicSource.mute = muted;
        }
    }
}
