# AGENTS.md — RealShop/Properties/

## Что здесь

`AssemblyInfo.cs` — версия и регистрация MelonMod:

- `AssemblyVersion`/`AssemblyFileVersion` — `0.3.11.0` (только числовая база, без суффиксов);
- `[assembly: MelonInfo(typeof(RealParts), "RealShop", "0.3.11-dev3", "https://microup.ru")]` — версия, видимая MelonLoader (здесь носится `-devN`);
- `[assembly: MelonGame("Red Dot Games", "Car Mechanic Simulator 2021")]` — привязка к игре.

## Правила этого уровня

- **Версия поднимается только после мержа MR в `main` на GitHub** (one bump per merge). В ветке разработки релиз-номер не трогать.
- В разработке `MelonInfo` = `X.Y.Z-devN` (`0.3.11-dev1`, `0.3.11-dev2`, …): N увеличивать на каждой dev-сборке.
- **Перед заливкой MR в `main` суффикс `-devN` убирать** — отдельный финальный коммит (`0.3.11-dev3` → `0.3.11`); в том же коммите обновлять `## Last update v X.Y.Z` в README.
- `AssemblyVersion`/`AssemblyFileVersion` физически не принимают `-devN` (только цифры): держать числовую базу `X.Y.Z.0`, синхронно с числовой частью `MelonInfo`.
- `MelonGame` и `MelonInfo`-тип (`RealParts`) не менять.
- `Guid` и `ProjectGuid` из `.csproj` не трогать.
