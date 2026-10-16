# AGENTS.md — RealShop/Patches/

## Что здесь

Все Harmony-патчи игровых методов. Каждый класс — один хук или группа хуков одной подсистемы:

| Файл | Классы / цели |
| --- | --- |
| `EconomyPatches.cs` | `SellItemCapPatch`+`SellPriceCapPatch` (sell-cap, ищутся рефлексией в `PatchBootstrap`; с v0.3.11 cap умножается на `SellConditionFloor + SellConditionSpan·cond`), `SellProbePatch` (диагностика), `CarSaleConditionPatch` (`CarSummaryTab.SetupForSell` — цена продажи машины `baseCarPrice·(CarSaleConditionFloor + CarSaleConditionSpan·globalCondition)`, перезапись текстов sellValue/profit заменой цифр; кормит `CarPriceCache` — модель→baseCarPrice для `MarginService`), `FreeMapTravelPaymentPatch`/`FreeMapTravelSetMoneyPatch` (`GlobalData.AddPlayerMoney`/`SetPlayerMoney` — возврат стоимости поездки junkyard/auction), `StartingMoneyPatch`/`UpdateStartingMoneyPatch` (стартовые деньги). |
| `RepairPatches.cs` | `AddRedPartsToRepairTablePatch` (`Inventory.GetItemsForRepairTable` — красные детали в таблице ремонта), `DestroyPartOnRepairFailPatch` (`RepairPartWindow.ProcessGameResult` — провал ремонта уничтожает деталь), `RemoveRepairedPartsFromRepairWindowPatch`, `BlockRepairedPartMiniGamePatch`, `CapRepairResultOnProcessGameResultPatch` + `RepairResultConditionFixer` (потолок восстановления), `RefreshRepairListAfterRepairPatch`, `LaborCostService` + `LaborCostOnRepairPatch` (стоимость работы; с v0.3.11 гипербола `1 + LaborCurveK·cond/(1−cond)`, cond до `LaborCurveMaxCond`, состояние снимается в `Prefix` до ремонта), `RepairCostPreviewService` + три хука `RepairCostPreviewOn*Patch` (`FillItemData`/`FillItemDataWithAnimation`/`OnDownWindowItemChange` — на кнопке Start дописывается `+Δ%  -total`, total = игровая `CalculateRepairPrice` + труд; с v0.3.12 кнопка зелёная/красная по `MarginService.IsCycleProfitable`, цвет ставится только когда известен канал машины). |
| `JunkyardPatches.cs` | `FilterJunkyardCarsByYearPatch` (`CarBundleLoader.GetCarsForScene` — на свалке только машины старше cutoff; заполняет `JunkyardCarIds`), `JunkyardDonorCondition*` (`CarLoader.SetConditionOnDetails`/`SetConditionAll` — ухудшение состояния деталей донора). |
| `RarityColorPatches.cs` | Postfix-хуки `FillShopItem`/`FillInventoryItem`/`FillBasicInventoryItem`/`FillInventoryItemDetails`/`FillChoosePartDownItem` — цвет карточки по потолку (green/purple), маркеры `MAX xx%`, `current-max%`, оранжевый `!` (нужно машине), зелёный `▲` (есть лучшая копия) и зелёный `$` (v0.3.12, `ApplyProfitMarker`: следующий цикл ремонта окупается на машине; позиция под `!`/`▲`, свой `ProfitMarkerCache`, сброс на пул-слотах). |
| `TravelPatches.cs` | `MarkFreeTravelFrom*` (`MapWindow.SubmitItem`/`SubmitItemAction`), `UnlockEarlyDestinations*` (`SceneHelper.GetLevelForScene`/`CanGoToScene`/`CanDriveToDestination` — junkyard/auction с 1 уровня). |

## Правила этого уровня

- Новый хук = новый `internal static class` в тематическом файле с `[HarmonyPatch(typeof(<игровой класс>), "<метод>")]` **или** ручная установка из `Runtime/PatchBootstrap.cs`, если метод ищется рефлексией по сигнатуре (образцы: sell-cap, rarity color).
- Каждый хук обязан логировать установку и провал (`MelonLogger.Warning("[Подсистема] ... not found/failed")`) — это единственный способ обнаружить поломку хука после обновления игры.
- Тело хука: первым делом `RealParts.ActiveConfig == null` → выход; не кидать исключений наружу (try/catch для рефлексии обязателен).
- Пул-слоты UI переиспользуются игрой: любой визуальный эффект обязан корректно сбрасываться на «обычном» элементе (см. `OriginalBgColor`/`ColoredSlots` в `RarityColorPatches.cs`).
- `ProcessGameResult` имеет несколько postfix-патчей в одном методе — порядок и взаимное невмешательство проверять вручную (cap, refresh, labor, destroy).
- Шкала условий — 0..1; пороги брать из констант точки входа, не дублировать числа.

## Чего не делать

- Не патчить метод без лога успеха — молча отвалившийся хук = баг-репорт «мод сломался после апдейта».
- Не менять состояние игры из `Prefix` без `__result`/`ref`-параметров там, где достаточно `Postfix`.
- Не переименовывать классы патчей — на них ссылается поиск рефлексии по имени метода/сигнатуре.
