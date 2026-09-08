# IcyClawz ClientMods (fork)

> **원작자 · 원본 레포**
> **IcyClawz** — https://github.com/IcyClawz/ClientMods
>
> 이 레포는 위 원작의 **포크**입니다. 기능은 그대로고, **SPT 4.1에서 빌드·동작하도록
> 포팅**한 것이 전부입니다. 포크 시점은 `2b9ed9f` (EFT 0.16.9.0.40087, SPT 4.0).
>
> 여기에 플리마켓 탄약 오버레이 기능 하나를 추가했습니다 (아래 "추가한 기능").

현재 기준 **SPT 4.1**.

포함된 플러그인:

| 프로젝트 | 하는 일 |
|---|---|
| `MunitionsExpert` | 탄약에 관통/장갑피해/파편화/도탄/내구소모/발열/불발 항목을 추가하고, 아이콘 배경을 관통 레벨 색으로 칠함 |
| `MagazineInspector` | 탄창에 들어 있는 탄약 수를 스킬 수준에 맞춰 표시 |
| `ItemSellPrice` | 아이템 정보창에 상인 판매가 표시 |
| `ItemAttributeFix` | 압축 표시된 속성 툴팁이 잘린 값을 보여주던 문제 수정 |
| `CustomInteractions` | 다른 모드가 컨텍스트 메뉴에 항목을 붙일 수 있게 해주는 API |
| `ItemContextMenuExt` | 위 API 로 붙는 실제 메뉴 항목들 |

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

### 2. 멤버 이름 — 대응표가 없어서 실기 로그로 확인했습니다

SPT 4.1.5 클라이언트의 BepInEx 로그에는 HarmonyX 가 패치 대상 메서드의 **IL 을 통째로
덤프**해 둡니다. 거기서 실제 4.1 시그니처를 직접 읽어 확인했습니다.

| 4.0 | 4.1 | 확인 방법 |
|---|---|---|
| `InteractionButtonsContainer.method_1` | `CreateContextButton` | 로그의 IL 덤프 |
| `InteractionButtonsContainer.method_3` | `CreateDynamicContextButton` | 로그의 IL 덤프 |
| `InteractionButtonsContainer.method_4` | `CloseSubMenu` | 로그의 IL 덤프 |
| `InteractionButtonsContainer.method_5` | `BindButton` | 로그의 IL 덤프 |
| `InteractionButtonsContainer.simpleContextMenuButton_0` | `_subMenuButton` | `ldfld EFT.UI.SimpleContextMenuButton ...::_subMenuButton` |
| `CompactCharacteristicPanel.string_0` | `_dataForTooltip` | `stfld System.String ...::_dataForTooltip` |

같은 로그에서 이 모드가 이름으로 잡는 나머지 멤버들도 4.1에 그대로 살아 있는 걸
확인했습니다: `GridItemView.Caption` (`TMPro.TextMeshProUGUI`),
`RagfairOfferItemView.UpdateInfo`, `ItemView.UpdateColor`, `StaticIcons.GetAttributeIcon`,
`ItemUiContext.GetItemContextInteractions`, `ItemUiContext.ShowContextMenu`,
`InteractionButtonsContainer._buttonTemplate` / `_buttonsContainer`,
`CompactCharacteristicPanel.ItemAttribute` / `SetValues`,
`DynamicContextInteraction.Key` / `Icon` / `Execute`.

### 3. 이름을 못 찾은 멤버는 "모양"으로 찾습니다

로그에도 안 나오고 표도 없는 멤버가 셋 있었습니다. 이름 대신 **타입으로** 찾도록
바꿨습니다 — 셋 다 해당 타입에서 그 타입을 가진 유일한 필드라서 모호하지 않습니다.

| 4.0 이름 | 어디 | 지금 찾는 방법 |
|---|---|---|
| `ItemInfoInteractionsAbstractClass<T>.Dictionary_0` | CustomInteractions | `ContextInteractions<T>` 에서 유일한 `Dictionary<string, DynamicContextInteraction>` |
| `TraderClass.SupplyData_0` | ItemSellPrice | `Trader` 에서 유일한 `SupplyData` |
| `DynamicInteractionClass.Action_0` | CustomInteractions | `DynamicContextInteraction` 에서 유일한 `Action` |

`Ienumerable_0` → `_subInteractions`, `bool_8` → `_expanded` 처럼 **타입에서 파생된
이름(`Action_0`, `Dictionary_0`, `SupplyData_0`)은 4.1이 실제로 바꿉니다.** 이름을
그대로 두면 조용히 `null` 이 돌아와서 NRE 로 죽습니다.

### 4. 프리패처를 들어냈습니다

`CustomInteractions.Prepatch` 는 BepInEx 프리로더 단계에서 `Assembly-CSharp.dll` 을
직접 고치던 패처였습니다. 하는 일은 두 가지:

1. `DynamicInteractionClass.Action_0` 을 `protected` + 비-readonly 로 열어서, 서브클래스가
   직접 대입할 수 있게 함
2. 생성자의 `callback` 인자를 optional 로 만들어서 `base(id, id)` 2인자 호출이 되게 함

4.1에서는 **이대로 두면 로딩 자체가 터집니다.** `assembly.MainModule.GetType("DynamicInteractionClass")`
가 `null` 을 돌려주기 때문입니다 — 4.1에서 이 타입은 전역이 아니라 `EFT.UI` 안에 있고,
Cecil 의 `GetType` 은 네임스페이스를 포함한 전체 이름을 요구합니다.

