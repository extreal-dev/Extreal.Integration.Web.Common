using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Video;

namespace Extreal.Integration.Web.Common.Video
{
    /// <summary>
    /// Class that provides EVideoPlayer.
    /// </summary>
    public static class EVideoPlayerProvider
    {
        /// <summary>
        /// Provides the EVideoPlayer.
        /// </summary>
        /// <remarks>
        /// Creates and returns a EVideoPlayer for Native (C#) or WebGL (JavaScript) depending on the platform.
        /// </remarks>
        /// <param name="videoPlayer">Video player for Native (C#)</param>
        /// <param name="targetRenderTexture">Target render texture for WebGL</param>
        /// <returns></returns>
        [SuppressMessage("Usage", "CC0038"), SuppressMessage("Usage", "IDE0022"), SuppressMessage("Usage", "CC0057")]
        public static EVideoPlayer Provide(VideoPlayer videoPlayer, RenderTexture targetRenderTexture)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            return new NativeEVideoPlayer(videoPlayer);
#else
            return new WebGLEVideoPlayer(targetRenderTexture);
#endif
        }
    }
}
