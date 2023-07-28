using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS.TextChatScreen
{
    public class TestScreenScope : LifetimeScope
    {
        [FormerlySerializedAs("textChatScreenView")] [SerializeField] private TestScreenView testScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(testScreenView);

            builder.RegisterEntryPoint<TestScreenPresenter>();
        }
    }
}
