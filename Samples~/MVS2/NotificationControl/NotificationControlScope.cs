using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.NotificationControl
{
    public class NotificationControlScope : LifetimeScope
    {
        [SerializeField] private NotificationControlView notificationControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(notificationControlView);

            builder.RegisterEntryPoint<NotificationControlPresenter>();
        }
    }
}
