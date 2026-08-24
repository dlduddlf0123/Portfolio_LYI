# FingFing

Meta Quest와 Steam에 출시한 VR 핸드 트래킹 퍼즐 게임입니다. 플레이어가 컨트롤러 대신 손가락으로 캐릭터를 선택하고 이동시키며 테이블 위 퍼즐을 해결합니다.

## 담당 역할

- Unity 클라이언트 개발 총괄
- OVRHand/OVRSkeleton 기반 손 추적과 캐릭터 선택·이동 인터랙션
- 회전·크랭크·함정 등 퍼즐 기믹과 스테이지 흐름
- 데이터, Addressables, 오브젝트 풀링 등 공통 시스템
- Meta Quest 및 Steam 출시 대응

## 코드에서 볼 수 있는 내용

| 경로 | 내용 |
| --- | --- |
| [Managers/TokTokManager.cs](Managers/TokTokManager.cs) | 손 Bone 거리 판정과 선택·이동 로직이 결합된 실제 프로젝트 매니저 |
| [Table/](Table/) | 회전 테이블, 크랭크 등 손으로 조작하는 퍼즐 장치 |
| [GameScripts/](GameScripts/) | 스테이지에서 사용하는 게임플레이 기믹 |
| [Managers/ObjectPoolingManager.cs](Managers/ObjectPoolingManager.cs) | 반복 생성 오브젝트를 관리하는 공통 풀 |
| [정리한 제스처 샘플](../../SampleCode/XR/MetaHandGestureDetector.cs) | 추적 검증과 제스처 판정 책임만 분리한 포트폴리오용 예제 |

전체 프로젝트 설명과 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 볼 수 있습니다.

> 이 폴더는 공개 가능한 스크립트와 개발 자료를 보존한 코드 아카이브이며, 단독으로 실행 가능한 전체 Unity 프로젝트는 아닙니다.
