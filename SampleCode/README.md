# Featured Sample Code

실제 프로젝트 코드에서 면접 시 설명하기 좋은 책임을 선별하고, 프로젝트 전역 의존성을 줄여 검토하기 쉽게 정리한 코드입니다. 원본 프로젝트의 맥락과 개발 흔적은 각 파일에 연결된 경로에서 함께 확인할 수 있습니다.

## 코드 목록

| 분류 | 코드 | 핵심 내용 | 실무 원본 |
| --- | --- | --- | --- |
| XR | [MetaHandGestureDetector.cs](XR/MetaHandGestureDetector.cs) | 추적 데이터 검증, Bone ID 탐색, 검지 펴짐 판정, 상태 변경 이벤트 | [TokTokManager.cs](../2024/VRFingFing/Managers/TokTokManager.cs) |
| XR | [CharacterPetting.cs](XR/CharacterPetting.cs) | 손 접촉점과 원래 자세 사이의 제한된 Bone 변형 및 복원 | [CharacterPetting.cs](../2024/VisionPetty/Character/CharacterPetting.cs) |
| Gameplay | [RoomLifecycle.cs](Gameplay/RoomLifecycle.cs) | Room 상태 전이, 적 이벤트 구독 해제, 클리어 보상 전달 | [Room.cs](../2023/Burbird/SceneGame/Room/Room.cs) / [BossRoom.cs](../2023/Burbird/SceneGame/Room/BossRoom.cs) |
| Gameplay | [RangedAttackPattern.cs](Gameplay/RangedAttackPattern.cs) | 조준 예고, 방향 고정, 다중 발사, 오브젝트 풀 분리 | [EnemyRangedAttack.cs](../2023/Burbird/Character/Enemy/Attack/EnemyRangedAttack.cs) / [EnemyAttack_SnipeShot.cs](../2023/Burbird/Character/Enemy/Attack/EnemyAttack_SnipeShot.cs) |
| Utility | [BiDirectionalDictionary.cs](Utility/BiDirectionalDictionary.cs) | 양쪽 충돌을 정리해 1:1 매핑 불변식을 유지하는 자료구조 | [프로젝트 원본](../2024/VRFingFing/ETC/BiDirectionalDictionary.cs) |

## 정리 원칙

- 샘플은 전체 프로젝트 코드를 현대식으로 다시 작성한 결과물이 아니라, 실제 구현의 핵심 책임을 작은 단위로 분리한 포트폴리오용 예제입니다.
- 실무 원본과 정리본을 모두 연결해 당시 프로젝트 구조와 현재의 코드 판단을 함께 볼 수 있게 했습니다.
- Unity 또는 Meta XR SDK 타입을 사용하는 예제는 해당 패키지가 있어야 컴파일됩니다. 이 저장소는 전체 Unity 프로젝트가 아닌 코드 아카이브입니다.

## 기존 샘플에서 제외한 코드

- `CharacterAIManager`: 실제 펫 AI 구현 이력은 의미가 있지만, 상태·이동·터치·생활 콘텐츠가 한 Manager에 결합되어 대표 코드로는 범위가 넓습니다. 원본은 [VisionPetty 프로젝트](../2024/VisionPetty/Character/CharacterAIManager.cs)에 보존했습니다.
- `PlayerShooter`: 플레이어 전투 전반과 입력 상태가 결합된 큰 클래스이므로, 더 작은 Room 및 적 공격 구조를 대표 게임플레이 코드로 선택했습니다. 원본은 [Burbird 프로젝트](../2023/Burbird/Character/Player/PlayerShooter.cs)에 보존했습니다.
- `Trap_Shooting`: 퍼즐 기믹의 실제 구현이지만 책임 범위와 프로젝트 의존성이 커서 대표 목록에서는 제외했습니다. 원본은 [FingFing 프로젝트](../2024/VRFingFing/GameScripts/InteractionObjects/Trap/Trap_Shooting.cs)에 보존했습니다.

각 프로젝트의 목적, 담당 역할, 플레이 영상은 [Notion 포트폴리오](https://www.notion.so/Unity-5-26b8567073eb80cf9000d84b0b50f05d)에서 확인할 수 있습니다.
