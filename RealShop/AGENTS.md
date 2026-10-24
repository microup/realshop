# AGENTS.md — RealShop/ (корень C#-проекта)

## Что здесь

Точка входа мода и сквозные модули без собственной папки:

| Файл | Роль |
| --- | --- |
| `RealShop.cs` | `RealParts : MelonMod` — единственный вход MelonLoader. `OnApplicationStart` (config + `PatchBootstrap.Install`), `OnUpdate` (hotkeys, удержание `StartingMoney`, `_scene.Update`), `OnSceneWasLoaded` (init каталога на `buildIndex==10`). Здесь же глобальные константы (`RedPartConditionThreshold=15f`, `FullyRepairedPartConditionThreshold=99.5f`, `RepairGroupAvailableFromStart=1`, `DefaultJunkyardTravelCost=500`) и статические хелперы шкал условий `IsFullyRepaired`/`IsDamagedRed`. |
| `Config.cs` | Все MelonPreferences-категории (`MainSection`, `CarSection`, `RepairSection`, `Category`), файл `Mods/RealShop.cfg`, дефолты `Value*` и таблицы id (`RetroPartYears`, `SportGT`, `SpecialRetro`, `CarNames`, `ListWhiteCarParts`). С v0.3.11 в `CarSection` добавлены: `EnableSellConditionFactor`/`SellConditionFloor`/`SellConditionSpan`, `EnableLaborCurve`/`LaborCurveK`/`LaborCurveMaxCond`, `EnableCarSaleCondition`/`CarSaleConditionFloor`/`CarSaleConditionSpan`, `EnableRepairCostPreview`; с 0.3.11-devN — `EnableProfitMarker`; с 0.3.11-dev5 — `EnableAuctionRatingCap`/`AuctionMaxRating` (кап звёздного рейтинга машин на аукционе). |
| `Helpers.cs` | `Helpers.GetYearFromString` (год из `(1968)` в имени), `Helpers.CheckForSubstrings`; `CarData` — кэш лет машин из `StreamingAssets/Cars/*/config.txt` (`TryGetYearByCarId`, `TryGetBodyYear`). |
| `Inspect.cs` | Отладочный дамп детей GameObject'а (`MelonLogger`). |
| `RealShop.csproj` | Целевой фреймворк v4.8, x64, `AllowUnsafeBlocks`, ссылки на DLL игры по абсолютным путям от `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\`. |

## Правила этого уровня

- Всё в namespace `RealShop`. Точка входа одна — новые хуки сюда не добавлять, они живут в `Patches/` и ставятся через `Runtime/PatchBootstrap.cs`.
- `RealParts.ActiveConfig` — единственная легитимная точка доступа к конфигу из остальных модулей.
- Условия деталей в игре живут на шкале **0..1**; константы в `RealShop.cs` пересчитывают проценты в 0..1 — любое новое сравнение условий обязано использовать `IsFullyRepaired`/`IsDamagedRed`, а не сырые числа.
- Hotkeys: переключение мода (`IsKeyBindEnableSwitchMode`, по умолчанию F9), `Ctrl+AddMoneyKey` (S) — выдача денег, `Ctrl+G` — `GarageNeedsRecon.Dump()`. Новое сочетание — только через `Config` (записывать в cfg), не хардкодить.
- `Config.cs` в кодировке **Windows-1251** — редактировать с сохранением кодировки; массивы id деталей (~1400 строк) расширять внутри существующих массивов.
- `OnUpdate` вызывается каждый кадр: тяжёлую работу (рефлексию, поиск объектов) в нём не разворачивать без кэша.

## Чего не делать

- Не менять `buildIndex`-константы (1 = игровая сцена с версией сборки, 10 = сцена магазина) без проверки в игре.
- Не удалять `#pragma warning disable CS0618` — MelonLoader 0.6.1 требует устаревшие API.
- Не переводить `Config.cs` в UTF-8 и не «чистить» кириллические комментарии.
