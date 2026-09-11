### ⚠️ IMPORTANT NOTICE / DISCLAIMER

**Original Author:** IcyClawz
**Original Repository:** ClientMods
**Original Link:** https://github.com/IcyClawz/ClientMods
**License:** See upstream repository
**This Port By:** R_F (danyhappy564-cmyk) — unofficial, AI-assisted port. Not affiliated with or endorsed by the original author.

1. **Reflection & Take-Downs:** I deeply reflect on the ECOT incident. As an AI-assisted "vibe coder," I will immediately delete files if the original authors ask.
2. **No Re-Distribution:** These ported builds are unverified, temporary fixes. Please do NOT re-upload or share them anywhere else.
3. **Do Not Pester Original Authors:** Never report bugs or pester original modders regarding issues from my unofficial ports.
4. **Full Credit & Respect:** I will always credit original creators on GitHub and prioritize their decisions above all else.
5. **Support Original Creators:** Instead of using my ports, please visit the original authors' Forge pages to leave kind words or tips.

---

# IcyClawz ClientMods (fork)

> **원작자 · 원본 레포**
> **IcyClawz** — https://github.com/IcyClawz/ClientMods
>
> 이 레포는 위 원작의 **포크**입니다. 기능은 그대로고, **SPT 4.1에서 빌드·동작하도록
> 포팅**한 것이 전부입니다. 포크 시점은 `2b9ed9f` (EFT 0.16.9.0.40087, SPT 4.0).
>
> 여기에 플리마켓 탄약 오버레이 기능 하나를 추가했습니다 (아래 "추가한 기능").

현재 기준 **SPT 4.1**.

이 레포는 모드 **하나가 아니라 6개**입니다 (원작이 자기 클라이언트 모드를 한 솔루션에
모아둔 모음집). 서로 독립이라 원하는 것만 골라 빌드하면 됩니다.

| 프로젝트 | 하는 일 | 의존 |
|---|---|---|
| `MunitionsExpert` | 탄약에 관통/장갑피해/파편화/도탄/내구소모/발열/불발 항목을 추가하고, 아이콘 배경을 관통 레벨 색으로 칠하고, 플리 매물에 `[관통력/데미지]` 를 겹쳐 표시 | 없음 |
| `ItemAttributeFix` | 압축 표시된 속성 툴팁이 잘린 값을 보여주던 문제 수정 | 없음 |
| `MagazineInspector` | 탄창에 들어 있는 탄약 수를 스킬 수준에 맞춰 표시 | 없음 |
| `ItemSellPrice` | 아이템 정보창에 상인 판매가 표시 | 없음 |
| `CustomInteractions` | 다른 모드가 컨텍스트 메뉴에 항목을 붙일 수 있게 해주는 API (그 자체로는 기능 없음) | 없음 |
| `ItemContextMenuExt` | 우클릭 메뉴에 발사모드 / 조준경 배율·영점 / 택티컬 온오프 추가 | `CustomInteractions` |

`MunitionsExpert` 만 쓸 거면 그 프로젝트만 빌드하면 됩니다 (아래 "빌드" 참고).
`ItemAttributeFix` 는 같이 넣는 걸 권합니다 — `MunitionsExpert` 가 추가하는 항목이 전부
압축 표시 타입이고, 바닐라는 그 툴팁 값을 잘라서 보여줍니다.

---

## 추가한 기능 — 플리마켓 탄약 [관통력/데미지]

플리마켓 매물 아이콘 위에 `[관통력/데미지]` 를 겹쳐서 표시합니다. 탄약 낱개는 물론
**탄약 팩(AmmoBox)** 도 안에 든 탄을 읽어서 같이 표시합니다.

붙은 곳은 `EFT.UI.DragAndDrop.RagfairOfferItemView.UpdateInfo()` 입니다. 이 메서드는
원래 그리드 아이템에서 이름표로 쓰는 `GridItemView.Caption` 을 매물 아이콘에서는
그냥 꺼버립니다 — 즉 매물마다 **안 쓰고 놀고 있는 텍스트 요소**가 하나씩 있습니다.
그걸 빌려 씁니다.

이 구조라서 생기는 이점이 두 개 있습니다.

