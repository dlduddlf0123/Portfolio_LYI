# Burbird

모바일 2D 횡스크롤 슈팅 로그라이트입니다. 전투와 Room 진행을 중심으로 반복 플레이, 장비, 성장, 연습 기능을 구성했습니다.

## 담당 역할

- Unity 클라이언트 개발 총괄
- 플레이어 조작, 슈팅, 적과 보스 패턴 등 전투 시스템
- 일반 Room, Boss Room, Rest Room과 스테이지 진행 구조
- Practice Mode와 적 패턴 테스트 도구
- 장비·속성 효과·데이터 및 서버 연동

## 코드에서 볼 수 있는 내용

| 경로 | 내용 |
| --- | --- |
| [SceneGame/Room/Room.cs](SceneGame/Room/Room.cs) | Room 진입부터 적 처치, 문 개방까지의 기본 생명주기 |
| [SceneGame/Room/BossRoom.cs](SceneGame/Room/BossRoom.cs) | 보스 전투 규칙을 기본 Room 흐름에 확장한 구현 |
| [Character/Enemy/Attack/](Character/Enemy/Attack/) | 공통 원거리 공격과 패턴별 파생 구현 |
| [SceneGame/Practice/](SceneGame/Practice/) | 적과 보스 패턴을 반복 검증하기 위한 연습 기능 |
| [정리한 Room 샘플](../../SampleCode/Gameplay/RoomLifecycle.cs) | 진행 상태와 외부 의존성을 인터페이스로 분리한 예제 |
| [정리한 원거리 공격 샘플](../../SampleCode/Gameplay/RangedAttackPattern.cs) | 예고·발사·회수 흐름을 분리한 예제 |

기획과 구현 과정은 폴더 안의 개발노트 및 기획 자료에 보존했습니다. 전체 프로젝트 설명과 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 볼 수 있습니다.

> 이 폴더는 공개 가능한 스크립트와 개발 자료를 보존한 코드 아카이브이며, 단독으로 실행 가능한 전체 Unity 프로젝트는 아닙니다.
