## Portfolio Sample Code

대표 샘플 코드 모음입니다.

### 파일 설명 및 출처

* **BiDirectionalDictionary.cs**
  * **출처:** 2024 / VRFingFing
  * **설명:** 제네릭(`<T1, T2>`)과 인덱서(Indexer)를 활용해 1:1 매칭이 가능한 양방향 딕셔너리를 직접 구현한 커스텀 자료구조 스크립트입니다.
  
* **CharacterAIManager.cs**
  * **출처:** 2024 / VisionPetty
  * **설명:** 코루틴(Coroutine)을 활용하여 가상 펫의 상태(IDLE, WALK, RUN, HIT 등)를 제어하는 유한 상태 머신(FSM) 기반의 AI 컨트롤러입니다.

* **PlayerShooter.cs**
  * **출처:** 2023 / Burbird
  * **설명:** 플레이어 컨트롤러 중 오브젝트 풀링을 활용한 발사 관련 기능 스크립트입니다.

* **TokTokManager.cs**
  * **출처:** 2024 / VRFingFing
  * **설명:** Oculus SDK(`OVRHand`, `OVRSkeleton`)를 활용하여 손가락 뼈(Bone) 간의 거리를 계산해 특정 제스처(Pinch 등)를 인식하고 처리하는 핸드 트래킹 매니저입니다.

* **TouchMoveMesh.cs**
  * **출처:** 2024 / VisionPetty
  * **설명:** 사용자의 터치(Hand) 위치에 따라 캐릭터의 Bone Transform을 수학적으로 계산하고 조작하여, 런타임 중에 Mesh가 눌리거나 변형되는 듯한 인터랙션을 시뮬레이션하는 스크립트입니다.

* **Trap_Shooting.cs**
  * **출처:** 2024 / VRFingFing
  * **설명:** 오브젝트 풀링을 활용한 발사체 관리, `Raycast`와 `LineRenderer`를 이용한 궤적 예측 및 시각화, 코루틴을 이용한 발사 패턴 제어가 구현된 함정 기믹 스크립트입니다.
