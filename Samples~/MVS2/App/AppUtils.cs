using System.Collections.Generic;

namespace Extreal.Integration.Web.Common.MVS2.App
{
    public static class AppUtils
    {
        private static readonly HashSet<StageName> SpaceStages = new()
        {
            StageName.TitleStage
        };

        public static bool IsSpace(StageName stageName)
            => SpaceStages.Contains(stageName);
    }
}
