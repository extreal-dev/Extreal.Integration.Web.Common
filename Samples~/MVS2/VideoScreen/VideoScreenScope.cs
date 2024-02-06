using Extreal.Integration.Web.Common.Video;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.VideoScreen
{
    public class VideoScreenScope : LifetimeScope
    {
        [SerializeField] private VideoScreenView videoScreenView;
        [SerializeField] private EVideoPlayer videoPlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(videoScreenView);
            builder.RegisterComponent(videoPlayer);

            builder.RegisterEntryPoint<VideoScreenPresenter>();
        }
    }
}
