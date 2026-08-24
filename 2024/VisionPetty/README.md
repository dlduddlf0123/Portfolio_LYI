# VisionPetty (MR Petty)

Apple Vision Pro에서 실제 공간에 캐릭터를 배치하고 손으로 교감하는 공간형 펫 시뮬레이션입니다. 이 폴더에서는 XR Hands·PolySpatial 기반 입력과 접촉형 캐릭터 인터랙션을 확인할 수 있습니다.

## 이 폴더에서 먼저 볼 코드

| 경로 | 확인할 수 있는 구현 |
| --- | --- |
| [Character/CharacterPetting.cs](Character/CharacterPetting.cs) | 손 접촉 위치에 따라 캐릭터 Bone을 변형하고 복원하는 쓰다듬기 구현 |
| [Hand/XRHandGestureInput.cs](Hand/XRHandGestureInput.cs) | 손 제스처 판정 결과를 게임 입력으로 연결하는 흐름 |
| [VisionOS/](VisionOS/) | 공간 메시와 visionOS 전용 기능 연동 |
| [RaceContent/](RaceContent/) | 공간형 펫 게임 안에서 동작하는 미니게임 흐름 |

가장 역할이 명확한 구현부터 확인하려면 [CharacterPetting 대표 샘플](../../SampleCode/XR/CharacterPetting.cs)을 먼저 보는 것을 권장합니다.

## 프로젝트 내 담당 범위

- Unity 클라이언트 개발 총괄
- visionOS, PolySpatial, XR Hands 기반 입력과 공간 배치
- 정적 손 제스처 및 캐릭터 선택·호출 상호작용
- 손 접촉에 반응하는 쓰다듬기와 캐릭터 상태·생활 콘텐츠
- 인벤토리, 먹이, 수면, 미니게임 등 플레이 흐름

프로젝트 배경, 개발 과정과 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.

> 코드와 개발 자료를 선별한 아카이브이므로 이 폴더만으로는 전체 Unity 프로젝트를 실행할 수 없습니다.
