using Extreal.Integration.Web.Common.Video;
using UnityEngine;
using UnityEngine.Video;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.VideoScreen
{
    public class VideoScreenScope : LifetimeScope
    {
        [SerializeField] private VideoScreenView videoScreenView;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RenderTexture renderTexture;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(videoScreenView);

            var eVideoPlayer = EVideoPlayerProvider.Provide(videoPlayer, renderTexture);
            builder.RegisterComponent(eVideoPlayer);

            builder.RegisterEntryPoint<VideoScreenPresenter>();
        }
    }
}
