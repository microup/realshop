# AGENTS.md — RealShop/Travel/

## Что здесь

`FreeTravelService.cs` — сервис бесплатных поездок junkyard/auction. Сам патчей не содержит (они в `Patches/TravelPatches.cs` и `Patches/EconomyPatches.cs`); здесь состояние и расчёт:

- `PendingFreeTravelCost` — «ожидаем возврат»: помечается при выборе направления, сбрасывается при загрузке сцены; патчи `GlobalData.AddPlayerMoney`/`SetPlayerMoney` возвращают списанную сумму.
- `GetFreeTravelCost` — стоимость из `GlobalData.Cost_TravelJunkyard`/`Cost_TravelAuction`; фолбэк junkyard — `RealParts.DefaultJunkyardTravelCost` (500).
- `IsFreeTravelDestination` — только `MapDestinationID.Junkyard` и `Auction`.

## Правила этого уровня

- Механизм «списать → вернуть» двусторонний: любое изменение здесь требует парного изменения в `Patches/TravelPatches.cs` (пометка) и `Patches/EconomyPatches.cs` (возврат) — проверять обе стороны.
- `PendingFreeTravelCost` обязан сбрасываться при смене сцены (это делает точка входа в `OnSceneWasLoaded`) — иначе возврат сработает лишний раз.
- Новые бесплатные направления добавлять через `IsFreeTravelDestination` + соответствующий `GlobalData.Cost_*`, без хардкода сумм.

## Чего не делать

- Не менять семантику `PendingFreeTravelCost` на «запрет списания» — игра списывает деньги через `SetPlayerMoney` в момент поездки, возврат постфактум является частью контракта хуков.
