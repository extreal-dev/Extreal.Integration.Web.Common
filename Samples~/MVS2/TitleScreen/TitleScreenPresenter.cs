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
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("CodeCracker", "CC0057")]
        public TitleScreenPresenter(TitleScreenView titleScreenView, StageNavigator<StageName, SceneName> stageNavigator)
        {
            this.titleScreenView = titleScreenView;
            this.stageNavigator = stageNavigator;
        }

        public void Initialize() =>
            titleScreenView.OnGoButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.VideoStage).Forget())
                .AddTo(disposables);

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
