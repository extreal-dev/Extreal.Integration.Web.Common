using System.Collections.Generic;

namespace Extreal.Integration.Web.Common.MVS.App
{
    public static class AppUtils
    {
        private static readonly HashSet<StageName> SpaceStages = new()
        {
            StageName.TestStage
        };

        public static bool IsSpace(StageName stageName)
            => SpaceStages.Contains(stageName);
    }
}
