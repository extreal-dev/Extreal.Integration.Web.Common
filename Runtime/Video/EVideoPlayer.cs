using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using UniRx;

namespace Extreal.Integration.Web.Common.Video
{
    /// <summary>
    /// Class that handles VideoPlayer.
    /// </summary>
    public abstract class EVideoPlayer : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(EVideoPlayer));

        /// <summary>
        /// Invokes immediately after the preparation is completed.
        /// </summary>
        public IObservable<Unit> OnPrepareCompleted => onPrepareCompleted.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onPrepareCompleted = new Subject<Unit>();

        /// <summary>
        /// Fires the OnPrepareCompleted.
        /// </summary>
        protected void FireOnPrepareCompleted()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(FireOnPrepareCompleted));
            }
            onPrepareCompleted.OnNext(Unit.Default);
        }

        /// <summary>
        /// <para>Invokes immediately after an error has occurred.</para>
        /// Arg: Error message
        /// </summary>
        public IObservable<string> OnErrorReceived => onErrorReceived.AddTo(Disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onErrorReceived = new Subject<string>();

        /// <summary>
        /// Fires the OnErrorReceived.
        /// </summary>
        /// <param name="message"></param>
        protected void FireOnErrorReceived(string message)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnErrorReceived)}: message={message}");
            }
            onErrorReceived.OnNext(message);
        }

        /// <summary>
        /// Disposables.
        /// </summary>
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        /// <summary>
        /// Video url.
        /// </summary>
        /// <value>Video url.</value>
        protected string Url { get; private set; }

        /// <summary>
        /// Video length.
        /// </summary>
        public abstract double Length { get; }

        /// <summary>
        /// Sets the video url.
        /// </summary>
        /// <param name="url">Video url.</param>
        public void SetUrl(string url) => Url = url;

        /// <summary>
        /// Sets the playback position.
        /// </summary>
        /// <param name="time">Playback position.</param>
        public abstract void SetTime(double time);

        /// <summary>
        /// Prepares the VideoPlayer.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// Plays the video.
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Pauses the video.
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Stops the video.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Sets the audio volume.
        /// </summary>
        /// <param name="volume">Audio volume (0.0 - 1.0).</param>
        /// <param name="trackIndex">Track index.</param>
        public abstract void SetAudioVolume(float volume, ushort trackIndex = default);

        /// <inheritdoc/>
        protected sealed override void ReleaseManagedResources()
        {
            DoReleaseManagedResources();
            Disposables.Dispose();
        }

        /// <summary>
        /// Releases managed resources.
        /// </summary>
        protected abstract void DoReleaseManagedResources();
    }
}
