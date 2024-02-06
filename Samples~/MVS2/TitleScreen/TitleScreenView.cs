using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.Integration.Web.Common.MVS2.TestScreen
{
    public class TitleScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField videoUrl;
        [SerializeField] private Button goButton;

        public IObservable<string> OnVideoUrlChanged
            => videoUrl.onEndEdit.AsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> OnGoButtonClicked
            => goButton.OnClickAsObservable().TakeUntilDestroy(this);

        public void Initialize(string videoUrl)
            => this.videoUrl.text = videoUrl;
    }
}
