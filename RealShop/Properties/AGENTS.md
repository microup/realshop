# AGENTS.md — RealShop/Properties/

## Что здесь

`AssemblyInfo.cs` — версия и регистрация MelonMod:

- `AssemblyVersion`/`AssemblyFileVersion` — `0.3.12.0`;
- `[assembly: MelonInfo(typeof(RealParts), "RealShop", "0.3.12", "https://microup.ru")]` — версия, видимая MelonLoader;
- `[assembly: MelonGame("Red Dot Games", "Car Mechanic Simulator 2021")]` — привязка к игре.

## Правила этого уровня

- При каждом релизе поднимать **все три** версии синхронно (AssemblyVersion, AssemblyFileVersion, MelonInfo) и обновлять `## Last update v X.Y.Z` в README репозитория в том же коммите.
- `MelonGame` и `MelonInfo`-тип (`RealParts`) не менять.
- `Guid` и `ProjectGuid` из `.csproj` не трогать.
