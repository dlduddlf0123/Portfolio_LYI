# VisionPetty (MR Petty)

Apple Vision Pro에서 실제 공간에 캐릭터를 배치하고 손으로 교감하는 공간형 펫 시뮬레이션입니다.

## 담당 역할

- Unity 클라이언트 개발 총괄
- visionOS, PolySpatial, XR Hands 기반 입력과 공간 배치
- 정적 손 제스처 및 캐릭터 선택·호출 상호작용
- 손 접촉에 반응하는 쓰다듬기와 캐릭터 상태·생활 콘텐츠
- 인벤토리, 먹이, 수면, 미니게임 등 플레이 흐름

## 코드에서 볼 수 있는 내용

| 경로 | 내용 |
| --- | --- |
| [Hand/XRHandGestureInput.cs](Hand/XRHandGestureInput.cs) | 손 제스처 결과를 게임 입력으로 연결하는 실제 프로젝트 코드 |
| [Character/CharacterPetting.cs](Character/CharacterPetting.cs) | 접촉 위치에 따라 캐릭터 Bone을 변형하는 쓰다듬기 구현 |
| [VisionOS/](VisionOS/) | 공간 메시와 visionOS 전용 기능 연동 |
| [RaceContent/](RaceContent/) | 공간형 펫 게임 안에서 동작하는 미니게임 흐름 |

전체 프로젝트 설명과 개발 과정, 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 볼 수 있습니다.

> 이 폴더는 포트폴리오 검토를 위해 선별한 코드 아카이브이며, 단독으로 실행 가능한 전체 Unity 프로젝트는 아닙니다.
