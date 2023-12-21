using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using UniRx;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS.TestScreen
{
    public class TestScreenPresenter : DisposableBase, IInitializable
    {
        private readonly TestScreenView testScreenView;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SuppressMessage("CodeCracker", "CC0057")]
        public TestScreenPresenter(TestScreenView testScreenView) => this.testScreenView = testScreenView;

        public void Initialize()
        {
            var sample = new Sample().AddTo(disposables);

            testScreenView.OnActionButtonClicked
                .Subscribe(parameters => sample.DoAction(parameters.Param1, parameters.Param2))
                .AddTo(disposables);

            testScreenView.OnFunctionButtonClicked
                .Subscribe(parameters =>
                {
                    var result = sample.DoFunction(parameters.Param1, parameters.Param2);
                    testScreenView.ShowResult(result);
                })
                .AddTo(disposables);

            sample.OnCallback
                .Subscribe(testScreenView.ShowResult)
                .AddTo(disposables);

            testScreenView.OnSuppressDoActionTraceLogButtonClicked
                .Subscribe(_ => sample.SuppressDoActionTraceLog())
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
