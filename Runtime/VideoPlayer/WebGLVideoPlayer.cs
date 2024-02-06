#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;
using UnityEngine;

namespace Extreal.Integration.Web.Common.Video
{
    public class WebGLVideoPlayer : DisposableBase, IVideoPlayer
    {
        public IObservable<Unit> OnPrepareCompleted => onPrepareCompleted;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onPrepareCompleted;

        public IObservable<string> OnErrorReceived => onErrorReceived;
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<string> onErrorReceived;

        public double Length
            => double.Parse(WebGLHelper.CallFunction(WithPrefix("GetLength"), instanceId));

        private string url;
        private bool isPlaying;

        private Texture2D videoTexture;
        private readonly RenderTexture targetRenderTexture;

        private string instanceId;
        private static readonly Dictionary<string, WebGLVideoPlayer> Instances = new Dictionary<string, WebGLVideoPlayer>();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public WebGLVideoPlayer(RenderTexture targetRenderTexture)
        {
            this.targetRenderTexture = targetRenderTexture;

            onPrepareCompleted = new Subject<Unit>().AddTo(disposables);
            onErrorReceived = new Subject<string>().AddTo(disposables);

            instanceId = Guid.NewGuid().ToString();
            Instances[instanceId] = this;

            WebGLHelper.CallAction(WithPrefix(nameof(WebGLVideoPlayer)), instanceId);
            WebGLHelper.AddCallback(WithPrefix(nameof(CompletePreparation)), CompletePreparation);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveError)), ReceiveError);

            UniTask.Void(async () =>
            {
                await UniTask.Yield();
                while (!disposables.IsDisposed)
                {
                    Update();
                    await UniTask.Yield();
                }
            });
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void CompletePreparation(string instanceId, string unused)
            => Instances[instanceId].onPrepareCompleted.OnNext(Unit.Default);

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveError(string message, string instanceId)
            => Instances[instanceId].onErrorReceived.OnNext(message);

        protected override void ReleaseManagedResources()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(ReleaseManagedResources)), instanceId);
            targetRenderTexture.Release();
            disposables.Dispose();
        }

        private void Update()
        {
            if (isPlaying)
            {
                if (videoTexture)
                {
                    UnityEngine.Object.Destroy(videoTexture);
                    videoTexture = null;
                }
                videoTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                WebGLHelper.CallAction(WithPrefix("UpdateTexture"), videoTexture.GetNativeTexturePtr().ToString(), instanceId);
                Graphics.Blit(videoTexture, targetRenderTexture);
            }
        }

        public void SetUrl(string url)
            => this.url = url;

        public void SetTime(double time)
            => WebGLHelper.CallAction(WithPrefix(nameof(SetTime)), time.ToString(), instanceId);

        public void Prepare()
            => WebGLHelper.CallAction(WithPrefix(nameof(Prepare)), url, instanceId);

        public void Play()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Play)), instanceId);
            isPlaying = true;
        }

        public void Pause()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Pause)), instanceId);
            isPlaying = false;
        }

        public void Stop()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Stop)), instanceId);
            isPlaying = false;
        }

        public void SetAudioVolume(ushort trackIndex, float volume)
            => WebGLHelper.CallAction(WithPrefix(nameof(SetAudioVolume)), volume.ToString(), instanceId);

        private static string WithPrefix(string name)
            => $"{nameof(WebGLVideoPlayer)}#{name}";
    }
}
#endif
