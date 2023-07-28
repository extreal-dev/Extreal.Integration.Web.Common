﻿using Extreal.Core.StageNavigation;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS.App
{
    public class AppPresenter : IStartable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        public AppPresenter(StageNavigator<StageName, SceneName> stageNavigator)
            => this.stageNavigator = stageNavigator;

        public void Start()
            => stageNavigator.ReplaceAsync(StageName.TestStage);
    }
}
