# Burbird

모바일 2D 횡스크롤 슈팅 로그라이트 프로젝트입니다. 이 폴더에서는 캐릭터 전투, Room 진행, 반복 테스트를 위한 Practice Mode와 데이터 구조를 확인할 수 있습니다.

## 이 폴더에서 먼저 볼 코드

| 경로 | 확인할 수 있는 구현 |
| --- | --- |
| [SceneGame/Room/Room.cs](SceneGame/Room/Room.cs) | Room 진입, 적 생성, 클리어 보상과 출구 개방까지의 기본 흐름 |
| [Character/Enemy/Attack/](Character/Enemy/Attack/) | 공통 공격 클래스를 기반으로 확장한 적·보스 패턴 |
| [SceneGame/Practice/](SceneGame/Practice/) | 실제 빌드와 에디터에서 적 패턴을 반복 검증하기 위한 연습 기능 |
| [Character/](Character/) | 플레이어·적 캐릭터의 상태와 전투 구현 |
| [Equipment/](Equipment/) | 장비와 속성 효과 관련 구현 |

대표 코드만 빠르게 확인하려면 [Room과 원거리 공격 샘플](../../SampleCode/)을 먼저 보는 것을 권장합니다.

## 프로젝트 내 담당 범위

- Unity 클라이언트 개발 총괄
- 플레이어 조작, 슈팅, 적과 보스 패턴 등 전투 시스템
- 일반 Room, Boss Room, Rest Room과 스테이지 진행 구조
- Practice Mode와 적 패턴 테스트 도구
- 장비·속성 효과·데이터 및 서버 연동

관련 기획 자료는 프로젝트 폴더에 보존되어 있습니다. 프로젝트 배경, 결과와 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.

> 코드와 개발 자료를 선별한 아카이브이므로 이 폴더만으로는 전체 Unity 프로젝트를 실행할 수 없습니다.
