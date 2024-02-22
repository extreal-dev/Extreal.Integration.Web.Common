#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Extreal.Integration.Web.Common.Video
{
    public class WebGLEVideoPlayer : EVideoPlayer
    {
        public override double Length
            => double.Parse(WebGLHelper.CallFunction(WithPrefix("GetLength"), instanceId));

        private bool isPlaying;

        private Texture2D videoTexture;
        private readonly RenderTexture targetRenderTexture;

        private string instanceId;
        private static readonly Dictionary<string, WebGLEVideoPlayer> Instances = new Dictionary<string, WebGLEVideoPlayer>();

        public WebGLEVideoPlayer(RenderTexture targetRenderTexture)
        {
            this.targetRenderTexture = targetRenderTexture;

            instanceId = Guid.NewGuid().ToString();
            Instances[instanceId] = this;

            WebGLHelper.CallAction(WithPrefix(nameof(WebGLEVideoPlayer)), instanceId);
            WebGLHelper.AddCallback(WithPrefix(nameof(CompletePreparation)), CompletePreparation);
            WebGLHelper.AddCallback(WithPrefix(nameof(ReceiveError)), ReceiveError);

            UniTask.Void(async () =>
            {
                await UniTask.Yield();
                while (!Disposables.IsDisposed)
                {
                    Update();
                    await UniTask.Yield();
                }
            });
        }

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void CompletePreparation(string instanceId, string unused)
            => Instances[instanceId].FireOnPrepareCompleted();

        [AOT.MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void ReceiveError(string message, string instanceId)
            => Instances[instanceId].FireOnErrorReceived(message);

        protected override void DoReleaseManagedResources()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(DoReleaseManagedResources)), instanceId);
            targetRenderTexture.Release();
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

        public override void SetTime(double time)
            => WebGLHelper.CallAction(WithPrefix(nameof(SetTime)), time.ToString(), instanceId);

        public override void Prepare()
            => WebGLHelper.CallAction(WithPrefix(nameof(Prepare)), Url, instanceId);

        public override void Play()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Play)), instanceId);
            isPlaying = true;
        }

        public override void Pause()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Pause)), instanceId);
            isPlaying = false;
        }

        public override void Stop()
        {
            WebGLHelper.CallAction(WithPrefix(nameof(Stop)), instanceId);
            isPlaying = false;
        }

        public override void SetAudioVolume(float volume, ushort trackIndex = default)
            => WebGLHelper.CallAction(WithPrefix(nameof(SetAudioVolume)), volume.ToString(), instanceId);

        public override void Clear()
        {
            Stop();
            WebGLHelper.CallAction(WithPrefix(nameof(Clear)), instanceId);
        }

        private static string WithPrefix(string name)
            => $"{nameof(WebGLEVideoPlayer)}#{name}";
    }
}
#endif
