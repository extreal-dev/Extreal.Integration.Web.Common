﻿using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using VContainer.Unity;

namespace Extreal.Integration.Web.Common.MVS2.App
{
    public class AppPresenter : IStartable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        [SuppressMessage("CodeCracker", "CC0057")]
        public AppPresenter(StageNavigator<StageName, SceneName> stageNavigator)
            => this.stageNavigator = stageNavigator;

        public void Start()
            => stageNavigator.ReplaceAsync(StageName.TitleStage).Forget();
    }
}
