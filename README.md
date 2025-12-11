# CustomModule

**프로젝트 개요**

- **CustomModule**은 Unity 프로젝트로, OpenCVForUnity, FFmpegOut, TextMesh Pro, UniRx, UniTask, Google.Protobuf, Newtonsoft.Json 등 여러 써드파티 라이브러리와 플러그인을 통합해 다양한 유틸리티(웹캠 재생, 이미지/비디오 녹화, 그래프 표시, 파일 입출력 등)를 제공하는 샘플/모듈 모음입니다.

**주요 특징**

- OpenCV 기반 이미지/비디오 처리(StreamingAssets에 OpenCV 리소스 포함)
- FFmpeg 기반 녹화/출력(FFmpegOut 플러그인 포함)
- TextMesh Pro 예제 및 UI 텍스트 관련 스크립트
- UniRx / UniTask 비동기·리액티브 유틸리티
- Protobuf / Newtonsoft 기반 직렬화 유틸리티

**권장 요구사항**

- Unity Editor 버전: 프로젝트 파일에 기록된 `UnityVersion: 6000.2.7f2` (프로젝트와 동일하거나 호환되는 Unity 버전 사용 권장). Editor 경로는 환경에 따라 다릅니다.
- 운영체제: Windows / macOS / Linux (빌드 대상에 따라 상이)
- 의존 패키지: OpenCVForUnity, FFmpegOut, TextMesh Pro, UniRx, UniTask, Google.Protobuf, Newtonsoft.Json

**프로젝트 구조(요약)**

- `Assets/` : Unity 에셋(스크립트, 씬, 플러그인, 3rd party 소스 등)
  - `Assets/Scenes/CustomModule.unity` : 메인 예제 씬
  - `Assets/Scripts/ToolMenu.cs` 등 유틸 스크립트
  - `Assets/StreamingAssets/` : OpenCVForUnity 등 런타임 리소스
- `Packages/manifest.json` : Unity 패키지 의존성 (`com.cysharp.unitask`, `com.neuecc.unirx` 등)
- `.sln` / `.csproj` : Visual Studio용 솔루션/프로젝트 파일 (참조 확인용)

**설치 및 시작 가이드**

1. Unity 설치
   - Unity Hub에서 프로젝트에 맞는 Editor(예: `6000.2.7f2`)를 설치하세요.
2. 프로젝트 열기
   - Unity Hub에서 `Add` > 해당 폴더(`.../CustomModule`)를 선택하거나 Unity Editor에서 `Open Project`로 열기.
3. 패키지 복원
   - `Packages/manifest.json`에 정의된 패키지는 Unity가 자동으로 복원합니다. (`OpenUPM` scoped registry에 `com.cysharp.unitask`, `com.neuecc.unirx` 등 설정됨)
4. 서드파티 애셋
   - OpenCVForUnity, FFmpegOut 같은 에셋은 프로젝트에 이미 포함된 리소스(`Assets/StreamingAssets` 등)가 있지만, 에셋 패키지가 없는 경우 Unity Asset Store 또는 공급처에서 설치해야 합니다(상업용 라이선스 필요할 수 있음).

**로컬에서 실행 / 예제 씬**

- Unity에서 `Assets/Scenes/CustomModule.unity` 씬을 열고 Play 버튼으로 예제 동작을 확인하세요.
- 주요 샘플 스크립트(예):
  - `Assets/Projects/WebcamPlayer/WebcamPlayer.cs` — 웹캠 재생 관련
  - `Assets/Projects/OpenCVRecorder/OpenCVRecorder.cs` — OpenCV 기반 녹화
  - `Assets/Projects/FFmpegRecorder/FFmpegRecorder.cs` — FFmpegOut 녹화 인터페이스

**커맨드라인 빌드 예시**

Unity Editor 명령행 빌드 예시(경로는 환경에 맞게 수정):

```bash
/path/to/Unity -batchmode -quit -projectPath "/mnt/c/Users/allen/wkspaces/CustomModule" -buildWindows64Player "Builds/CustomModule.exe" -logFile build.log
```

**개발/디버깅 팁**

- Visual Studio/JetBrains Rider에서 `.sln` 파일을 열면 스크립트 편집 및 디버깅이 수월합니다.
- 패키지 문제 발생 시 `Library/` 폴더를 삭제하고 Unity를 재시작하면 패키지/임시 파일이 재생성됩니다(주의: 재임포트 시간이 소요됩니다).

**기여 방법**

- 이 저장소는 주로 Unity 프로젝트 파일을 포함하므로, Git 사용 시 대용량 바이너리(라이브러리 DLL, 에셋 등)에 주의하세요.
- 작은 스크립트 변경이나 문서 기여는 PR로 요청해 주세요. 큰 에셋 추가는 사전에 이슈로 상의 바랍니다.

**라이선스·저작권**

- 프로젝트에는 여러 서드파티 라이브러리(예: OpenCVForUnity, FFmpegOut, TextMesh Pro 등)가 포함되어 있습니다. 각 라이브러리의 라이선스를 확인하세요. 저장소 자체의 라이선스 정보가 필요하면 알려주시면 README에 추가하겠습니다.

---

원하시면 영어버전 README나, CI(예: GitHub Actions) 자동 빌드 스크립트, 또는 `CONTRIBUTING.md` 초안을 이어서 만들어 드릴게요. 어떤 추가 정보를 넣을까요?

# CustomModule
