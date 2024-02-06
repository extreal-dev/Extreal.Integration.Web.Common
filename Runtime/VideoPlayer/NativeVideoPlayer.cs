using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using UniRx;
using UnityEngine.Video;

namespace Extreal.Integration.Web.Common.Video
{
    public class NativeVideoPlayer : DisposableBase, IVideoPlayer
    {

        public IObservable<Unit> OnPrepareCompleted => onPrepareCompleted;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onPrepareCompleted;
        private void PrepareCompletedEventHandler(VideoPlayer videoPlayer)
            => onPrepareCompleted.OnNext(Unit.Default);

        public IObservable<string> OnErrorReceived => onErrorReceived;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onErrorReceived;
        private void ErrorReceivedEventHandler(VideoPlayer source, string message)
            => onErrorReceived.OnNext(message);

        public double Length
            => videoPlayer.length;

        private readonly VideoPlayer videoPlayer;
        private string url;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public NativeVideoPlayer(VideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
            onPrepareCompleted = new Subject<Unit>().AddTo(disposables);
            onErrorReceived = new Subject<string>().AddTo(disposables);
            this.videoPlayer.prepareCompleted += PrepareCompletedEventHandler;
            this.videoPlayer.errorReceived += ErrorReceivedEventHandler;
        }

        protected override void ReleaseManagedResources()
        {
            videoPlayer.prepareCompleted -= PrepareCompletedEventHandler;
            videoPlayer.errorReceived -= ErrorReceivedEventHandler;
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
            {
                videoPlayer.targetTexture.Release();
            }
            disposables.Dispose();
        }

        public void SetUrl(string url)
            => this.url = url;

        public void SetTime(double time)
            => videoPlayer.time = time;

        public void Prepare()
        {
            videoPlayer.url = url;
            videoPlayer.Prepare();
        }

        public void Play()
            => videoPlayer.Play();

        public void Pause()
            => videoPlayer.Pause();

        public void Stop()
            => videoPlayer.Stop();

        public void SetAudioVolume(ushort trackIndex, float volume)
            => videoPlayer.SetDirectAudioVolume(trackIndex, volume);
    }
}
