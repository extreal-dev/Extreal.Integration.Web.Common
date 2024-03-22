import { updateTexture } from "../helper";

type VideoPlayerCallbacks = {
  onPrepareCompleted: () => void;
  onErrorOccurred: (message: string) => void;
};

class VideoPlayer {
  private callbacks: VideoPlayerCallbacks;
  private element: HTMLVideoElement | null = null;

  constructor(gameObjectId: string, callbacks: VideoPlayerCallbacks) {
    this.callbacks = callbacks;
    const videoElementId = this.getVideoElementId(gameObjectId);

    this.element = document.createElement("video");
    this.element.id = videoElementId;

    this.element.setAttribute("loop", "");
    this.element.setAttribute("playsinline", "");
    this.element.crossOrigin = "anonymous";
    this.element.volume = 1;
    this.element.muted = true;

    this.element.style.position = "absolute";
    this.element.style.visibility = "hidden";
    this.element.style.width = "0";
    this.element.style.height = "0";

    this.element.onerror = (_event, _source, _lineno, _colno, error) => {
      const message = error?.message ?? "Unknown error has occurred";
      this.callbacks.onErrorOccurred(message);
    }

    document.getElementById("unity-container")?.appendChild(this.element);
  };

  public releaseManagedResources = () => {
    if(this.element) {
      document.getElementById("unity-container")?.removeChild(this.element);
      this.element = null;
    }
  };

  public prepare = (filePath: string) => {
    var elem = this.getVideoElement();
    elem.setAttribute("src", filePath);
    const completePreparationFunc = () => {
      this.callbacks.onPrepareCompleted();
      elem.removeEventListener("canplay", completePreparationFunc);
    }
    elem.addEventListener("canplay", completePreparationFunc)
  };

  public play = () => {
    var elem = this.getVideoElement();

    elem.muted = true;
    elem.play();

    if (this.isIOSSafari()) {
      const unmuteFunc = () => {
        if (this.element) {
          this.element.muted = false;
        }
        document.getElementById("unity-canvas")?.removeEventListener("touchstart", unmuteFunc);
        document.getElementById("unity-canvas")?.removeEventListener("mousedown", unmuteFunc);
        document.getElementById("unity-canvas")?.removeEventListener("keydown", unmuteFunc);
      };
      document.getElementById("unity-canvas")?.addEventListener("touchstart", unmuteFunc);
      document.getElementById("unity-canvas")?.addEventListener("mousedown", unmuteFunc);
      document.getElementById("unity-canvas")?.addEventListener("keydown", unmuteFunc);
    } else {
      elem.muted = false;
    }
  };

  public pause = () => {
    const elem = this.getVideoElement();
    elem.pause();
  };

  public stop = () => {
    const elem = this.getVideoElement();
    elem.pause();
    elem.currentTime = 0;
  };

  public updateTexture = (textureId: number) => {
    const elem = this.getVideoElement();
    updateTexture(elem, textureId);
  };

  public getLength = () => {
    const elem = this.getVideoElement();
    return elem.duration.toString();
  };

  public setTime = (time: number) => {
    const elem = this.getVideoElement();
    elem.currentTime = time;
  };

  public setAudioVolume = (volume: number) => {
    const elem = this.getVideoElement();
    elem.volume = volume;
  };

  private getVideoElementId = (gameObjectId: string) => "video_screen_" + gameObjectId;

  private isIOSSafari = () => /^((?!chrome|android).)*safari/i.test(navigator.userAgent);

  private getVideoElement = () => {
    if (!this.element) {
      throw new Error(`This instance is already cleared`);
    }
    return this.element;
  };
}

export { VideoPlayer }
