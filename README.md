# RealShop
This mod was created with the aim of adding more realism to the game. In real life, it's unlikely that you can buy a new body part or engine for cars older than 2005. In this mod, I'm trying to address this issue to introduce more realism. There's still a lot of work to be done, but you can already experience and evaluate a certain part of the new gameplay. I hope you'll enjoy this challenge because now, to assemble an old car, you'll have to visit junkyards, auctions, or barns to find the necessary parts. Starting from v0.3.4, this gameplay is available from the beginning of a save because the junkyard and repair restrictions are opened earlier by the mod. Good luck in restoring retro cars! 😊

## IMPORTANT

Starting from v0.3.4, RealShop is intended to be playable from the beginning of a save. The junkyard and repair flow are unlocked earlier by the mod, so you no longer need to wait until the full repair branch is unlocked before using it.

Repair is now a high-stakes restoration challenge. Even a good rare part that is not fully repaired can be lost forever after one failed repair attempt, so pushing a valuable find from "good enough" to perfect condition becomes a real decision. This gives more weight to junkyard, barn, and auction discoveries: quality and rarity matter because one mistake can turn a hard-to-find part into a permanent loss.

## Key Features:

1. Junkyards, barns, and auctions now play a significant role in the game.
2. Car parts for vehicles older than the configured cutoff (`RemoveCarsOlderThanYear`, default 2005) are disabled in the shopping catalog. The cutoff is a single switch: change it and the parts above/below it appear or disappear automatically. Body parts of every car older than the cutoff (base game and DLC) are detected and hidden automatically, without manual lists.
3. Access to some auction features is restricted.
4. Access to the car dealership is restricted.
5. All body and interior parts can be repaired.
6. Junkyard and auction access are unlocked from level 1 on the map.
7. Junkyard and auction travel are intended to be free.
8. Junkyard car spawns are filtered by `RemoveCarsOlderThanYear`; with the default value `2005`, only cars older than 2005 can appear there.
9. Repair is riskier: red parts can be restored, but any failed repair attempt immediately and permanently destroys the part, even when the part was already in good condition.
10. Fully repaired parts are hidden from the repair table list.
11. All repairable parts are available from the first repair level.
12. Configuration file allows for customization, including:
- Changing the year for cars that will not appear in the shop (add name to config section).
- Customizing the key binding to enable or disable the mod.
﻿﻿- A year dictionary `ListRetroPartYears` of `year|part_id` entries for engine, interior and wheel-rim parts that are no longer sold in the store. A part is hidden when its year is below `RemoveCarsOlderThanYear`; body parts do not need entries (they are detected from each car's own year). Entries may carry an optional rarity token `year|part_id|rarity` (`common`/`uncommon`/`rare`/`unique`) that drives the rarity price multiplier.
- Whitelist addition: If a desired item is not available or not displayed in the store, it can be added to the configuration file whitelist for visibility.
﻿﻿- You can enable or disable visiting the store. You can also enable or disable the auction where higher-quality cars were sold
﻿﻿- Adding or removing specific brands from the list that won't appear in the shop.
- Change the prices of all spare parts through the configuration file using these variables (PriceParts1950_1959 and etc)  
- Optional economy realism toggles (all off by default): `EnableSellCap`/`SellRatio` (cap resale price), `EnableLaborCost`/`LaborCostBase` (charge labor per repair, scaled by rarity), `EnableDonorCondition`/`DonorConditionScale` (junkyard donors yield worse parts), and the `PriceRarity*` rarity multipliers.  

## Last update v 0.3.8

1. Two-factor part pricing: a hidden retro part's price is now `base × era multiplier × rarity multiplier`. `ListRetroPartYears` entries accept an optional third token — `year|part_id|rarity` (rarity = `common`/`uncommon`/`rare`/`unique`). Old two-token entries still work and default to `common`. Rarity multipliers are configurable: `PriceRarityCommon` (1.0), `PriceRarityUncommon` (1.5), `PriceRarityRare` (2.5), `PriceRarityUnique` (4.0).
2. Sell-price cap (anti-flipping): when `EnableSellCap` is on, selling a part pays at most `SellRatio` (default 0.35) of that part's shop price, so restoring and reselling a rare part can no longer print money.
3. Labor cost per repair: when `EnableLaborCost` is on, each successful repair also charges `LaborCostBase` scaled by the part's rarity, so a full rebuild of a 1950s engine costs real money even when the part came from the junkyard.
4. Junkyard donor condition: when `EnableDonorCondition` is on, the parts carried by junkyard donor cars have their condition scaled by `DonorConditionScale` (default 0.6), so old donors yield worse parts. This is scoped to junkyard cars only.

Note: the three new economy features (`EnableSellCap`, `EnableLaborCost`, `EnableDonorCondition`) are **off by default**, so existing saves are unaffected until you opt in. Because the game is built with IL2CPP, the sell-price, repair-charge and donor-condition hooks were confirmed by method signature only and should be verified in-game; the toggles let you test each one independently.

## Last update v 0.3.7

1. Reworked how retro parts are hidden so that `RemoveCarsOlderThanYear` is now a single, flexible switch: change it and the parts above/below the cutoff appear or disappear automatically, without editing the part lists again.
2. The six year-based lists (`ListRetroParts1950_1959` … `ListRetroParts2001_2005`) are replaced by one year dictionary `ListRetroPartYears`, where every entry is `year|part_id`. A part is hidden only when its year is lower than `RemoveCarsOlderThanYear`, and the price multiplier is chosen from that same year.
3. Body parts are no longer marked by hand. The mod now reads each car's year from its own `config.txt` (base game and DLC cars alike) and automatically hides the body parts of every car older than the cutoff, so newly added cars are covered without any manual marking.
4. Engine, interior and wheel-rim parts stay in the year dictionary (the game does not expose a reliable part-to-engine mapping, so these remain explicit but are now year-tagged and threshold-driven). `ListSportGT` and `ListSpecialRetro` remain always-hidden lists.
5. Junkyard car filtering is more robust: each junkyard car's year is now read from its own `config.txt` first (covering base game and all DLC), falling back to the game API only if that fails. An old car can no longer be dropped from the junkyard when the game API fails to resolve a year that is present in the config.

Note: the config format changed. Delete `Mods/RealShop.cfg` so it regenerates with the new `ListRetroPartYears` section (the old `ListRetroParts*` sections are no longer used).

## Previous update v 0.3.6

1. Fixed a bug where parts shared between retro cars (older than 2005) and modern cars (2005 and newer) were hidden from the shop, which made some modern cars impossible to repair (for example, "Oil Filter I4 is not available" when repairing a Mini Cooper 2015).
2. Audited every hidden part ID against all cars from 2005 and newer and removed the shared ones from the year-based lists. This includes the generic AQC I4 (R4) engine family and the V6 3.0 engine family together with their tuned (`t_`) and exhaust (`w_`, `t_w_`) variants, plus interior, seat, bench and steering-wheel parts and body parts that were hidden by mistake but belong to modern cars.
3. Engine families that are exclusive to old cars (Willys L134, R4 for Sakura Osava, R4 Turbo for Olsen Pramie Club, R4 K-Series, R4 M31, SR20 and similar) stay hidden, so those retro cars still have to be restored from junkyards, barns and auctions.
4. Some generic retro wheel rims that are also used by a few modern cars were intentionally left hidden: rims are cosmetic and do not block repairs, while hiding them keeps the retro look.

Note: if you already have a config file, update the `ListRetroParts*` sections in `Mods/RealShop.cfg` (or delete the config so it regenerates with the new defaults).

## Previous update v 0.3.5

1. Extended the retro parts catalog with the newly installed DLC so that old cars are correctly removed from the shop and can only be found at junkyards, barns and auctions.
2. Added 16 cars older than 2005 that were missing from the shop filter: Jeep Willys Military (1942), Jeep Willys Civilian (1949), Bolt Appalachia (1950), Dodge Charger R/T (1968), Liner Navigator (1978), Sakura Osava (1980), Emden Saxon (1988), Salem Earthquake Gen IX (1989), Morena Bizzarini (1991), Bolt Escapade Mk IV (1991), CAB Roamer (1993), Sakura Tiara (1995), Bolt Ranger (1995), Salem Earthquake Gen X Rex (1999), Olsen Pramie Club (2002), Lacroix Marseille (2002).
3. Added the engine part families exclusive to these old cars (I4 Willys L134, R4 for Sakura Osava, R4 Turbo for Olsen Pramie Club) and the CAB Roamer RWD gearbox, so their engines are removed from the shop as well.
4. Engine parts shared with cars from 2005 and newer were intentionally left in the shop to avoid breaking modern cars.

Note: if you already have a config file, update the `ListRetroParts*` sections in `Mods/RealShop.cfg` (or delete the config so it regenerates with the new defaults).

## Previous update v 0.3.4

1. Junkyard and auction access are now available from level 1 and unlocked on the map.
2. Junkyard and auction travel costs are intercepted so these trips do not charge credits.
3. Junkyard cars are filtered by `RemoveCarsOlderThanYear`. Default `2005` means cars from 2005 and newer will not spawn there; 2004 and older can spawn.
4. Red parts can now be repaired, and every repair attempt carries permanent risk: one failed decision destroys the part immediately, even if it was a good-quality find below 100%.
5. The repair table now hides fully repaired parts.
6. Repairable parts are available from the first repair level instead of requiring the restorer branch level.
7. Default economy values were rebalanced for first-time config generation:
   - `PriceParts1950_1959 = 8`
   - `PriceParts1960_1969 = 7`
   - `PriceParts1970_1979 = 6`
   - `PriceParts1980_1990 = 4`
   - `PriceParts1991_2000 = 3`
   - `PriceParts2001_2005 = 2`
   - `PricePartsSportGT = 6`
   - `PricePartsSpecialRetro = 10`
   - `PricePartsOther = 2`

These defaults are used only when `Mods/RealShop.cfg` does not exist yet. If you already have a config file, update the values manually if you want to use the new balance.

## Previous update v 0.3

1. added groups for different car release periods, now the older the car, the more expensive it is.
2. a group for very rare and sports cars has been added, with their prices being very high.
3. all prices for parts have been revised. Now, it is easy and not expensive to assemble cars that are younger than 2005, and all the parts for them should be available in the store.
4. the store has been fixed and redesigned, now most of the various car parts items are displayed correctly.

This mod seriously changes the pricing policy of gameplay. Now, parts and repairs for retro cars will be very expensive, so be sure to check what's available in the junkyard - it may be the most cost-effective way to get the necessary parts.

Global mod update: Introduces price changes for cars based on age, adds a group for rare sports cars, and revises part prices. Gameplay now emphasizes cost-effective part sourcing for retro cars.

Exclusive cars:  Mercedes-Benz W198 300SL (1954), Aston martin DB5 (1963), JUEYUAN QIANFENG (1959), DC TYPHOON (1964), FORD GT 40 (1965), JAGUAR E-TYPE (1961), JAGUAR XJ220 (1992), LOTUS ESPRIT S1 (1975), SAKURA GT20 (1969)

(these cars are the most expensive in the game, but you can modify this list in the configuration file.)

Sport cars: PAGANI ZONDA REVOLUTION, PORSCHE 911 RS AMERICA (964),  PORSCHE 911 Turbo S (964)  (1993), PORSCHE CARRERA GT (2003), PORSCHE 911 CARRERA 4S (991) (2016)

Retro 1950_1959: DELRAY CUSTOM (1957),  SALEM 1500 (1955)

and etc....

----

It's important to note that many parts in the shop will be unavailable, making regular visits to junkyards necessary. Non-default items can't be found in the shop, but can be discovered in barns, auctions, or junkyards and then repaired. Make sure to replace the old configuration file with the new one.

## With this mod, realism is brought closer to the game. Enjoy the enhanced experience!

## How install:

1. Download realshop.dll
2. Download and install [MelonLoader](https://melonwiki.xyz/#/?id=requirements) ----> **!!!!  0.5.7 VERSION**
3. Always delete old files to this mod "Car Mechanic Simulator 2021\Mods"
4. Copy the dll file to the **"Car Mechanic Simulator 2021\Mods"**

##
In the plans, I will remove engines, tires, and other items for old and retro cars from the store. Now junkyards, barns, and auctions take on a different meaning.

## Before
<p align="center">
  <img src="/Assets/before.png" alt="before">
</p>

## After
<p align="center">
  <img src="/Assets/after.png" alt="before">
</p>

## Install Melonloader
<p align="center">
  <img src="/Assets/melon.png" alt="before">
</p>

[nexusmods.com](https://www.nexusmods.com/carmechanicsimulator2021/mods/164)
