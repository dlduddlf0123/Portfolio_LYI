# 이영일 | Unity Client Developer

Unity/C# 기반 게임 클라이언트와 XR 인터랙션 실무 코드를 정리한 포트폴리오 저장소입니다. Meta Quest·Steam 출시 경험과 Apple Vision Pro·모바일 AR 개발 경험을 코드와 프로젝트 자료로 확인할 수 있습니다.

프로젝트의 배경, 담당 업무, 결과와 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에 정리되어 있습니다. 이 저장소는 그 내용을 뒷받침하는 **코드 및 실제 개발 자료 아카이브**입니다.

## 빠르게 확인하기

| 확인 목적 | 바로가기 | 확인할 수 있는 내용 |
| --- | --- | --- |
| 출시 프로젝트 경험 | [FingFing](2024/VRFingFing/) · [Steam](https://store.steampowered.com/app/2660300/FingFing/) · [Meta Store](https://www.meta.com/experiences/6526112794179970) | VR 핸드 트래킹 게임의 개발 총괄과 플랫폼 출시 대응 |
| XR 인터랙션 구현 | [CharacterPetting](SampleCode/XR/CharacterPetting.cs) · [TokTokManager](SampleCode/XR/TokTokManager.cs) | Vision Pro 접촉 인터랙션과 Meta Quest 손가락 제스처 입력 |
| 일반 게임 클라이언트 구현 | [Room](SampleCode/Gameplay/Room.cs) · [원거리 공격 구조](SampleCode/Gameplay/EnemyRangedAttack.cs) | Room 진행, 전투 흐름, 오브젝트 풀링과 공격 패턴 확장 |
| 프로젝트 전체 맥락 | [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d) | 프로젝트 설명, 역할, 결과와 플레이 영상 |

## Featured Projects

| 프로젝트 | 플랫폼 | 이 저장소에서 먼저 볼 내용 |
| --- | --- | --- |
| [FingFing](2024/VRFingFing/) | Meta Quest / Steam | OVRHand·OVRSkeleton 기반 입력, 퍼즐 기믹, 캐릭터·스테이지 시스템 |
| [VisionPetty](2024/VisionPetty/) | Apple Vision Pro | XR Hands·PolySpatial 기반 입력, 쓰다듬기, 공간형 캐릭터 콘텐츠 |
| [Burbird](2023/Burbird/) | Mobile | 캐릭터 전투, Room 진행, 적 패턴, Practice Mode와 데이터 구조 |
| [Mobile AR Hand Tracking](2020/ARVisionHandTracking/) | iOS | Core ML 기반 손 추적과 Unity·네이티브 연동. [ManoMotion 적용 버전](2021/ARManoMotionHandTracking/)도 함께 보관 |

## Featured Code

| 코드 | 검토 포인트 | 프로젝트 |
| --- | --- | --- |
| [CharacterPetting](SampleCode/XR/CharacterPetting.cs) | 접촉 위치를 Bone 변형으로 연결하고 원래 자세로 복원하는 흐름 | VisionPetty |
| [TokTokManager](SampleCode/XR/TokTokManager.cs) | OVRHand/OVRSkeleton Bone 거리 판정과 캐릭터 선택·이동 인터랙션 | FingFing |
| [Room](SampleCode/Gameplay/Room.cs) | Room 진입·전투·완료·퇴장 흐름 | Burbird |
| [EnemyRangedAttack](SampleCode/Gameplay/EnemyRangedAttack.cs) | 공통 원거리 공격과 오브젝트 풀 연동 | Burbird |
| [EnemyAttack_SnipeShot](SampleCode/Gameplay/EnemyAttack_SnipeShot.cs) | 조준 예고와 방향 고정을 사용하는 파생 공격 패턴 | Burbird |

대표 코드는 새로 작성하거나 리팩터링한 예제가 아니라 실제 프로젝트 원본의 복사본입니다. 웹에서 읽을 수 있도록 일부 파일의 문자 인코딩만 UTF-8로 변환했습니다. 추천 검토 순서와 원본 경로는 [Featured Code 안내](SampleCode/)에서 확인할 수 있습니다.

## Project Archive

[2016](2016/) · [2017](2017/) · [2018](2018/) · [2019](2019/) · [2020](2020/) · [2021](2021/) · [2022](2022/) · [2023](2023/) · [2024](2024/)

연도별 폴더에는 대표 프로젝트 외의 과거 작업도 개발 이력 아카이브로 보존되어 있습니다.

## Repository Scope

포트폴리오 검토에 필요한 스크립트와 개발 자료를 선별한 저장소입니다. Asset, Package, Scene, `.meta` 파일이 모두 포함된 완전한 Unity 프로젝트가 아니므로 저장소만으로는 실행되지 않을 수 있습니다.
