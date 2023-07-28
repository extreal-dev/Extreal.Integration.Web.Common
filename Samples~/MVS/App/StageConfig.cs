using Extreal.Core.StageNavigation;
using UnityEngine;

namespace Extreal.Integration.Web.Common.MVS.App
{
    [CreateAssetMenu(
        menuName = "Web.Common/" + nameof(StageConfig),
        fileName = nameof(StageConfig))]
    public class StageConfig : StageConfigBase<StageName, SceneName>
    {
    }
}
