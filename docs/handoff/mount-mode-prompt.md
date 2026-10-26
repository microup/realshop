# PROMPT: режим монтажа и приёмка заказа в RealShop (после 0.3.11-dev49)

Ты работаешь в `D:\dev\src\realshop` (MelonLoader-мод для Car Mechanic Simulator 2021, IL2CPP, legacy `Harmony`, .NET Framework, csproj-стиль, не SDK). Задача ниже сформулирована как задача на доведение до результата, не на исследование.

## 1. Что нужно починить

Заказ «собрать машину с нуля»: машина приезжает голым остовом (панели сняты, мотор скрыт, салон отсутствует, качество везде 1 %). Нужно, чтобы игрок **мог ставить детали в режиме монтажа**, а в карточке заказа видел **список недостающих деталей**, которые надо купить/установить.

Признаки готовности (все проверяются по логу и скриншоту, по одному прогону):

1. Заказ после взятия остаётся **активным**: нет `offer expired without acceptance`, нет `cancelled before commit`, `BuildJobState.ActiveJobId` выставлен.
2. В карточке заказа в блоке «ДЕТАЛИ ДЛЯ РЕМОНТА» перечислены недостающие детали заказа (не только «Замените масло»).
3. В режиме монтажа подсвечиваются снятые панели (в прогоне id=81 их 12: `unmountedPanels=12 of 12`), и их можно установить.
4. Числа на карточке заказа соответствуют состояниям машины 1 % (или 0 % там, где категория не приезжает), а не требованиям заказа.

## 2. Главный вывод последних прогонов (это и есть цель работы)

Приёмка заказа **не завершается**. Порядок событий в одном прогоне (id=81, 0.3.11-dev49):

```
10:09:08.972  condition guard open: job=81 seconds=30
10:09:09.576  strip: chassis kept
10:09:12.533  panels stripped: mountedPanels=0 unmountedPanels=12 before=12 effective=True
10:09:16.390  condition strip: … parts=14 stage=2 frame=1% engine=hidden wheels=0 suspension=11 suspObjects=6
10:09:17.131  hook active: CancelJob
10:09:17.132  condition guard closed: (accept candidate cleared)
10:09:17.132  cancelled before commit: id=81
10:10:31.760  offer expired without acceptance: id=81
```

`AcceptOrderAction` вызывается, зачистка ложится, а через ~8 с игра вызывает `CancelJob`, и заказ **не переходит в принятое состояние** — в 10:10:31 он истекает как непринятый. Активного заказа нет ⇒ у режима монтажа нет списка деталей ⇒ подсвечивать нечего, а в карточке остались только собственные проблемы игры («замените масло»).

**Рабочая гипотеза, которую проверять первой:** зачистка подавляет ровно те методы, которыми игра завершает приёмку. Подавляются `CreateInterior, CreateParts, CreateDriveshaft, CreateExterior, CreateCarLightParts, CreateBonusParts` (cfg `BuildJobStripMethods`), и приёмка обрывается на них → `CancelJob`. Дешёвый проверочный эксперимент (без правок кода):

1. В `Mods/RealShop.cfg` очистить `BuildJobStripMethods` (значение `""`) и перезапустить.
2. Взять заказ, ничего не нажимая, 60 с.
3. Смотреть: есть ли `cancelled before commit` и `offer expired without acceptance`.

Если при пустом `BuildJobStripMethods` заказ становится активным — причина в подавленных методах; дальше разбирать, какой именно метод нужен потоку приёмки (см. декмп `CarLoader` в `/tmp/re_dump/cl/CarLoader.decompiled.cs`: `CreateParts`, `CreateCarLightParts`, `CreateBonusParts`, `CreateDriveshaft`, `CreateExterior`, `CreateInterior`), и подавлять его точечно (оставлять вызов оригинала, а результат портить после), а не выключать всю зачистку. Если заказ активен и с пустым списком методов, а монтажа всё равно нет — переходить к п. 3.

## 3. Дальнейший план, если гипотеза не подтвердится