- 새 GameObject 를 안 만듭니다. 프리팹 레이아웃을 건드리지 않습니다
- `UpdateInfo` 가 항상 먼저 캡션을 꺼주므로, **오브젝트 풀에서 재활용된 뷰**가
  이전 탄약의 오버레이를 달고 나오는 일이 없습니다 (탄약이 아니면 우리가 다시
  켜지 않으니 꺼진 상태 그대로)

설정 (BepInEx Configuration Manager → `com.IcyClawz.MunitionsExpert`):

| 항목 | 기본값 | 설명 |
|---|---|---|
| `Flea Market Ammo Stats` → `Show penetration/damage` | `true` | 끄면 오버레이 자체를 안 그림 |
| `Flea Market Ammo Stats` → `Text color` | `#b6c1c7` | 게임이 자기 라벨에 쓰는 것과 같은 형식의 hex |

### 알려진 상호작용

`UIFixes` 의 `BarterOfferPatches.ItemUpdateInfoPatch` 도 같은 `UpdateInfo` 에 postfix 로
붙어 있습니다 (실기 로그에서 확인). 두 postfix 는 서로를 막지 않고 둘 다 실행되지만,
UIFixes 가 물물교환 매물에서 캡션을 쓴다면 **탄약 물물교환 매물**에서는 우리 쪽 표시가
덮어쓸 수 있습니다. 그런 경우엔 위 설정을 끄면 됩니다.

---

## 4.1 포팅에서 바뀐 것

4.1은 클라이언트를 **역난독화**해서 배포합니다. 타입은 위키에 4.0→4.1 대응표가 있지만
**멤버(필드/메서드) 이름은 표가 없습니다** — SPT 어셈블리 툴이 빌드 시점에 시그니처를
맞춰가며 이름을 되살리는 방식이라, 어떤 멤버는 바뀌고 어떤 멤버는 안 바뀝니다.

### 1. 타입 이름 20개

소스에 등장하는 식별자 573개를 위키 대응표에 전수 대조했고, 걸린 건 아래 20개입니다.
(나머지는 원래부터 실명이라 표에 없음 = 그대로)

| 4.0 | 4.1 |
|---|---|
| `AmmoItemClass` | `EFT.InventoryLogic.Ammo` |
| `BackendConfigSettingsClass` | `GlobalConfiguration` |
| `CacheResourcesPopAbstractClass` | `ResourcesCache` |
| `DynamicInteractionClass` | `EFT.UI.DynamicContextInteraction` |
| `FirearmLightStateStruct` | `LightsState` |
| `FirearmScopeStateStruct` | `ScopeState` |
| `GClass2340` | `InGameStatus` |
| `GClass3130` | `CurrencyUtil` |
| `GClass3752` | `EFT.UI.BaseContextInteractions` |
| `GClass3775` | `EFT.UI.BaseEmptyContextInteractions` |
| `GInterface396` | `ILightComponentTemplate` |
| `GInterface404` | `ISightComponentTemplate` |
| `ISession` | `IEftSession` |
| `ItemAttributeClass` | `ItemAttribute` |
| `ItemContextClass` | `DragItemContext` |
| `ItemInfoInteractionsAbstractClass<T>` | `EFT.UI.ContextInteractions<T>` |
| `MagazineItemClass` | `EFT.InventoryLogic.Magazine` |
| `SharedGameSettingsClass` | `SettingsManager` |
| `ThrowWeapItemClass` | `ThrowWeap` |
| `TraderClass` | `EFT.Trading.Trader` |

### 2. 멤버 이름 — 실제 4.1 어셈블리로 전수 확인

멤버는 대응표가 없어서, **실제 SPT 4.1 `Assembly-CSharp.dll`** 을 직접 열어 하나씩
맞췄습니다. 이름만 본 게 아니라 **접근 지정자와 오버로드 개수까지** 확인했고, 마지막엔
`MetadataLoadContext` 로 이 모드가 하는 리플렉션 조회를 **그대로 재현해서** 전부
`null` 아닌 결과가 나오는지 실행해 봤습니다.

