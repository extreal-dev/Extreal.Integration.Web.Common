using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.StageNavigation;
using Extreal.Integration.Web.Common.MVS2.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.TestScreen
{
    public class TitleScreenPresenter : DisposableBase, IInitializable
    {
        private readonly TitleScreenView titleScreenView;
        private readonly AppState appState;
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("CodeCracker", "CC0057")]
        public TitleScreenPresenter(TitleScreenView titleScreenView, AppState appState, StageNavigator<StageName, SceneName> stageNavigator)
        {
            this.titleScreenView = titleScreenView;
            this.appState = appState;
            this.stageNavigator = stageNavigator;
        }

        public void Initialize()
        {
            titleScreenView.OnVideoUrlChanged
                .Subscribe(appState.SetVideoUrl)
                .AddTo(disposables);

            titleScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VideoStage).Forget())
                .AddTo(disposables);

            const string initialVideoUrl = "http://localhost:3333/Panorama/testvideo.mp4";
            titleScreenView.Initialize(initialVideoUrl);
            appState.SetVideoUrl(initialVideoUrl);
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
