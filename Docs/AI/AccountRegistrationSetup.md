# 이메일 인증 가입: 테스트 서버 설정

현재 가입 식별자는 이메일 하나다. 로그인 아이디·아이디 예약·기존 계정 아이디 추가 절차는 사용하지 않는다. `firebase.json`은 Functions 소스 `functions/`와 RTDB 규칙 `database.rules.json`을 가리키며, 함수 리전은 `asia-northeast3`, Node 런타임은 `22`다.

배포 함수는 `checkRegistrationAvailability`, `sendVerificationCode`, `verifyVerificationCode`, `completeRegistration` 네 개다. Unity 기본 URL은 `https://asia-northeast3-PROJECT_ID.cloudfunctions.net`이며 `AccountServerClient`가 `/함수명`을 덧붙인다. `LoginUI.Account Api Base Url`을 비우면 Firebase 앱의 ProjectId로 같은 URL을 만든다.

## 테스트 프로젝트 서버 설정

Secret 값은 문서·Git·로그에 기록하지 않는다.

```powershell
firebase functions:secrets:set RESEND_API_KEY --project YOUR_TEST_PROJECT_ID
firebase functions:secrets:set CODE_HMAC_SECRET --project YOUR_TEST_PROJECT_ID
```

비밀이 아닌 설정은 `functions/.env.YOUR_TEST_PROJECT_ID`에 둔다. 첫 Resend 테스트는 `functions/.env.test.example`을 복사해 아래처럼 사용한다. `onboarding@resend.dev`는 Resend 가입 이메일로만 발송한다.

```dotenv
RESEND_FROM_EMAIL=onboarding@resend.dev
REQUIRE_APP_CHECK=false
```

공개 전에는 검증한 발신 도메인으로 바꾼다. 현재 Unity 클라이언트는 App Check 토큰 헤더를 보내지 않으므로 테스트에서는 `REQUIRE_APP_CHECK=false`를 유지하고, Firebase App Check SDK·공급자와 `X-Firebase-AppCheck` 헤더 전달을 먼저 연결한 뒤에만 `true`로 전환한다. Firebase Authentication에서 이메일/비밀번호 공급자를 활성화하고 비밀번호 정책이 클라이언트·서버 규칙(8~20자, printable ASCII, 영문자·숫자 각 1개 이상)보다 강하지 않은지도 확인한다.

Functions는 프로젝트 ID나 DB URL을 하드코딩하지 않는다. `.firebaserc.example`을 `.firebaserc`로 복사해 테스트 alias만 설정한다. Unity까지 테스트 프로젝트에 연결할 때는 같은 Android package name으로 등록한 테스트 앱의 `Assets/google-services.json`으로 교체한다.

## Join 프리팹 연결

`Assets/Resources/UI/Join.prefab`의 루트 `Join`에는 `JoinUI`와 아래 참조가 연결되어 있어야 한다. Persistent **On Click()은 비워 두며** 스크립트가 리스너를 한 번 등록한다.

> 2026-10-01 정적 검사 기준 저장된 `Join.prefab`에는 아직 `JoinUI`가 없다. Join 루트에 `JoinUI`와 아래 16개 참조가 저장되어 있는지 확인한다. 자식의 `JoinScrollEventTrigger`는 실행 시 자동 구성되므로 프리팹에 수동으로 추가하지 않는다.

| Inspector 필드 | 연결 대상 |
| --- | --- |
| Nickname Input | `2_Info/1_Nickname/InputField/nickname_if` |
| Nickname Error Text | `2_Info/1_Nickname/ErrorText` |
| Nickname Check Button | `2_Info/1_Nickname/CheckBtn` |
| Phone Input | `2_Info/2_Phone/InputField/phone_if` |
| Phone Error Text | `2_Info/2_Phone/ErrorText` |
| Email Input | `2_Info/3_Email/InputField/email_if` |
| Email Error Text | `2_Info/3_Email/ErrorText` |
| Send Code Button | `2_Info/3_Email/2_SendCodeBtn` |
| Verification Code Input | `2_Info/4_EmailCode/CodeInputField/email_if` |
| Verification Error Text | `2_Info/4_EmailCode/ErrorText` |
| Verify Code Button | `2_Info/4_EmailCode/Verify CodeBtn` |
| Password Input | `2_Info/5_PW/InputField/pw_if` |
| Password Error Text | `2_Info/5_PW/PwErrorText` |
| Password Confirm Input | `2_Info/5_PW/CheckInputField/pw_if` |
| Password Confirm Error Text | `2_Info/5_PW/PwCheckErrorText` |
| Join Button | `Content/3_JoinBtn` |

Join 루트의 `JoinUI`에는 `Join Scroll Rect`를 연결한다. 비워 두면 같은 오브젝트의 `ScrollRect`를 자동 사용한다. 실행 시 Content 아래의 입력창과 버튼에 기존 `JoinScrollEventTrigger`를 재사용하거나, 없을 때만 자동 추가하므로 개별 트리거 연결은 필요 없다. Login 프리팹 루트의 `LoginUI`에는 Email Input, Password Input, Join Button, Login Button, Reset Password Button, Status Text를 연결한다.

## 설치·검증·배포 순서

```powershell
pnpm --dir functions install --frozen-lockfile
pnpm --dir functions run build
pnpm --dir functions test
firebase emulators:start --only functions,auth,database --project YOUR_TEST_PROJECT_ID
firebase deploy --only functions --project YOUR_TEST_PROJECT_ID
# 에뮬레이터와 테스트 프로젝트에서 기존 계정 접근을 확인한 뒤에만:
firebase deploy --only database --project YOUR_TEST_PROJECT_ID
```

규칙 적용 전 현재 테스트 규칙을 백업한다. 적용 후 미인증 접근 거부, 본인 기존 `UserInfo/{uid}`만 읽기/수정 가능, 클라이언트의 신규 프로필·닉네임 생성/변경 거부, `serverOnly` 전체 접근 거부를 확인한다. 운영 프로젝트에는 검증 전 규칙을 배포하지 않는다.