| 4.0 | 4.1 |
|---|---|
| `AmmoTemplate.CachedQualities` | `_cachedQualities` |
| `InteractionButtonsContainer.method_1` | `CreateContextButton` |
| `InteractionButtonsContainer.method_3` | `CreateDynamicContextButton` |
| `InteractionButtonsContainer.method_4` | `CloseSubMenu` |
| `InteractionButtonsContainer.method_5` | `BindButton` |
| `InteractionButtonsContainer.simpleContextMenuButton_0` | `_subMenuButton` |
| `CompactCharacteristicPanel.string_0` | `_dataForTooltip` |
| `ItemInfoInteractionsAbstractClass<T>.Dictionary_0` | `ContextInteractions<T>._dynamicInteractions` |
| `TraderClass.SupplyData_0` | `Trader._supplyData` |
| `DynamicInteractionClass.Action_0` | `DynamicContextInteraction._callback` |

### 3. 이름만 고쳐서는 안 되는 것 두 가지

**(a) 접근 지정자가 바뀐 필드 4개.** 4.1은 `[SerializeField] private` 이던 필드
여럿을 **public** 으로 내보냅니다. 원작은 이것들을 `BindingFlags.NonPublic` 으로만
찾고 있었고, 그러면 **조용히 `null` 이 돌아옵니다.**

| 필드 | 4.1 접근 지정자 |
|---|---|
| `GridItemView.Caption` | `public` |
| `EntityIcon._colorPanel` | `public` |
| `InteractionButtonsContainer._buttonsContainer` | `public` |
| `InteractionButtonsContainer._buttonTemplate` | `public` |

이제 필드 조회는 전부 `Public | NonPublic | Instance` 로 통일했습니다.

**(b) 역난독화가 만들어낸 오버로드 충돌.** 4.0에서 `method_0<T>` 와 `method_1` 이라는
**서로 다른 이름**이던 두 메서드가 4.1에서는 **둘 다 `CreateContextButton`** 이
됐습니다. 이름만 주고 `GetMethod("CreateContextButton", ...)` 를 부르면
`AmbiguousMatchException` 이 나면서 `InteractionButtonsContainerExtensions` 정적
생성자가 통째로 터집니다 — 컨텍스트 메뉴 확장이 전부 죽습니다. 인자 타입 9개를
명시해서 원하는 오버로드를 특정하도록 고쳤습니다.

이 둘은 **위키 대응표만 보고 타입 이름만 갈아끼웠으면 절대 안 걸렸을** 문제입니다.

### 4. 프리패처를 들어냈습니다

`CustomInteractions.Prepatch` 는 BepInEx 프리로더 단계에서 `Assembly-CSharp.dll` 을
직접 고치던 패처였습니다. 하는 일은 두 가지:

1. `DynamicInteractionClass.Action_0` 을 `protected` + 비-readonly 로 열어서, 서브클래스가
   직접 대입할 수 있게 함
2. 생성자의 `callback` 인자를 optional 로 만들어서 `base(id, id)` 2인자 호출이 되게 함

4.1에서는 **이대로 두면 로딩 자체가 터집니다.** `assembly.MainModule.GetType("DynamicInteractionClass")`
가 `null` 을 돌려주기 때문입니다 — 4.1에서 이 타입은 전역이 아니라 `EFT.UI` 안에 있고,
Cecil 의 `GetType` 은 네임스페이스를 포함한 전체 이름을 요구합니다.

고쳐서 살릴 수도 있었지만, **4.1에서는 두 조작 다 필요가 없습니다.** 실제 어셈블리를
열어보니 `DynamicContextInteraction._callback` 은 이미 `public` 이고 readonly 도 아닙니다
(1번 불필요). 생성자를 `base(id, id, null)` 로 명시하면 2번도 불필요합니다. 그래서
프로젝트를 삭제했습니다. 남겨두면 오히려 손해입니다 — 이미 `public` 인 게임 필드를
`protected` 로 **좁히는** 조작이라, 같은 필드를 IL 로 직접 읽는 다른 모드를 깨뜨릴 수
있습니다.

