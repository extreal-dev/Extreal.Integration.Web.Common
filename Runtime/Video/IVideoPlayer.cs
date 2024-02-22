using System;
using UniRx;

namespace Extreal.Integration.Web.Common.Video
{
    public interface IVideoPlayer : IDisposable
    {
        IObservable<Unit> OnPrepareCompleted { get; }
        IObservable<string> OnErrorReceived { get; }
        double Length { get; }
        void Clear();
        void SetUrl(string url);
        void SetTime(double time);
        void Prepare();
        void Play();
        void Pause();
        void Stop();
        void SetAudioVolume(ushort trackIndex, float volume);
    }
}
