# ⚙️ Action-Driven Entity
전통적인 객체지향 기반 설계 대신, **상호작용을 Action 단위로 분리**하고,  
**Entity는 단순한 Action 컨테이너**로만 동작하는 구조를 채택

## 📌 핵심 개념

### 1. Entity = Action의 집합
- `Entity`는 데이터를 보관하는 ID 기반 컨테이너에 불과
- 모든 행위는 **Action 단위로 설계**, Entity는 Action을 `Attach/Detach`만 실행
- 즉, **Entity는 로직 X**. 로직은 전적으로 Action에만 존재, Entity는 Action을 관리하는 로직만 보유
- 또한 동적인 Action 관리를 통해 조종하는 오브젝트 변경, 새로운 Action 사용 등의 유연한 오브젝트 관리

```csharp
entity.AttachAction(gameContext, new MoveableAction());
entity.AttachAction(gameContext, new BurnableAction());

---

### 2. Action = 실행 단위이자 로직 모듈

- 모든 동작은 `IAction` 인터페이스를 통해 정의
- Action은 프레임마다 `CanExecute()` → `Execute()` 순으로 실행
- 우선순위를 통해 실행 순서를 제어 가능

```csharp
public interface IAction
{
    void Attach(GameContext context, Entity entity, int priority);
    void Detach(GameContext context, Entity entity);
    bool CanExecute(GameContext context, Entity entity, float deltaTime);
    void Execute(GameContext context, Entity entity, float deltaTime);
}
```

#### 예시 Action 목록

- `RotateAction` : 회전 처리
- `MoveControllableAction` : 입력 기반 이동
- `UseAirJumpAction` : 공중 점프 추가 아이템 사용

---

### 3. Stat = 단순한 키-값 데이터 저장소

- `Entity`의 내부 데이터는 `SetStat`, `GetStat`으로 저장되고 조회
- `Action`은 stat을 참조하여 로직을 결정할 수 있지만,  
  Stat 자체는 **로직 미포함**.

```csharp
float hp = entity.GetStat("HP");
entity.SetStat("HP", hp - 10f);
```

---

## 🧩 구조적 특징

| 항목             | 설명                                           |
|------------------|------------------------------------------------|
| ✅ 느슨한 결합    | Action 단위로 설계되어 객체 간 의존도 최소화 |
| ✅ 상태 + 로직 통합 | 상태 변화는 Action 내부에서 처리               |
| ✅ 유연한 조합    | 원하는 Action만 attach하여 동작 정의 가능     |
| ✅ 확장 용이      | 새로운 Action 클래스만 구현하면 기능 추가 가능 |

---

## 📦 데이터 관리 구조

현재 프로젝트는 `SceneData` 및 `EntityData`를 **ScriptableObject** 형태로 사용하여,  
외부에서 `ActionID`, `StatID`와 매핑된 문자열을 등록하고 이를 기반으로 로딩 및 실행

- 각 Entity는 외부 JSON 또는 ScriptableObject 데이터를 통해 생성
- 해당 데이터는 내부적으로 Action 클래스 및 Stat 설정으로 해석되어 적재

---

## 🔄 개선 방향성

### 1. 메시지 큐 기반 상호작용
- `GameContext.messageQueue.Enqueue(...)` 형식의 메시지 시스템의 도입을 고려
- Action은 메시지를 수신하고, 자신이 처리할 수 있는 메시지만 대응
- 이 구조가 완성되면 객체 간 직접 호출 없이 완전한 **익명 메시지 기반 시스템** 완성
- 현재는 Action 내에서 다른 Entity의 Stat에 접근하지만 메시지 큐를 이용하면 Entity간 완벽한 은닉을 보장
- 목적지 Entity에서 해당 메시지를 처리하지 않으면 무시되고, 처리되면 소모

### 2. 데이터 관리 구조 변경 
- ScriptableObject 대신 json으로 분리해 빌드 이후에도 Scene, Entity를 수정 가능하도록 설계

### 3. Action들의 그룹화
- 혹은 디렉토리 분리

### 4. Action <-> Stat 간 연관관계 표현
- 현재 Action들은 Entity 내 Stat이 null 일 시 default 값 상수를 이용
- 연관관계를 표현하는 데이터를 생성하고 해당 파일 기반의 경고 내지 알림 필요

### 5. Action <-> Component 간 연관관계 표현
- Collider나 RigidBody 등 Component를 동적으로 추가하는 Action을 생성하거나
- Action <-> Component 간 연관관계를 표현하는 데이터를 생성하고 해당 데이터 기반의 경고 내지 알림 필요

---

![Demo](https://github.com/qwe1e34r56ty/3DSurvival_ForSubmit/issues/1#issue-3085055769)



