# AGENTS.md — корень репозитория RealShop

## Обзор проекта

Каноническое расположение проекта: `D:\dev\src\realshop\` (remote `https://github.com/microup/realshop.git`). Любые копии этого дерева в других папках — снимки для чтения; код менять только здесь.

RealShop — MelonLoader-мод для *Car Mechanic Simulator 2021* (Red Dot Games). C# / .NET Framework **4.8**, `OutputType=Library`, сборка в `RealShop.dll`. Игра собрана на **IL2CPP**, поэтому мод работает через MelonLoader 0.6.1 + UnhollowerBaseLib 0.4.18 и Harmony **2.9.0** (`using Harmony;` + `HarmonyInstance` — это алиасы Harmony 2.x, не устаревшая 1.x). Мод убирает из магазина запчасти машин старше `RemoveCarsOlderThanYear` (2005), вводит restoration ceilings (потолок восстановления, v0.3.9), экономику (sell-cap, labor cost, donor condition), маркеры потребности/апгрейда на свалке (v0.3.10) и условную экономику v0.3.11: цена продажи машины и деталей зависит от состояния, труд — гипербола `1 + k·cond/(1−cond)`, превью цены/прироста следующего цикла на кнопке Start; индикатор маржинальности (`MarginService`, в разработке как 0.3.11-devN): зелёный `$`-маркер на карточках и зелёная/красная раскраска превью, когда цикл ремонта окупается/не окупается (resale-канал детали + канал машины). Текущая версия: **0.3.11** в разработке, `MelonInfo` = `0.3.11-dev1` (`RealShop/Properties/AssemblyInfo.cs`).

## Настройка окружения

- Требуется установленная игра **Car Mechanic Simulator 2021** с **MelonLoader** (проверено на game build 1.0.40).
- **Канонический путь игры: `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\`**. Все `HintPath` в `.csproj` (21 ссылка) и `OutputPath` — абсолютные, от этого корня. Относительные `..\..\..\..\SteamLibrary\...` в `.csproj` запрещены (не совпадают с реальным расположением диска). Если игра в другом месте — правьте `HintPath` локально и **не коммитьте** это в `.csproj`.
- Конфиг мода создаётся игрой в `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\Mods\RealShop.cfg` (MelonPreferences); секретов нет.

## Сборка и запуск

```bash
# MSBuild 18 (Visual Studio 18 Community) — проверено, работает:
"C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe" RealShop.sln //p:Configuration=Debug //v:minimal
```

- NuGet не используется — только прямые `<Reference>` из каталога MelonLoader игры.
- Debug-сборка пишется прямо в `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\output\` (`OutputPath` в `.csproj`) — оттуда MelonLoader подхватывает `RealShop.dll`.
- **Не использовать** старый `C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe` — он не понимает C# 6+ (строковые интерполяции `$"..."` в `RealShop.cs`, `Inspect.cs`, `PatchBootstrap.cs`) и даёт ложные `error CS1056`.
- Релиз-архив для Nexus собирается отдельно, только по явному запросу.
- **Тестов в проекте нет и не создавать** без явного запроса. Проверка изменений — сборка + live-запуск игры; мод логирует каждый патч на старте (`[SellCap]`, `[RarityColor]`, `[DonorCond]`, `[Money]`, `[Recon]`) — по логам видно, какие хуки активны.

## Стиль кода и конвенции

- Один namespace `RealShop`; классы `internal static` для сервисов и патчей.
- Дефолты конфига — `const`/`static` поля в `Config.cs` (`ValuePriceParts*`, `MaxQuality*` и т.д.), наружу — свойства через `MelonPreferences_Entry.Value`.
- Логи только через `MelonLogger.Msg/Warning` с префиксом подсистемы в квадратных скобках.
- Не добавлять комментарии к коду, если об этом явно не просят.
- `#pragma warning disable CS0618` уже расставлен там, где нужны устаревшие MelonLoader API (`OnApplicationStart`, `MelonMod`), — не удалять.

## Структура (детали — в дочерних AGENTS.md)

