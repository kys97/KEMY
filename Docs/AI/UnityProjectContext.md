# Unity Project Context

<!-- unity-onboarding:generated:start -->

## Project Summary

- Project root: `C:/Users/lolla/Documents/GitHub/KEMY`
- Last analyzed: 2026-09-25
- Last analyzed commit: `c8016d77f86cbac0f0c3c2da8470bde06bd068e2`
- Mobile Unity game using Firebase Authentication/Realtime Database and Photon PUN.

## Confirmed Environment

- Unity: 6000.5.8f1 (local working tree upgrade; Git has related uncommitted changes)
- Render pipeline: Built-in pipeline with post-processing; Shader Graph package is also present.
- Input: legacy Input Manager usage.
- Active target: Android; package id `com.DefaultCompany.KEMY`, version 1.0.2, min SDK 28, target SDK 34, ARMv7.

## Important Packages And Frameworks

| Area | Finding | Evidence |
| --- | --- | --- |
| Authentication/data | Firebase Unity SDK 11.6.0, Auth and Realtime Database | `Assets/Firebase/Editor/*_version-11.6.0_manifest.txt` |
| Networking | Photon PUN 2.43 | `Assets/Photon/PhotonUnityNetworking/Code/PhotonNetwork.cs` |
| UI | uGUI and TextMesh Pro prefabs loaded through Resources | `Assets/Resources/UI`, `Assets/Scripts/Manager/UIManager.cs` |
| Tests | Unity Test Framework 1.7.0; no first-party test assembly found | `Packages/manifest.json` |

## Directory Structure And Architecture

- First-party runtime code: `Assets/Scripts` (MonoBehaviour managers and UI controllers).
- Scenes: `Assets/Scenes`; UI/prefabs: `Assets/Resources`.
- Save data uses `DataClass` JSON plus Firebase `UserInfo/{uid}` records.
- No first-party asmdef; runtime scripts compile into Assembly-CSharp.
- Authentication/data boundary: `FirebaseAccountService`; `GameManager` coordinates startup and UI routing.

## Scenes And Startup Flow

- Enabled build scenes, in order: Home, RunMenu, RunGame, RunGameExplain, Quiz, Kamp.
- Login and Name scene entries are disabled. Home is the composition root and startup scene.
- Home initializes managers, waits for Firebase, checks `FirebaseAuth.CurrentUser`, loads `UserInfo/{uid}`, then shows Login or Home UI. 신규 가입은 Login UI의 서버 인증 패널에서 완료되며 SetName으로 이동하지 않는다.

## Email-code account registration (2026-09-27)

- Email is the sole login identifier. Legacy profiles remain usable without a `loginId`; UID and game data are not rewritten.
- `functions/` provides nickname/phone availability, email-code send/check, and final registration endpoints.
- The server owns verification sessions, HMAC nickname/phone indexes, private phone records, registration state, and rate limits under `serverOnly`.
- Final signup creates Firebase Auth and the default profile only after a server-verified one-time grant. Registration retries use a deterministic UID and never overwrite an existing profile.
- `JoinUI` is the registration controller type. The intended `Assets/Resources/UI/Join.prefab` wiring uses its 16 serialized references plus `JoinScrollEventTrigger`, but the 2026-10-01 prefab snapshot does not currently contain those components.
- Setup and Inspector wiring are documented in `Docs/AI/AccountRegistrationSetup.md`.

## Coding And Serialization Constraints

- Scripts use global namespaces and explicit Resources/UI names.
- Preserve prefab/scene GUIDs and child ordering. Account UI scripts use serialized Inspector references rather than child indices.
- Preserve Firebase field names: `name`, `coin`, `heart`, `skinSet`, `faceSet`, `skinList`, `faceList`.
- Do not treat database errors as missing profiles and do not overwrite existing profiles during registration retry.

## Validation And Tooling

- Connected Unity Editor: available locally, Android target active; no Unity MCP tools exposed.
- Generated C# project compilation is available as fallback.
- Current Editor session prevented a second batch-mode Unity instance and APK build from running.

## Important Unknowns And Risks

- Firebase console Auth provider and Realtime Database security rules require external verification.
- No existing identity-verification channel exists for email recovery.
- Existing unrelated working-tree changes include a Unity upgrade, Photon compatibility edits, and TMP imports.
- Device runtime, real Firebase account flows, and Android signing were not verified during onboarding.

## Source Files Inspected

- `ProjectSettings/ProjectVersion.txt`, `ProjectSettings/EditorBuildSettings.asset`, `ProjectSettings/ProjectSettings.asset`
- `Packages/manifest.json`, `Packages/packages-lock.json`
- `Assets/Scripts/Manager/{GameManager,JsonManager,DataClass,UIManager}.cs`
- `Assets/Scripts/UI/{LoginUI,SetNameUI,HomeUI}.cs`
- `Assets/Resources/UI/{Login,SetName,Home}.prefab`, `Assets/Scenes/Home.unity`

<!-- unity-onboarding:generated:end -->
