# Extreal.Integration.Web.Common

## How to test

1. Import the sample MVS from Package Manager.
1. Enter the following command in the `MVS/WebScripts` directory.
   ```
   $ yarn
   $ yarn dev
   ```
   The JavaScript code will be built and output to `/Assets/WebTemplates/Dev`.
1. Open `Build Settings` and change the platform to `WebGL`.
1. Select `Dev` from `Player Settings > Resolution and Presentation > WebGL Template`.
1. Add all scenes in MVS to `Scenes In Build`.
1. Run from `Build And Run`.