1. Найти, что именно игра считает «принятым заказом»: состояние `OrderGenerator`/`Job` после успешной приёмки штатного заказа (сравнить лог штатного заказа без RealShop с логом нашего). Ключевые точки: `AcceptOrderAction`, `UpdateJobs`, `PrepareJob`, `SetupIssues`, `JobTask`/`JobPart` списки.
2. Проверить `JobTask.partsCount` и `JobTask.Parts` в `BuildJobServicePrices.BuildTasks()` — сейчас создаются задачи с `partsCount = 0` и пустым `Parts`. Игра может строить «ДЕТАЛИ ДЛЯ РЕМОНТА» именно из `JobPart`: при пустом списке список в UI пуст. Это прямой кандидат на п. 2 признаков готовности.
3. Режим монтажа: подсветка строится по задачам заказа и по «отсутствующим» деталям машины (`CarLoader.GetMissingWheelTypes`, `CheckCarHaveMountedWheel`, `SetRandomMissingPanels`, `SetRandomMissingSuspension`). Проверить, видит ли игра панели снятыми после того, как заказ стал активным.
4. F8 (переключатель объектов подвески) — вторично: `F8 matched=6 activeBefore=0 enabled=6`, базовая подсветка появляется, но при входе в монтаж исчезает. Это симптом, а не причина; чинить отдельно, когда заказ станет активным.

## 4. Что уже работает — не переделывать

- `tasks built: 11 of 11 bodyTasks=included (job tasks are never omitted)` (dev49): задачи заказа больше никогда не сокращаются. Решение пользователя: «оставить в заказе что надо сделать все детали и никогда их не удалять».
- Блокировка игровой отмены **выключена** (dev48, `BuildJobBlockCancel = false`): `CancelJob` через ~8 с — это завершение приёмки, а не отмена; блокировка оставляла заказ в списке предложений (`accept not confirmed … stillOffered waits=8 → 40`). **Не возвращать блокировку.**
- Guard состояния: переоткрывается после отмены (`BuildJobKeepGuardOnCancel = true`), окно `BuildJobGuardKeepSeconds = 600` с; форсы писателей качества работают (`condition guard: game writes forced to 1% (first via SetConditionAll)`).
- Клавиша переключателя — **F8** (cfg `BuildJobSuspensionToggleKey`). **F9 не использовать**: это встроенный хоткей MelonLoader «переключить моды» (`the mod has been False` → `accept candidate abandoned … (mod disabled)`).
- Колёса обязана убирать **игра**, косметическое скрытие колёс выключено (иначе игра не даст их поставить).
- Качества: старт 1 %, рама 1 %, отсутствующая категория → 0 %; двигатель не устанавливается (только требование и тип из поддерживаемых кузовом); салон отсутствует, требование компенсируется (иначе NRE на твине несуществующего салона).

## 5. Правила проекта (нарушение = откат)

- Писать только в `D:\dev\src\realshop\`.
- Файлы **> 500 строк запрещены** (`RealShop/ConfigInit.cs` уже 456 — новые записи cfg выносить в отдельный `partial`-файл; `Patches/CarStripPatches.cs` 493 — новые классы туда не добавлять).
- `RealShop/Config.cs` — кодировка **Windows-1251**, CRLF, править **только python-ом** (не редактором). `.cs` с кириллицей — UTF-8 с BOM.
- Новый `.cs` **явно** добавлять в `RealShop.csproj` (`<Compile Include="Jobs\X.cs" />`) — сборка не по маске; на этом уже падало.
- Тесты не создавать. Правила префиксов: префикс, пишущий `__result`, обязан вернуть `false` (`Patches/JobConditionPatches.cs`).
- Версия: `0.3.11-devN` (`RealShop/Properties/AssemblyInfo.cs`), следующий — **dev50**; релизный номер только после мержа в `main`. Не пушить. Не трогать `.git`, `.opencode`, `.github`.
- Конфиг чинит/мигрирует сам (`Config.RepairBuildJobConfig()`, гейт по `BuildJobConfigSchema`, сейчас схема **6**): одна миграция на ключ на bump схемы. Пользователь cfg руками не правит.
- Сборка:
  `"C:/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe" RealShop.sln /p:Configuration=Debug /v:minimal`
  (пишет в `output\`, post-build копирует в `Mods\`). Проверить, что `Mods\RealShop.dll` обновился и содержит новые строки лога (в IL2CPP-модификациях строки логов UTF-16, идентификаторы — ASCII).
- Порядок: ревью (`pal` + `pal-deepseek`) → сборка → документация (`README.md`, секция `## Last update v 0.3.11-devN (in development, unreleased)`). **Ревью не запускалось для dev28…dev49** — последний завершённый прогон ревью: dev27.
- Ответы пользователю по-русски; документы коллекции на английском. Не выдумывать API/команды/результаты; успех подтверждать строками лога.

## 6. Карта файлов

