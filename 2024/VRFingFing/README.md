# FingFing

Meta Quest와 Steam에 출시한 VR 핸드 트래킹 퍼즐 게임입니다. 이 폴더에서는 손가락 입력을 캐릭터 조작으로 연결한 코드와 테이블 위 퍼즐 기믹을 확인할 수 있습니다.

[Steam](https://store.steampowered.com/app/2660300/FingFing/) · [Meta Store](https://www.meta.com/experiences/6526112794179970)

## 이 폴더에서 먼저 볼 코드

| 경로 | 확인할 수 있는 구현 |
| --- | --- |
| [Managers/TokTokManager.cs](Managers/TokTokManager.cs) | OVRHand/OVRSkeleton Bone 거리 판정과 캐릭터 선택·이동 로직 |
| [Table/](Table/) | 회전 테이블, 크랭크 등 손으로 조작하는 퍼즐 장치 |
| [GameScripts/](GameScripts/) | 스테이지에서 사용하는 퍼즐과 게임플레이 기믹 |
| [Managers/ObjectPoolingManager.cs](Managers/ObjectPoolingManager.cs) | 반복 생성 오브젝트를 관리하는 공통 풀 |

핵심 입력 구현만 빠르게 확인하려면 [TokTokManager 대표 샘플](../../SampleCode/XR/TokTokManager.cs)을 먼저 보는 것을 권장합니다.

## 프로젝트 내 담당 범위

- Unity 클라이언트 개발 총괄
- OVRHand/OVRSkeleton 기반 손 추적과 캐릭터 선택·이동 인터랙션
- 회전·크랭크·함정 등 퍼즐 기믹과 스테이지 흐름
- 데이터, Addressables, 오브젝트 풀링 등 공통 시스템
- Meta Quest 및 Steam 출시 대응

프로젝트 배경, 결과와 플레이 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.

> 코드와 개발 자료를 선별한 아카이브이므로 이 폴더만으로는 전체 Unity 프로젝트를 실행할 수 없습니다.
