# Extreal.Integration.Web.Common

## How to test

- Enter the following command in the `WebScripts~` directory.

   ```bash
   yarn
   yarn dev
   ```

- Import the sample MVS from Package Manager.
- Enter the following command in the `MVS/WebScripts` directory.

   ```bash
   yarn
   yarn dev
   ```

   The JavaScript code will be built and output to `/Assets/WebGLTemplates/Dev`.
- Open `Build Settings` and change the platform to `WebGL`.
- Select `Dev` from `Player Settings > Resolution and Presentation > WebGL Template`.
- Add all scenes in MVS to `Scenes In Build`.
- Run from `Build And Run`.
