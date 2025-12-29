# StatSystem

캐릭터가 사용하는 모든 수치를 Stat 단위로 통합 관리하기 위한 시스템.
스탯 계산, 파생 스탯, 효과 적용을 태그 기반 구조로 처리하며
캐릭터 로직과 수치 계산을 분리하는 것을 목표로 한다.


## 핵심 개념
1. **StatTag** 기반 식별
- 모든 스탯은 StatTag로 식별됨
- 문자열 이름이나 네이밍 규칙에 의존하지 않음
- 스탯 참조, 계산, Effect 소스 지정 모두 StatTag 기준으로 처리

2. **Definition** 기반 데이터 설계
- 모든 스탯은 StatDefinition으로 정의
- 캐릭터는 StatDatabase를 통해 스탯을 구성
- 계산 방식, 파생 관계, 효과 소스는 Definition 단에서 설정

3. **BaseStat** / **Derived** 구조
스탯은 역할에 따라 다음과 같이 구분됨.

`Attribute`
-  기본 수치 스탯
-  외부 기여값(BaseContribution)을 통해 값 변경

`SecondaryStat`
-  다른 스탯을 참조하여 계산되는 파생 스탯
-  StatReference (BaseStat + weight) 조합으로 계산

`ResourceStat`
-  Current 값을 가지는 자원형 스탯 (HP, MP 등)
-  최대값 변경 시 비율 보정 및 Clamp 처리 포함


## 제공하는 기능

### 스탯 등록 및 관리
- `StatDatabase` 기반으로 캐릭터 별 스탯 구성
- `StatHandler` 를 통해 스탯 조회, 갱신, Effect 적용 관리

### 스탯 계산
- Base 값 -> 기여값(`BaseContribution`) -> 최종 계산
- Dirty flag 기반 계산으로 연산 최소화
- 계산 단계와 조회 단계 분리

### BaseContribution 시스템
모든 스탯 변화는 `BaseContribution`으로 통합 처리
특징:
- 영구 / 일시 / 버프 디버프 모두 동일한 구조
- source 기반으로 제거
- 범용적으로 사용 가능한 스탯 변경 단위

### Effect 시스템
- `EffectDefinition` 을 통해 효과의 계산식과 적용 방식을 정의
- Effect는 `StatReference` 기반 계산 또는 **Flat** 값 기반으로 구성 가능
지원 방식:
- Instant: 즉시 수치 적용
- Sustained: 지속 효과 (BaseContribution 추가/제거)
- Periodic: 지속적인 Instant 효과

## 사용방법

### StatHandler 구성
캐릭터는 `IStatController` 인터페이스를 통해 `StatHandler`를 보유.
- **StatHandler** 역할
-  스탯 초기화
-  스탯 조회
-  Effect 생성 및 적용
-  Effect 라이프사이클 관리
패키지 내 제공되는 `PlayerController` 예제를 참고하여 캐릭터에 적용.

### 스탯 정의와 스탯 데이터베이스 구성
1. 필요한 "StatDefinition" 생성
2. 각 스탯을 정의하는 StatTag (1:1 대응) 생성
3. `StatDatabase` 에서 태그와 함께 스탯 타입(Base / Secondary / Resource) 설정
4. 필요 시 SecondaryStat의 소스 스탯 지정


### Effect 정의 및 적용
**EffectDefinition**에서 다음을 설정

- 적용 방식 (Instant / Sustained / Periodic)
- 계산 소스 (StatTag 기반)
- Flat 값 여부
- StatHandler.CreateEffectInstance()를 통해 적용

패키지 내 `IStatInvoker` 예제 코드 참고 권장.