고쳐서 살릴 수도 있었지만, 위 3번에서 `Action_0` 을 리플렉션으로 잡게 바꾸고 생성자를
`base(id, id, null)` 로 명시하니 **두 가지 조작이 전부 필요 없어졌습니다.** 그래서
프로젝트를 삭제했습니다. 남겨두면 오히려 손해입니다 — 4.0에서 `public` 이던 필드를
`protected` 로 **좁히는** 조작이라, 같은 필드를 IL 로 직접 읽는 다른 모드를 깨뜨릴 수
있습니다.

> **업그레이드하는 경우**: `BepInEx\patchers\` 에 남아 있는 예전
> `IcyClawz.CustomInteractions.Prepatch.dll` 을 **지우세요.** 그게 남아 있으면 위에서
> 말한 `null` 때문에 프리로더 단계에서 죽습니다.

덤으로, 이것 때문에 `CustomInteractions` 가 참조하던
`Shared\Assembly-CSharp-CustomInteractions.dll` (프리패치가 적용된 게임 어셈블리 사본)
도 필요 없어졌습니다. 이제 설치본의 `Assembly-CSharp.dll` 을 그냥 참조합니다.

---

## 빌드 설정도 갈아엎었습니다

- **모든 프로젝트가 `..\Shared\*.dll` 을 참조하고 있었습니다** — 레포에 없고 손으로
  채워야 하는 폴더라, 새로 클론하면 참조가 전부 한꺼번에 깨지고 뭐가 없는지도 안
  알려줍니다. `Directory.Build.props` 에서 `SptRoot` 로 설치본을 직접 가리키게 바꿨습니다
  (기본값 `E:\SPT 4.1`, `-p:SptRoot=...` 또는 환경변수로 덮어쓰기)
- `SptRoot` 가 SPT 설치본이 아니면 "타입을 찾을 수 없음" 수십 줄 대신 **이유를 말하는
  에러 하나**로 실패합니다
- `net472` → `netstandard2.1` (SPT 4.1 클라 플러그인 기준). **단 `MunitionsExpert` 만
  `net472` 로 남겼습니다** — `Resources.resx` 에 PNG 3장이 들어 있고 디자이너가 그걸
  `System.Drawing.Bitmap` 으로 타이핑하는데, `System.Drawing` 은 netstandard2.1에
  없습니다. 아이콘을 raw byte 로 갈아엎지 않는 한 못 옮깁니다. BepInEx 는 Mono 에서
  net472 플러그인을 잘 로드하고 원작도 그렇게 배포했으니 그대로 뒀습니다
- 요즘 SDK 는 BinaryFormatter 기반 리소스 쓰기를 아예 빼버려서, net472에서도 위 PNG
  들이 MSB3822/MSB3823 으로 실패합니다 → `GenerateResourceUsePreserializedResources`
  + `System.Resources.Extensions` 추가
- `ItemContextMenuExt` 가 `IcyClawz.CustomInteractions.dll` 을 파일로 참조하고 있었는데
  같은 솔루션 안에 있는 프로젝트라 `ProjectReference` 로 교체
- `MunitionsExpert` 에 `Unity.TextMeshPro` 참조 추가 (위 오버레이 기능용)

## 빌드

```
dotnet build ClientMods.sln -c Release
dotnet build ClientMods.sln -c Release -p:"SptRoot=D:\내 SPT 경로"
```

각 프로젝트의 `bin\Release\` 에 dll 이 나옵니다. `BepInEx\plugins\` 에 넣으세요.
(원작과 마찬가지로 자동 복사는 없습니다.)

---

## 확인한 것 / 확인 못 한 것

| | 상태 |
|---|---|
| 타입 20개 대응 | **확인** — 위키 4.0→4.1 표 전수 대조 |
| 멤버 6개 대응 | **확인** — 실제 4.1.5 클라 로그의 HarmonyX IL 덤프 |
| 이름으로 잡는 나머지 멤버가 4.1에 존재 | **대부분 확인** (위 로그). `EntityIcon._colorPanel` / `EntityIcon.Show` / `ItemView.BackgroundColor` / `AmmoTemplate.GetCachedReadonlyQualities` 는 로그에 안 잡혔지만, 넷 다 원래부터 실명이라 역난독화 대상이 아닙니다 |
| 이름 못 찾은 멤버 3개 | **모양으로 회피** — 해당 타입에서 유일한 타입이라 이름이 뭐가 되든 잡힙니다 |
| 4.1 형태 어셈블리로 전체 컴파일 | **통과** — 4.0 `Assembly-CSharp` 에 위 타입 리네임을 Cecil로 실제 적용한 DLL을 만들어 6개 프로젝트 전부 빌드 |
| 실제 4.1 `Assembly-CSharp.dll` 로 컴파일 | **못 함** — 이 작업 환경에 4.1 클라이언트 어셈블리가 없습니다 |
| 인게임 검증 | **안 함** |

### 안 되면 여기부터 보세요

- **플리 오버레이가 안 뜬다**: `RagfairOfferItemViewPatch` 가 붙었는지 로그에서
  `Enabled patch RagfairOfferItemViewPatch` 를 확인하세요. 붙었는데 안 보이면
  `GridItemView.Caption` 의 RectTransform 이 64×64 아이콘 밖으로 밀려났을 수 있습니다
- **컨텍스트 메뉴 항목이 사라졌다**: 위 3번의 "모양으로 찾기" 셋 중 하나가
  `SingleOrDefault` 에서 걸렸을 가능성 — 4.1이 같은 타입 필드를 하나 더 추가했다면
  예외가 납니다. 로그에 `InvalidOperationException` 이 뜹니다
- **게임이 아예 안 켜진다**: `BepInEx\patchers\IcyClawz.CustomInteractions.Prepatch.dll`
  이 남아 있는지 확인하세요 (위 4번)

---

## License

원작과 동일 (University of Illinois/NCSA Open Source License, `LICENSE` 참고).
