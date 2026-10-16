# AGENTS.md — RealShop/Runtime/

## Что здесь

Инфраструктура времени выполнения:

| Файл | Роль |
| --- | --- |
| `PatchBootstrap.cs` | Единая точка установки всех рефлексивных хуков: `TryPatchSellItem`, `TryPatchSellPrice`, `SellProbePatch.Install`, `RarityColorPatches.Install`, `TryPatchStartingMoney`, `TryCacheProfileManager`, затем `LogDiagnostics()`. Каждый `Try*` — try/catch + лог `[Подсистема] ... patched OK / not found`. Поиск целей идёт через `HarmonyLib.AccessTools.TypeByName` и перебор `GameTypes.All()` по имени и сигнатуре параметров. |
| `GameTypes.cs` | Кэш всех типов сборки игры (`typeof(Item).Assembly.GetTypes()`), устойчивый к `ReflectionTypeLoadException` (IL2CPP-сборка частично не грузится — это норма). |
| `GarageNeedsRecon.cs` | Рефлексивная разведка профиля: кэш `ProfileManager` (из `TryCacheProfileManager`), списки «какие детали нужны моим машинам» (`IsNeeded`) и «есть ли лучшая копия» (`IsBetter`) для маркеров `!`/`▲`; с 0.3.11-devN — `TryGetNeedModels` (id → модели, которым нужна деталь), `PartCountFor` (число деталей модели, вес в маржинальности) и кэш цены машины `TryGetModelBuyPrice` (`CarInfoData.BuyPrice` из профиля, канал машины без вкладки Sell) для `$`-маркера; `Dump()` печатает диагностику по `Ctrl+G`. Работает через `MemberCache` и перечисление `carsOnParking`/сохранённых машин. |

## Правила этого уровня

- Новый рефлексивный хук добавлять **только** сюда, в `PatchBootstrap.Install`, с парой «нашёл/запатчил» логов; декларативные `[HarmonyPatch]` остаются в `Patches/`.
- Рефлексия по игре всегда через `GameTypes.All()` (кэш), никогда не вызывать `GetTypes()` напрямую.
- `GarageNeedsRecon` читает сохранённый профиль (не живые объекты сцены) — машины, купленные на свалке, подхватываются с задержкой в несколько секунд; это ожидаемое поведение, не «чинить» кэшем сцены.
- Тяжёлые рефлексивные обходы кэшировать (`MemberCache`, `_cache`); `Dump()` — только по горячим клавишам, не из `OnUpdate`.

## Чего не делать

- Не молчать при ненайденном методе — всегда `MelonLogger.Warning` с именем подсистемы.
- Не искать типы строками вне известных кандидатов без перебора `GameTypes.All()` (после обновления игры имена могут съехать — перебор должен остаться последним рубежом).
- Не сохранять ссылки на IL2CPP-объекты надолго без `null`-проверок: они живут до перезагрузки сцены.
