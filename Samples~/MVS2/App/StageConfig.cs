using Extreal.Core.StageNavigation;
using UnityEngine;

namespace Extreal.Integration.Web.Common.MVS2.App
{
    [CreateAssetMenu(
        menuName = "Web.Common/MVS2/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : StageConfigBase<StageName, SceneName>
    {
    }
}
