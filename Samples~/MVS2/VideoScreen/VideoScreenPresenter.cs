using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Web.Common.MVS2.App;
using Extreal.Integration.Web.Common.Video;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.VideoScreen
{
    public class VideoScreenPresenter : DisposableBase, IInitializable
    {
        private readonly VideoScreenView videoScreenView;
        private readonly EVideoPlayer videoPlayer;
        private readonly AppState appState;
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private bool isPlaying;

        public VideoScreenPresenter
        (
            VideoScreenView videoScreenView,
            EVideoPlayer videoPlayer,
            AppState appState,
            StageNavigator<StageName, SceneName> stageNavigator
        )
        {
            this.videoScreenView = videoScreenView;
            this.videoPlayer = videoPlayer;
            this.appState = appState;
            this.stageNavigator = stageNavigator;
        }

        public void Initialize()
        {
            const string initialVideoUrl = "http://localhost:3333/Panorama/testvideo.mp4";
            videoScreenView.SetVideoUrl(initialVideoUrl);

            videoScreenView.OnLoadButtonClicked
                .Subscribe(url =>
                {
                    StopVideo();
                    InitializeView(url);
                })
                .AddTo(disposables);

            videoScreenView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.TitleStage).Forget())
                .AddTo(disposables);

            videoScreenView.OnPlayButtonClicked
                .Subscribe(_ =>
                {
                    if (isPlaying)
                    {
                        videoPlayer.Pause();
                        videoScreenView.SetPlayLabel("Play");
                    }
                    else
                    {
                        videoPlayer.Play();
                        videoScreenView.SetPlayLabel("Pause");
                    }
                    isPlaying = !isPlaying;
                })
                .AddTo(disposables);

            videoScreenView.OnStopButtonClicked
                .Subscribe(_ => StopVideo())
                .AddTo(disposables);

            videoScreenView.OnSeekButtonClicked
                .Subscribe(time => videoPlayer.SetTime(double.Parse(time)))
                .AddTo(disposables);

            videoScreenView.OnVolumeChanged
                .Subscribe(volume => videoPlayer.SetAudioVolume(volume))
                .AddTo(disposables);

            videoPlayer.OnPrepareCompleted
                .Subscribe(_ =>
                {
                    var totalTime = (long)videoPlayer.Length;
                    videoScreenView.SetPrepareCompleted(totalTime);
                })
                .AddTo(disposables);

            videoPlayer.OnErrorReceived
                .Subscribe(appState.Notify)
                .AddTo(disposables);

            videoScreenView.Initialize();
        }

        private void InitializeView(string videoUrl)
        {
            videoScreenView.Initialize();

            videoPlayer.SetUrl(videoUrl);
            videoPlayer.Prepare();
        }

        private void StopVideo()
        {
            videoPlayer.Stop();
            videoScreenView.SetPlayLabel("Play");
            isPlaying = false;
        }

        protected override void ReleaseManagedResources()
            => disposables.Dispose();
    }
}
