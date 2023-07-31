using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS.TestScreen
{
    public class TestScreenScope : LifetimeScope
    {
        [SerializeField] private TestScreenView testScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(testScreenView);

            builder.RegisterEntryPoint<TestScreenPresenter>();
        }
    }
}
