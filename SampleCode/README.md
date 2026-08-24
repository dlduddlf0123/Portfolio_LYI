# Featured Code

전체 프로젝트를 탐색하지 않아도 XR 인터랙션과 일반 게임 클라이언트 구현을 빠르게 확인할 수 있도록 대표 코드 5개를 모았습니다.

## 추천 검토 순서

| 순서 | 코드 | 확인할 부분 | 실제 프로젝트 원본 |
| --- | --- | --- | --- |
| 1 | [CharacterPetting.cs](XR/CharacterPetting.cs) | 손 접촉 위치에 따른 Bone 변형, 거리 제한과 원래 자세 복원 | [VisionPetty](../2024/VisionPetty/Character/CharacterPetting.cs) |
| 2 | [TokTokManager.cs](XR/TokTokManager.cs) | OVRHand/OVRSkeleton Bone 거리 판정과 캐릭터 선택·이동 연결 | [FingFing](../2024/VRFingFing/Managers/TokTokManager.cs) |
| 3 | [Room.cs](Gameplay/Room.cs) | Room 진입, 적 생성, 클리어 보상과 출구 개방 흐름 | [Burbird](../2023/Burbird/SceneGame/Room/Room.cs) |
| 4 | [EnemyRangedAttack.cs](Gameplay/EnemyRangedAttack.cs) | 공통 원거리 공격, 투사체 상태 설정과 오브젝트 풀 연동 | [Burbird](../2023/Burbird/Character/Enemy/Attack/EnemyRangedAttack.cs) |
| 5 | [EnemyAttack_SnipeShot.cs](Gameplay/EnemyAttack_SnipeShot.cs) | LineRenderer 조준 예고와 최종 방향 고정을 추가한 파생 패턴 | [Burbird](../2023/Burbird/Character/Enemy/Attack/EnemyAttack_SnipeShot.cs) |

처음 두 파일에서는 XR 입력과 공간 인터랙션 경험을, 뒤의 세 파일에서는 Room 진행과 상속 기반 전투 패턴 구현을 확인할 수 있습니다. `EnemyRangedAttack`과 `EnemyAttack_SnipeShot`은 기반 클래스와 파생 구현을 함께 보는 것을 권장합니다.

## 읽기 전 참고

- 모두 실제 프로젝트에서 사용한 원본 코드의 복사본입니다.
- 현재 기준으로 재작성하거나 구조를 개선하지 않았습니다.
- CP949로 저장된 파일은 한글 주석을 웹에서 읽을 수 있도록 UTF-8로만 변환했습니다.
- Singleton과 프로젝트 전용 타입 등 당시 프로젝트의 의존성이 포함되어 있습니다.
- 전체 프로젝트의 배경과 결과는 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.
