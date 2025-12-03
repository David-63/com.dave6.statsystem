# StatSystem

캐릭터가 사용할 변수들을 스텟 단위로 묶어서 대신 관리해주는 패키지

## 뭘 할수있지?

- 채력, 마나, 스테미너와 같이 소모되는 자원은 Attributes로 등록해서 관리
- 힘 민 지 같은 스텟포인트 종류는 PrimaryStats으로 등록해서 관리
- 엑셀이나 코드 없이 에디터 안에서 수식 만들기 (노드로 끌어다 붙이기만 하면 됨)

## 사용방법

1. 우클릭 → Create → DaveAssets → Stat Database 만들기
2. 안에 힘, 민첩, 체력 등 원하는 스탯 추가
3. 캐릭터에 StatController 컴포넌트 붙이고 → 만든 데이터베이스 연결
4. (원하면) 노드 그래프로 "PhysicalDamage = Strength × 2 + 10" 같은 계산식 만들기
