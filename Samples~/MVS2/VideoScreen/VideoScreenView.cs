using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.Integration.Web.Common.MVS2.VideoScreen
{
    public class VideoScreenView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_InputField videoUrl;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_Text playButtonLabel;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button seekButton;
        [SerializeField] private TMP_InputField seekTime;
        [SerializeField] private TMP_Text totalTime;
        [SerializeField] private Slider volume;

        public IObservable<string> OnLoadButtonClicked
            => loadButton.OnClickAsObservable().Select(_ => videoUrl.text).TakeUntilDestroy(this);
        public IObservable<Unit> OnBackButtonClicked
            => backButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnPlayButtonClicked
            => playButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnStopButtonClicked
            => stopButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<string> OnSeekButtonClicked
            => seekButton.OnClickAsObservable().Select(_ => seekTime.text).TakeUntilDestroy(this);
        public IObservable<float> OnVolumeChanged
            => volume.onValueChanged.AsObservable().TakeUntilDestroy(this);

        public void Initialize()
        {
            seekTime.text = string.Empty;
            totalTime.text = "/";
            playButton.interactable = false;
            stopButton.interactable = false;
            seekButton.interactable = false;
        }

        public void SetPrepareCompleted(long totalTime)
        {
            this.totalTime.text = $"/ {totalTime} sec";
            playButton.interactable = true;
            stopButton.interactable = true;
            seekButton.interactable = true;
        }

        public void SetPlayLabel(string text)
            => playButtonLabel.text = text;

        public void SetVideoUrl(string url)
            => videoUrl.text = url;
    }
}
