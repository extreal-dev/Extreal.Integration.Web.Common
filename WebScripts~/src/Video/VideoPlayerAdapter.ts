import { addAction, addFunction, callback } from "../helper";
import { VideoPlayer } from "./VideoPlayer";

class VideoPlayerAdapter {
  private videoPlayers = new Map<string, VideoPlayer>();

  public adopt = () => {
    addAction(this.withPrefix("WebGLVideoPlayer"), (instanceId) => {
      this.videoPlayers.set(instanceId, new VideoPlayer(instanceId, {
        onPrepareCompleted: () => callback(this.withPrefix("CompletePreparation"), instanceId),
        onErrorOccurred: (message) => callback(this.withPrefix("ReceiveError"), message, instanceId),
      }));
    });

    addAction(this.withPrefix("ReleaseManagedResources"), (instanceId) => this.getVideoPlayer(instanceId).releaseManagedResources());
    addAction(this.withPrefix("Prepare"), (filePath, instanceId) => this.getVideoPlayer(instanceId).prepare(filePath));
    addAction(this.withPrefix("Play"), (instanceId) => this.getVideoPlayer(instanceId).play());
    addAction(this.withPrefix("Pause"), (instanceId) =>this.getVideoPlayer(instanceId).pause());
    addAction(this.withPrefix("Stop"), (instanceId) =>this.getVideoPlayer(instanceId).stop());
    addAction(this.withPrefix("UpdateTexture"), (textureId, instanceId) => this.getVideoPlayer(instanceId).updateTexture(Number.parseInt(textureId)));
    addFunction(this.withPrefix("GetLength"), (instanceId) =>this.getVideoPlayer(instanceId).getLength());
    addAction(this.withPrefix("SetTime"), (time, instanceId) =>this.getVideoPlayer(instanceId).setTime(Number.parseFloat(time)));
    addAction(this.withPrefix("SetAudioVolume"), (volume, instanceId) =>this.getVideoPlayer(instanceId).setAudioVolume(Number.parseFloat(volume)));
  }

  private withPrefix = (name: string) => `WebGLVideoPlayer#${name}`;

  private getVideoPlayer = (instanceId: string) => {
    const videoPlayer = this.videoPlayers.get(instanceId);
    if (!videoPlayer) {
      throw new Error("Call the WebGLVideoPlayer constructor first in Unity.");
    }
    return videoPlayer;
  };
}

export { VideoPlayerAdapter }
