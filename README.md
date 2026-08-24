# 이영일 | Unity Client Developer

Unity 클라이언트 개발자로 5년 이상 실무 경험이 있으며, 게임플레이 시스템과 XR 인터랙션을 주로 구현했습니다. Meta Quest, Steam, Apple Vision Pro 등 여러 플랫폼에서 실제 출시 과정을 경험했습니다.

프로젝트 배경, 담당 업무, 결과는 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에 정리되어 있습니다. 이 저장소는 당시 작성한 코드와 개발 자료를 확인할 수 있는 보조 자료입니다.

## 핵심 경험

- Unity / C# 기반 게임 클라이언트 개발
- Meta Quest 핸드 트래킹과 Apple Vision Pro 공간 인터랙션
- 모바일 AR, ARKit, Core ML, ManoMotion 연동
- 전투·캐릭터·Room·데이터·오브젝트 풀링 등 게임플레이 시스템
- 기획 협업과 팀 리딩부터 플랫폼 대응, 스토어 출시까지의 개발 과정

## Featured Projects

### [FingFing](2024/VRFingFing/) — Meta Quest / Steam

손가락으로 캐릭터를 조작하는 VR 핸드 트래킹 퍼즐 게임입니다. 개발 총괄로서 손 제스처 입력, 퍼즐 기믹, 캐릭터와 스테이지 시스템, 플랫폼 출시 대응을 담당했습니다.

[Steam](https://store.steampowered.com/app/2660300/FingFing/) · [Meta Store](https://www.meta.com/experiences/6526112794179970)

### [VisionPetty](2024/VisionPetty/) — Apple Vision Pro

실제 공간에서 캐릭터와 교감하는 공간형 펫 시뮬레이션입니다. XR Hands/PolySpatial 기반 입력, 제스처, 쓰다듬기 인터랙션, 캐릭터 상태와 생활 콘텐츠를 구현했습니다.

### [Burbird](2023/Burbird/) — Mobile

2D 횡스크롤 슈팅 로그라이트입니다. 캐릭터 전투, Room 진행 구조, 적 공격 패턴, Practice Mode, 장비·데이터·서버 연동 등 게임 전반을 개발했습니다.

### Mobile AR Hand Tracking — iOS / Android

모바일 AR 환경에서 손으로 캐릭터와 상호작용하는 프로젝트입니다. [초기 Core ML 기반 구현](2020/ARVisionHandTracking/)과 [ManoMotion SDK 적용 버전](2021/ARManoMotionHandTracking/)을 통해 모바일 AR 및 네이티브 연동 경험을 확인할 수 있습니다.

## Featured Code

| 코드 | 보여주는 역량 | 원본 프로젝트 |
| --- | --- | --- |
| [MetaHandGestureDetector](SampleCode/XR/MetaHandGestureDetector.cs) | OVRHand/OVRSkeleton 추적 상태 검증과 Bone 거리 기반 검지 펴짐 판정 | FingFing |
| [CharacterPetting](SampleCode/XR/CharacterPetting.cs) | 접촉 위치를 Bone 변형으로 연결하는 공간 인터랙션 | VisionPetty |
| [RoomLifecycle](SampleCode/Gameplay/RoomLifecycle.cs) | Room 진입·전투·완료·퇴장 흐름과 이벤트 분리 | Burbird |
| [RangedAttackPattern](SampleCode/Gameplay/RangedAttackPattern.cs) | 텔레그래프, 발사 타이밍, 오브젝트 풀을 분리한 적 공격 구조 | Burbird |
| [BiDirectionalDictionary](SampleCode/Utility/BiDirectionalDictionary.cs) | 1:1 매핑 불변식을 보장하는 양방향 자료구조 | FingFing |

코드의 선별 기준과 원본 경로는 [SampleCode 안내](SampleCode/)에서 확인할 수 있습니다.

## Project Archive

[2016](2016/) · [2017](2017/) · [2018](2018/) · [2019](2019/) · [2020](2020/) · [2021](2021/) · [2022](2022/) · [2023](2023/) · [2024](2024/)

오래된 프로젝트는 개발 과정과 기술 변화가 남아 있는 아카이브로 보존했습니다.

## Repository Scope

회사 및 외부 리소스의 권리를 보호하기 위해 프로젝트 전체가 아닌 공개 가능한 스크립트와 일부 개발 자료만 포함합니다. Unity 프로젝트 실행에 필요한 Asset, Package, Scene, `.meta` 파일이 모두 제공되지는 않으므로 이 저장소 자체는 완전한 빌드 프로젝트가 아닙니다.

Contact: [youngil1128@gmail.com](mailto:youngil1128@gmail.com)
