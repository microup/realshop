# AGENTS.md — RealShop/Catalog/

## Что здесь

Логика каталога запчастей и тиров редкости (без Harmony-патчей):

| Файл | Роль |
| --- | --- |
| `RetroPartCatalog.cs` | `Initialize()` вызывается один раз при загрузке сцены магазина (`buildIndex==10`): проходит все `ShopType` через `Singleton<GameInventory>.Instance.GetItems(...)`, ставит `RepairGroup=1`, скрывает ретро-детали (`ShopGroup="Noinshop"`, `ShopName="none"`) и умножает цену на era-множитель. `GetRetroEntries()` парсит `ListRetroPartYears` (`year|part_id[|rarity]`, третий токен игнорируется) с кэшем по ссылке на массив; body-детали определяются автоматически через `CarData.TryGetBodyYear`. |
| `MaxQualityService.cs` | Потолок восстановления детали: `RarityFor`/`MaxFraction`/`GradeFor` — детерминированный хеш от `item.UID` (`HashUid`, соль `0x51ED2701L`), поэтому потолок стабилен между сохранениями. `IsBody` — body-детали получают отдельный диапазон `MaxQualityBodyMin/Max`. `IsRepairComplete` — деталь достигла потолка. |
| `MarginService.cs` | (v0.3.12) Маржинальная окупаемость следующего цикла ремонта: `TryCarGain` — прирост цены машины `baseCarPrice·CarSaleConditionSpan·Δ/N` по кэшу `CarPriceCache` (модель → baseCarPrice из `SetupForSell`) и множеству «нужно машине» (`GarageNeedsRecon.TryGetNeedModels`, `PartCountFor`); `ResaleGain` — прирост перепродажи детали; `IsCycleProfitable` — сравнение с полной стоимостью цикла. `EstimatedCycleDelta = 0.15` — оценка шага игры за цикл. Питает зелёный `$`-маркер и зелёную/красную раскраску превью. |

## Правила этого уровня

- Любая «редкость»/«потолок» выводятся из `item.UID` хешем — **не вводить** хранение rarity в конфиге или в самом предмете; третий токен `|rarity` в `ListRetroPartYears` — легаси, читается, но не влияет на цены/цвета.
- Изменения цен здесь применяются к `PartProperty.Price` однократно при инициализации каталога; повторный вызов `Initialize()` недопустим (защита `_isRunOnce` в точке входа).
- Множитель `0` из `GetYearRetroMultiplier` означает «не трогать»; скрытие и умножение цены — один и тот же блок (`continue` после скрытия).
- Новые списки всегда-скрытых машин/деталей (`SportGT`, `SpecialRetro`) — матчинг по подстроке `CarID`/`ID` через `Helpers.CheckForSubstrings`; добавлять id аккуратно, чтобы не скрыть детали современных машин (см. changelog v0.3.6 о разделённых engine-семействах).

## Чего не делать

- Не пересчитывать цены/потолки «на лету» при каждом открытии окна — каталог инициализируется один раз.
- Не менять соль/алгоритм `HashUid` без смены версии: существующие сохранения потеряют свои потолки.
- Не добавлять ручные списки body-деталей — они определяются из `config.txt` машин автоматически.