> **업그레이드하는 경우**: `BepInEx\patchers\` 에 남아 있는 예전
> `IcyClawz.CustomInteractions.Prepatch.dll` 을 **지우세요.** 그게 남아 있으면 위에서
> 말한 `null` 때문에 프리로더 단계에서 죽습니다.

덤으로, 이것 때문에 `CustomInteractions` 가 참조하던
`Shared\Assembly-CSharp-CustomInteractions.dll` (프리패치가 적용된 게임 어셈블리 사본)
도 필요 없어졌습니다. 이제 설치본의 `Assembly-CSharp.dll` 을 그냥 참조합니다.

---

## 5. 아이콘을 resx 에서 꺼냈습니다 (검사창이 깨지던 원인)

증상: 탄약을 검사하면 창 제목이 `EXAMINE : {0}` 인 채로 뜨고, 이름 자리에 `Blablabla`
가 박히고, 속성 값이 비고, **닫기 버튼이 안 먹습니다.**

원인은 아이콘 3장(`ArmorDamage` / `FragmentationChance` / `RicochetChance`)이 실려 있던
방식입니다.

1. 원작은 이 PNG 들을 `Resources.resx` 에 `System.Drawing.Bitmap` 으로 넣어뒀습니다
2. 요즘 .NET SDK 는 BinaryFormatter 기반 리소스 쓰기를 빼버려서, 그대로 빌드하면
   `MSB3822`/`MSB3823` 으로 실패합니다
3. 그래서 `GenerateResourceUsePreserializedResources` 를 켰는데 — **이게 함정입니다.**
   이 옵션은 `.resources` 헤더에 리더/셋 타입으로
   `System.Resources.Extensions.DeserializingResourceReader` 를 박아넣고, 런타임에 그
   어셈블리를 요구합니다. **BepInEx 는 `System.Resources.Extensions.dll` 을 안 실어줍니다**
4. → `Properties.Resources.ArmorDamage` 가 던짐 → `IconCache` 정적 생성자가
   `TypeInitializationException` → 이 캐시를 읽는
   `StaticIcons.GetAttributeIcon` **프리픽스가 호출될 때마다 던짐** → 게임이 검사창을
   만들다 말고 죽음 → 제목이 포맷 문자열 그대로 남고 닫기 핸들러가 안 붙음

고친 방식: resx / `System.Drawing` 을 통째로 버리고 **PNG 바이트를 그대로 임베드**해서
`Texture2D.LoadImage` 로 읽습니다. 두 의존이 다 사라지고, 덕분에 이 프로젝트도
`netstandard2.1` 로 넘어갔습니다.

거기에 더해 `IconCache` 는 **이제 절대 예외를 밖으로 안 냅니다.** 아이콘 로딩이 실패하면
로그만 남기고 `null` 을 돌려주고, 그러면 게임이 자기 아이콘 조회로 넘어갑니다 (그쪽은
못 찾아도 로그만 찍습니다). Harmony 프리픽스 안에서 던지면 UI 가 통째로 깨진다는 걸
이번에 확인했으니, 같은 실수가 다시 나와도 검사창은 살아 있습니다.

빌드 산출물로 검증한 것:

```
target: .NETStandard,Version=v2.1
  resource: IcyClawz.MunitionsExpert.Resources.ArmorDamage.png
  resource: IcyClawz.MunitionsExpert.Resources.FragmentationChance.png
  resource: IcyClawz.MunitionsExpert.Resources.RicochetChance.png
