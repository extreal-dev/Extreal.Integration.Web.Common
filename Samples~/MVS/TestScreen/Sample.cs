using System;
using AOT;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Web.Common.MVS.TestScreen
{
    public class Sample : DisposableBase
    {
        public IObservable<string> OnCallback => onCallback;
        private readonly Subject<string> onCallback = new Subject<string>();

        private static Sample instance;
        public Sample()
        {
            instance = this;
            WebGLHelper.AddCallback(nameof(HandleOnCallback), HandleOnCallback);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void HandleOnCallback(string str1, string str2)
            => instance.onCallback.OnNext($"received {str1} {str2} in callback");

        public void DoAction(string param1, string param2)
            => WebGLHelper.CallAction(nameof(DoAction), param1, param2);

        public string DoFunction(string param1, string param2)
            => WebGLHelper.CallFunction(nameof(DoFunction), param1, param2);

        public string DoTraceLogSuppressedFunction(string param1, string param2)
            => WebGLHelper.CallFunction(nameof(DoTraceLogSuppressedFunction), param1, param2);

        protected override void ReleaseManagedResources() => onCallback.Dispose();
    }
}
