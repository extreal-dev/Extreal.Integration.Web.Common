#if UNITY_WEBGL
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extreal.Integration.Web.Common
{
    /// <summary>
    /// Class for easy integration of C# and JavaScript with WebGL.
    /// </summary>
    public static class WebGLHelper
    {
        /// <summary>
        /// Initializes.
        /// </summary>
        /// <param name="webGLHelperConfig">WebGLHelper configuration</param>
        public static void Initialize(WebGLHelperConfig webGLHelperConfig = null)
            => Nop(JsonSerializer.Serialize(webGLHelperConfig ?? new WebGLHelperConfig { IsDebug = false }));

        [DllImport("__Internal")]
        private static extern void Nop(string str);

        /// <summary>
        /// Calls a function without a return value.
        /// </summary>
        /// <param name="name">Target</param>
        /// <param name="str1">First string parameter</param>
        /// <param name="str2">Second string parameter</param>
        [DllImport("__Internal")]
        public static extern void CallAction(string name, string str1 = "", string str2 = "");

        /// <summary>
        /// Calls a function with a return value.
        /// </summary>
        /// <param name="name">Target</param>
        /// <param name="str1">First string parameter</param>
        /// <param name="str2">Second string parameter</param>
        /// <returns>Return value</returns>
        [DllImport("__Internal")]
        public static extern string CallFunction(string name, string str1 = "", string str2 = "");

        /// <summary>
        /// Adds a callback.
        /// </summary>
        /// <param name="name">Target</param>
        /// <param name="action">Callback handler</param>
        [DllImport("__Internal")]
        public static extern void AddCallback(string name, Action<string, string> action);
    }

    /// <summary>
    /// Class to hold the WebGLHelper configuration.
    /// </summary>
    [SuppressMessage("Usage", "CC0047")]
    public class WebGLHelperConfig
    {
        /// <summary>
        /// Debug or not.
        /// </summary>
        [JsonPropertyName("isDebug")]
        public bool IsDebug { get; set; }
    }
}
#endif
