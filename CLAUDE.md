# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Abyss Slayer** — 픽셀 아트 스타일의 2D 횡스크롤 액션 RPG. 최대 5명 실시간 협동 보스 레이드 게임.
- **엔진**: Unity 6000.2.7f2 (Unity 6)
- **네트워크**: Photon Fusion 2 (Host-Client 구조)
- **플랫폼**: PC

## Build & Development

- Unity 에디터에서 직접 빌드/실행 (별도 CLI 빌드 스크립트 없음)
- 솔루션 파일: `Abyss_Slayer.sln`
- CI/CD 파이프라인 없음
- 조건부 컴파일 심볼: `AllMethodDebug`, `MoveSceneDebug` (디버그 로그 제어)

## Architecture

### Manager Hub (중앙 매니저 시스템)
모든 매니저는 `ManagerHub.Instance`를 통해 접근. `Singleton<T>` 기반 DontDestroyOnLoad.

```
ManagerHub.Instance.{
    GameFlowManager   // 게임 상태 머신 (씬 전환)
    GameValueManager  // 게임 상수/설정값 (SerializeField)
    UIManager         // UI 팝업 스택, ESC 처리
    DataManager       // Addressables 비동기 데이터 로딩
    SoundManager      // BGM/SFX, AudioMixer (SerializeField)
    ServerManager     // Photon Fusion 네트워크
    AnalyticsManager  // Unity Analytics 퍼널/이벤트
    UIConnectManager  // UI-게임 연결
}
```

### Game Flow (상태 머신 기반 씬 전환)
`ESceneName` enum으로 관리: StartScene → IntroScene → InputNameScene → LobbyScene → RestScene → BattleScene → EndingScene
- 클라이언트 씬 전환: `ClientSceneLoad()` (Start, Intro, Lobby)
- 서버 씬 전환: `RpcServerSceneLoad()` (Rest, Battle)
- 모든 전환은 LoadingState를 경유

### Player 시스템
- `Player : NetworkBehaviour, IHasHealth` — Photon Fusion의 NetworkBehaviour 상속
- `PlayerStateMachine` — 상태: Idle, Walk, Jump, Fall, Dead + 스킬 상태
- `[Networked]` 속성으로 위치/상태 동기화, RPC로 데미지/애니메이션 전파
- `ReactiveProperty<T>` (UniRx)로 HP, 스탯 등 UI 자동 바인딩
- 4개 클래스: `CharacterClass { Rogue, Healer, Mage, MagicalBlader, Tanker }`

### Boss 시스템
- `Boss : NetworkBehaviour` — 서버 권한 제어
- `BossController` — 페이즈 시스템 (50% HP에서 Phase 2), 가중치 기반 패턴 랜덤 선택
- 패턴: `BasePatternData` (ScriptableObject) 상속, 쿨타임/체력조건/가중치 기반
- 코루틴으로 패턴 연출 처리

### Skill 시스템
ScriptableObject 상속 구조:
```
Skill (base)
├── AttackSkill → MeleeAttackSkill, AreaSkill, ProjectileAttackSkill
├── BuffSkill
├── DashSkill
└── MovingSkill
```
클래스별 스킬 데이터: `Assets/02. Scripts/Skill/SkillData/{Mage,MagicalBlader,Rogue,Tanker}/`

### 네트워크 패턴
- Host가 Boss/게임 상태에 대한 State Authority 보유
- `[Networked]` 프로퍼티: 위치, HP, 상태 인덱스 등 자동 동기화
- `Rpc_` 접두사 메서드: 데미지, 애니메이션, 이펙트 등 이벤트성 동기화
- 씬 전환 시 `NetworkRunner` 유지

### 주요 라이브러리
- **Photon Fusion 2**: 멀티플레이 네트워크
- **UniRx**: ReactiveProperty 기반 데이터 바인딩
- **Addressables**: 비동기 에셋 로딩/캐싱
- **DOTween**: 애니메이션/트윈
- **Cinemachine**: 카메라 시스템

## Code Conventions

- **커밋 메시지**: `[Tag] 설명` 형식 — Tag: `Fix`, `Feat`, `Refactor`, `Chore`
- **코멘트/주석**: 한국어
- **프로퍼티 직렬화**: `[field: SerializeField] public Type Name { get; private set; }`
- **네트워크 프로퍼티**: `[Networked] public Type Name { get; set; }`
- **EditorConfig**: UTF-8 charset, CRLF line endings

## Directory Structure (Scripts)

```
Assets/02. Scripts/
├── Manager/        # ManagerHub, Singleton, 각종 매니저 (16)
├── Player/         # Player, StateMachine, Animation (18)
├── Boss/           # Boss, BossController, PatternData (35)
├── Skill/          # 스킬 베이스, 클래스별 스킬 데이터 (31)
├── UI/             # UIBase, LobbyUI, Slot 등 (76+)
├── Projectile/     # Boss/Player 투사체 (6)
├── Effect/         # 시각 이펙트 (11)
├── Interface/      # IHasHealth, ICoolDownSkill, IMVRP (3)
├── Pool/           # 오브젝트 풀링 (6)
├── Analytics/      # 분석 매니저/이벤트 (5)
├── Util/           # 유틸리티, 확장 메서드 (12)
└── Others/         # 기타 유틸리티 (12)
```