| Путь | Что там |
| --- | --- |
| `RealShop/Jobs/BuildJobServicePrices.cs` (237) | `BuildTasks()` (создание `JobTask`), цены, `FindCarLoaderByOrder` |
| `RealShop/Jobs/BuildJobServiceAdvance.cs` (229) | `FindCarLoaderByOrder(int jobId)` (`internal`): `CarLoaderPlaces.Get()` + `orderConnection == jobId` |
| `RealShop/Jobs/BuildJobServiceOffers.cs` | предложения, тик, `FindCarLoaderByOrder` |
| `RealShop/Jobs/BuildJobConditionGuard.cs` (401) | guard: `OpenGuard/CloseGuard/GuardApplies/GuardJobId`, форсы геттеров/писателей, `InteriorAbsent`, `KeepFrame`, `HideEngineObject` |
| `RealShop/Jobs/BuildJobCondition.cs` (364) | стадии зачистки, `TryApply`, `TryPersist`, лог `condition strip:` |
| `RealShop/Jobs/BuildJobStripWrites.cs` (412) | `UnmountByPrefix`, `StripSuspension`, `HideSuspensionObjects`, `WheelState`, логи имён деталей/подвески/рамы |
| `RealShop/Patches/JobAcceptPatches.cs` (345) | `AcceptOrderAction`, `CancelJob` (prefix `BuildJobBlockCancel` + postfix, переоткрытие guard) |
| `RealShop/Patches/CarStripPatches.cs` (493) | подавление методов сборки машины, `BuildJobStripTarget.IsMethodEnabled` |
| `RealShop/Patches/JobConditionPatches.cs` (244) | префиксы геттеров качества (правило: вернул `false`, если пишешь `__result`) |
| `RealShop/Jobs/BuildJobSuspensionToggle.cs` (113) | F8-переключатель объектов подвески |
| `RealShop/Config.cs` (CP1251) / `ConfigInit.cs` (456) | дефолты cfg / записи, `RepairBuildJobConfig`, миграции |
| `/tmp/re_dump/cl/CarLoader.decompiled.cs` | декмп: `CreateParts`, `CreateExterior`, `CreateInterior`, `CreateBonusParts`, `CreateCarLightParts`, `CreateDriveshaft`, `SetRandomMissingPanels`, `SetRandomMissingSuspension`, `GetMissingWheelTypes`, `CheckCarHaveMountedWheel`, `GetRootTransform`, `carParts` |
| `C:\Users\fets\AppData\LocalLow\Red Dot Games\Car Mechanic Simulator 2021\Player.log` | live-доказательства (read-only доступ выдан) |
| `D:\SteamLibrary\steamapps\common\Car Mechanic Simulator 2021\Mods\` | `RealShop.cfg` + `RealShop.dll` (write-доступ выдан) |

## 7. Строки лога, по которым читается состояние

- `tasks built: N of 11 bodyTasks=…` — состав задач заказа.
- `panels stripped: mountedPanels=A unmountedPanels=B before=C effective=…` — снятие панелей.
- `condition strip: … percent=1% parts=N stage=S frame=1% engine=hidden wheels=W suspension=11 suspObjects=6 missing=…` — состояние зачистки.
- `frame owner: body=1%/struct1% details=1%/struct1%` — записи качества рамы/деталей.
- `cancel stack:` / `cancelled before commit:` / `guard reopened after cancel:` — судьба приёмки.
- `offer expired without acceptance:` — заказ не стал активным (главный признак провала).
- `accept not confirmed: … stillOffered waits=N` — офер не ушёл из списка.
- `suspension toggle: F8 matched=M activeBefore=A enabled=E` — переключатель подвески.
- `the mod has been False` — сработал хоткей MelonLoader (F9), мод выключен.

## 8. Память

Память (`memory-service`, тег `realshop`, `conversation_id: realshop-dev23`) — обязательный цикл на каждый запрос: lookup → работа → сохранение исхода. Последняя запись: `ea8ee480e3dbb90a1253beef2d7aec1d3bea4dc6f5703424530622a9d8b07bf4` (dev49). Цепочка: dev48 `a527567388337b32a47cf041ecf5764d4533691bec58daca9d686e99613ae923`, dev47 `7b6d35e2cc2f1a8440c572854d4752b026937f93617066d0290e3ca355b97532`, dev46 `5d45ea2c3affa6253555f1e3e26bda9daeff7c01dd4e3181c359782896bde6ae`, dev45 `5b0325d1713b6249eeaf08889806c717caef60cc19df38348a1af1ede8b3f54e`.

Долг, который нельзя забыть: независимое ревью dev28…dev49 не проводилось.