ok ...ArmorDamage.png         649 bytes  pngSignature=True  identicalToSourceFile=True
ok ...FragmentationChance.png  733 bytes  pngSignature=True  identicalToSourceFile=True
ok ...RicochetChance.png       681 bytes  pngSignature=True  identicalToSourceFile=True
```

DLL 안에 `System.Resources.*` / `System.Drawing*` 문자열은 이제 하나도 없습니다.

---

## 빌드 설정도 갈아엎었습니다

- **모든 프로젝트가 `..\Shared\*.dll` 을 참조하고 있었습니다** — 레포에 없고 손으로
  채워야 하는 폴더라, 새로 클론하면 참조가 전부 한꺼번에 깨지고 뭐가 없는지도 안
  알려줍니다. `Directory.Build.props` 에서 `SptRoot` 로 설치본을 직접 가리키게 바꿨습니다
  (기본값 `E:\SPT 4.1`, `-p:SptRoot=...` 또는 환경변수로 덮어쓰기)
- `SptRoot` 가 SPT 설치본이 아니면 "타입을 찾을 수 없음" 수십 줄 대신 **이유를 말하는
  에러 하나**로 실패합니다
- `net472` → `netstandard2.1` (SPT 4.1 클라 플러그인 기준). 6개 전부. `MunitionsExpert`
  를 붙잡고 있던 `System.Drawing` 은 아래 아이콘 처리를 바꾸면서 없어졌습니다
- `ItemContextMenuExt` 가 `IcyClawz.CustomInteractions.dll` 을 파일로 참조하고 있었는데
  같은 솔루션 안에 있는 프로젝트라 `ProjectReference` 로 교체
- `MunitionsExpert` 에 `Unity.TextMeshPro` 참조 추가 (위 오버레이 기능용)
- 빌드 후 `BepInEx\plugins\` 로 자동 복사하는 단계 추가 (원작에는 없었음)

## 빌드

6개 전부:

```
dotnet build ClientMods.sln -c Release
dotnet build ClientMods.sln -c Release -p:"SptRoot=D:\내 SPT 경로"
```

`MunitionsExpert` 만 (권장 조합):

```
dotnet build MunitionsExpert/MunitionsExpert.csproj ItemAttributeFix/ItemAttributeFix.csproj -c Release
```

빌드하면 dll 6개를 **`$(SptRoot)\BepInEx\plugins\` 에 바로 복사합니다.**
원작은 자동 복사가 없어서 `bin\Release\` 에서 6개를 손으로 옮겨야 했습니다.

허브에서 받는 배포판과 같은 위치(plugins 바로 아래)에 평평하게 넣습니다. 그래야 다시
빌드했을 때 기존 설치본을 덮어쓰지, 두 벌이 남아서 BepInEx 가 중복 GUID 로 걸리지
않습니다.

복사가 싫으면:

```
dotnet build ClientMods.sln -c Release -p:CopyToSpt=false
```

---

## 확인한 것 / 확인 못 한 것

| | 상태 |
|---|---|
| 타입 20개 대응 | **확인** — 위키 4.0→4.1 표 전수 대조 |
| 멤버 이름 10개 대응 | **확인** — 실제 4.1 `Assembly-CSharp.dll` 메타데이터 |
| 이름으로 잡는 모든 멤버가 4.1에 존재 | **확인** — 23개 조회 전부. `MetadataLoadContext` 로 같은 조회를 실행해 `null`/예외가 없는지까지 봤습니다 |
| 필드 접근 지정자 | **확인** — public 으로 바뀐 4개를 찾아서 고쳤습니다 |
| 메서드 오버로드 모호성 | **확인** — `CreateContextButton` 하나가 걸렸고 시그니처로 특정했습니다 |
| 실제 4.1 `Assembly-CSharp.dll` 로 전체 컴파일 | **통과** — 6개 프로젝트 전부, 에러 0 |
| 인게임 검증 | **안 함** |

남는 경고는 `MagazineInspector` 의 `CS0618` 두 줄뿐입니다 (`InGameStatus.InRaid` 가
4.1에서 obsolete 로 표시됨). 원작 코드 그대로고 동작에는 문제 없습니다.

### 안 되면 여기부터 보세요

- **플리 오버레이가 안 뜬다**: `RagfairOfferItemViewPatch` 가 붙었는지 로그에서
  `Enabled patch RagfairOfferItemViewPatch` 를 확인하세요. 붙었는데 안 보이면
  `GridItemView.Caption` 의 RectTransform 이 64×64 아이콘 밖으로 밀려났을 수 있습니다
- **컨텍스트 메뉴 항목이 사라졌다**: 로그에 `AmbiguousMatchException` 이나
  `NullReferenceException` 이 있는지 보세요. 클라이언트가 4.1.5보다 새 빌드면 위 3번의
  이름/시그니처가 또 움직였을 수 있습니다
- **게임이 아예 안 켜진다**: `BepInEx\patchers\IcyClawz.CustomInteractions.Prepatch.dll`
  이 남아 있는지 확인하세요 (위 4번)
- **검사창이 `EXAMINE : {0}` / `Blablabla` 로 뜨고 안 닫힌다**: 위 5번 증상입니다. 예전
  빌드의 `IcyClawz.MunitionsExpert.dll` 이 남아 있는 겁니다 — 다시 빌드해서 덮어쓰세요

---

## License

원작과 동일 (University of Illinois/NCSA Open Source License, `LICENSE` 참고).
