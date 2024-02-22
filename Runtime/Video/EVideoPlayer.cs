using UnityEngine;
using UnityEngine.Video;
using System;
using Extreal.Core.Logging;
using UniRx;
using System.Diagnostics.CodeAnalysis;

namespace Extreal.Integration.Web.Common.Video
{
    /// <summary>
    /// Class that handles VideoPlayer depending on the platform.
    /// </summary>
    public class EVideoPlayer : MonoBehaviour
    {
        [Header("Use on platforms other than WebGL")]
        [SerializeField] private VideoPlayer videoPlayer;

        [Header("Use only on WebGL platform")]
        [SerializeField] private RenderTexture targetRenderTexture;

        /// <summary>
        /// Invokes immediately after the preparation is completed.
        /// </summary>
        public IObservable<Unit> OnPrepareCompleted => onPrepareCompleted.TakeUntilDestroy(this);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onPrepareCompleted = new Subject<Unit>();

        /// <summary>
        /// <para>Invokes immediately after an error has occurred.</para>
        /// Arg: Error message
        /// </summary>
        public IObservable<string> OnErrorReceived => onErrorReceived.TakeUntilDestroy(this);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onErrorReceived = new Subject<string>();

        /// <summary>
        /// Video length.
        /// </summary>
        public double Length
            => eVideoPlayer.Length;

        private string url;
        private IVideoPlayer eVideoPlayer;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(EVideoPlayer));

        /// <summary>
        /// Initializes the VideoPlayer.
        /// </summary>
        public void Initialize()
        {
            onErrorReceived.AddTo(this);
            onPrepareCompleted.AddTo(this);

            eVideoPlayer
#if UNITY_WEBGL && !UNITY_EDITOR
                = new WebGLVideoPlayer(targetRenderTexture)
#else
                = new NativeVideoPlayer(videoPlayer)
#endif
                .AddTo(this);

            eVideoPlayer.OnPrepareCompleted
                .Subscribe(onPrepareCompleted.OnNext)
                .AddTo(this);

            eVideoPlayer.OnErrorReceived
                .Subscribe(onErrorReceived.OnNext)
                .AddTo(this);
        }

        public void Clear()
            => eVideoPlayer.Clear();

        /// <summary>
        /// Sets the video url.
        /// </summary>
        /// <param name="url">Video url.</param>
        public void SetUrl(string url)
        {
            this.url = url;
            eVideoPlayer.SetUrl(this.url);
        }

        /// <summary>
        /// Sets the playback position.
        /// </summary>
        /// <param name="time">Playback position.</param>
        public void SetTime(double time)
            => eVideoPlayer.SetTime(time);

        /// <summary>
        /// Prepares the VideoPlayer.
        /// </summary>
        public void Prepare()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug("Prepare: " + url);
            }
            eVideoPlayer.Prepare();
        }

        /// <summary>
        /// Plays the video.
        /// </summary>
        public void Play()
            => eVideoPlayer.Play();

        /// <summary>
        /// Pauses the video.
        /// </summary>
        public void Pause()
            => eVideoPlayer.Pause();

        /// <summary>
        /// Stops the video.
        /// </summary>
        public void Stop()
            => eVideoPlayer.Stop();

        /// <summary>
        /// Sets the audio volume.
        /// </summary>
        /// <param name="volume">Audio volume (0.0 - 1.0).</param>
        /// <param name="trackIndex">Track index.</param>
        public void SetAudioVolume(float volume, ushort trackIndex = default)
        {
            var clampedVolume = Mathf.Clamp(volume, 0f, 1f);
            eVideoPlayer.SetAudioVolume(trackIndex, clampedVolume);
        }
    }
}
