# Extreal.Integration.Web.Common

## How to test

- Enter the following command in the `WebScripts~` directory.

   ```bash
   yarn
   yarn dev
   ```

- Import the sample MVS or MVS2 from Package Manager.
- Enter the following command in the `WebScripts~` directory under the imported directory.

   ```bash
   yarn
   yarn dev
   ```

   The JavaScript code will be built and output to `/Assets/WebGLTemplates/Dev`.
- Open `Build Settings` and change the platform to `WebGL`.
- Select `Dev` from `Player Settings > Resolution and Presentation > WebGL Template`.
- Add all scenes in MVS to `Scenes In Build`.
- Play
   - See [README](https://github.com/extreal-dev/Extreal.Dev/blob/main/WebGLBuild/README.md) to run WebGL application in local environment.

## Test cases for manual testing

### MVS (main feature of this library)

Listed above the scene played.

### MVS2 (video feature)

- Title screen
  - Ability to specify url (used when set url to VideoPlayer)
  - Ability to go to video screen (initialize, set url, prepare)
- Video screen
  - Display the length of the video (video preparation is completed, get video length)
  - Ability to play video (play)
  - Ability to pause video (pause)
  - Ability to stop video (stop)
  - Ability to adjust volume (set volume)
  - Error notification if specified url is invalid (error is received)
  - Ability to return title screen (dispose VideoPlayer)
