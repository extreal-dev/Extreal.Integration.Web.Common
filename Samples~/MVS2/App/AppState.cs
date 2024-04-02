using System;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Web.Common.MVS2.App
{
    public class AppState : DisposableBase
    {
        public IObservable<string> OnNotificationReceived => onNotificationReceived.AddTo(disposables);
        private readonly Subject<string> onNotificationReceived;
        public void Notify(string message) => onNotificationReceived.OnNext(message);

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public AppState()
            => onNotificationReceived = new Subject<string>().AddTo(disposables);

        protected override void ReleaseManagedResources()
            => disposables.Dispose();
    }
}