| Директория | Назначение | AGENTS.md |
| --- | --- | --- |
| `RealShop/` (корень C#-проекта) | точка входа, конфиг, хелперы | [RealShop/AGENTS.md](RealShop/AGENTS.md) |
| `RealShop/Patches/` | все Harmony-патчи игры | [RealShop/Patches/AGENTS.md](RealShop/Patches/AGENTS.md) |
| `RealShop/Catalog/` | каталог запчастей и тир rarity | [RealShop/Catalog/AGENTS.md](RealShop/Catalog/AGENTS.md) |
| `RealShop/Runtime/` | установка патчей, рефлексия, recon | [RealShop/Runtime/AGENTS.md](RealShop/Runtime/AGENTS.md) |
| `RealShop/Scene/` | UI-дерево сцены, `GameObject.Find` | [RealShop/Scene/AGENTS.md](RealShop/Scene/AGENTS.md) |
| `RealShop/Travel/` | бесплатные поездки junkyard/auction | [RealShop/Travel/AGENTS.md](RealShop/Travel/AGENTS.md) |
| `RealShop/Properties/` | версия сборки, `MelonInfo` | [RealShop/Properties/AGENTS.md](RealShop/Properties/AGENTS.md) |

## Архитектура (кратко)

Поток: `RealParts` (MelonMod) → `OnApplicationStart` грузит `Config` и вызывает `PatchBootstrap.Install(harmony)` → `OnSceneWasLoaded(buildIndex==10)` инициализирует `RetroPartCatalog` (цены/скрытие магазина) → `OnUpdate` крутит `SceneUiController` и hotkeys. Игровые типы доступны через `CMS.*` (Assembly-CSharp) и `GameTypes.All()` (кэш рефлексии по сборке игры).

## Git

- Коммиты в стиле Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`), новые ветки — с префиксом `bionic/`.
- **Версионирование**: релиз-номер поднимать **только после мержа MR в `main`** на GitHub (один подъём на мерж). В ветке — суффикс `X.Y.Z-devN` в `MelonInfo` (N растёт на каждой dev-сборке); **перед заливкой MR суффикс `-devN` убирать** финальным коммитом. Подробности — [RealShop/Properties/AGENTS.md](RealShop/Properties/AGENTS.md).
- Перед завершением задачи: сборка проходит, `git status` чистый по артефактам (`bin/`, `obj/`, `output/`, `.vs/` в `.gitignore`).
- Не пушить без явного запроса.
- Системный контракт (`SYSTEM_PROMPT.md`) имеет приоритет над этим файлом в вопросах безопасности, секретов и объёма изменений; локальные правила его только конкретизируют.

## Запреты и подводные камни

- **Кодировки**: `RealShop/Config.cs` — **Windows-1251** (кириллические комментарии), `RealShop/Inspect.cs` — UTF-8 BOM, остальные — UTF-8 без BOM. При редактировании `Config.cs` сохранять CP1251; не перекодировать файлы «автоматически».
- Не править `.csproj` без необходимости; `HintPath`/`OutputPath` — абсолютные от `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\`, относительные пути запрещены.
- Не трогать `Assets/` (картинки README), `LICENSE*`, `.gitattributes`.
- Хуки на IL2CPP-методы игры ломаются при обновлении игры — любой новый хук обязан логировать успех/провал (образец: `PatchBootstrap.TryPatchSellItem`).
- `ListRetroPartYears` — формат `year|part_id[|rarity]`; третий токен rarity с v0.3.9 **игнорируется** (не удалять из строк конфига молча, совместимость).
- `Config.cs` огромен (~1600 строк, бо́льшая часть — таблицы id деталей); новые id добавлять в существующие массивы, не плодить дубли.

## Чек-лист валидации

```bash
"C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe" RealShop.sln //p:Configuration=Debug //v:minimal   # 0 errors, RealShop.dll в output\
git diff --stat                                # только ожидаемые файлы
```

Затем (при доступе к игре): запустить игру, в логе MelonLoader убедиться, что все ожидаемые `[...]`-патчи сообщают об успехе.
