using UnityEngine.Video;

namespace Extreal.Integration.Web.Common.Video
{
    public class NativeEVideoPlayer : EVideoPlayer
    {
        private void PrepareCompletedEventHandler(VideoPlayer videoPlayer)
            => FireOnPrepareCompleted();

        private void ErrorReceivedEventHandler(VideoPlayer source, string message)
            => FireOnErrorReceived(message);

        public override double Length
            => videoPlayer.length;

        private readonly VideoPlayer videoPlayer;

        public NativeEVideoPlayer(VideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
            this.videoPlayer.prepareCompleted += PrepareCompletedEventHandler;
            this.videoPlayer.errorReceived += ErrorReceivedEventHandler;
        }

        protected override void DoReleaseManagedResources()
        {
            videoPlayer.prepareCompleted -= PrepareCompletedEventHandler;
            videoPlayer.errorReceived -= ErrorReceivedEventHandler;
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
            {
                videoPlayer.targetTexture.Release();
            }
        }

        public override void SetTime(double time)
            => videoPlayer.time = time;

        public override void Prepare()
        {
            videoPlayer.url = Url;
            videoPlayer.Prepare();
        }

        public override void Play()
            => videoPlayer.Play();

        public override void Pause()
            => videoPlayer.Pause();

        public override void Stop()
            => videoPlayer.Stop();

        public override void SetAudioVolume(float volume, ushort trackIndex = default)
            => videoPlayer.SetDirectAudioVolume(trackIndex, volume);

        public override void Clear()
            => Stop();
    }
}
