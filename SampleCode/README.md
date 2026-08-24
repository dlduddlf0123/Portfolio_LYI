# Featured Code Index

연도별 프로젝트에 보존된 실제 원본 코드를 현재 기준으로 다시 작성하지 않고 선별했습니다. CP949 원본은 GitHub에서 한글 주석을 읽을 수 있도록 UTF-8로만 변환했으며, 로직·이름·주석 내용은 수정하지 않았습니다.

## XR Interaction

| 샘플 | 확인할 부분 | 실제 프로젝트 원본 |
| --- | --- | --- |
| [TokTokManager.cs](XR/TokTokManager.cs) | OVRHand/OVRSkeleton Bone 거리 판정, 손 입력을 캐릭터 선택·이동으로 연결하는 흐름 | [FingFing](../2024/VRFingFing/Managers/TokTokManager.cs) |
| [CharacterPetting.cs](XR/CharacterPetting.cs) | 손 접촉 위치에 따른 Bone 변형, 변형 거리 제한과 원래 자세 복원 | [VisionPetty](../2024/VisionPetty/Character/CharacterPetting.cs) |

## Gameplay

| 샘플 | 확인할 부분 | 실제 프로젝트 원본 |
| --- | --- | --- |
| [Room.cs](Gameplay/Room.cs) | Room 진입, 적 생성, 클리어 보상, 출구 개방 흐름 | [Burbird](../2023/Burbird/SceneGame/Room/Room.cs) |
| [EnemyRangedAttack.cs](Gameplay/EnemyRangedAttack.cs) | 공통 원거리 공격, 투사체 상태 설정, 오브젝트 풀 연동 | [Burbird](../2023/Burbird/Character/Enemy/Attack/EnemyRangedAttack.cs) |
| [EnemyAttack_SnipeShot.cs](Gameplay/EnemyAttack_SnipeShot.cs) | LineRenderer 조준 예고, 최종 방향 고정, 파생 공격 패턴 | [Burbird](../2023/Burbird/Character/Enemy/Attack/EnemyAttack_SnipeShot.cs) |

## 코드 성격

- 현재 스타일로 재작성한 예제가 아니라 실제 프로젝트에서 사용한 원본 코드의 복사본입니다.
- 웹 표시를 위한 UTF-8 변환 외에는 코드 내용을 수정하지 않았습니다.
- Singleton과 프로젝트 전용 타입 등 당시 구조의 의존성이 포함되어 있습니다.
- 최신 설계의 모범 답안보다는 실제 기능 구현, 문제 해결 방식, 프로젝트 규모와 맥락을 확인하기 위한 자료입니다.
- 프로젝트 설명과 플레이 결과는 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.
