# RealShop
This mod was created with the aim of adding more realism to the game. In real life, it's unlikely that you can buy a new body part or engine for cars older than 2005. In this mod, I'm trying to address this issue to introduce more realism. There's still a lot of work to be done, but you can already experience and evaluate a certain part of the new gameplay. I hope you'll enjoy this challenge because now, to assemble an old car, you'll have to visit junkyards, auctions, or barns to find the necessary parts. Starting from v0.3.4, this gameplay is available from the beginning of a save because the junkyard and repair restrictions are opened earlier by the mod. Good luck in restoring retro cars! 😊

From v0.3.8 the hunt has a color: rare parts glow **green** and unique parts glow **purple** wherever they appear — junkyard, inventory, repair bench. Some finds are worth the trip the moment you spot them.

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
﻿﻿- A year dictionary `ListRetroPartYears` of `year|part_id` entries for engine, interior and wheel-rim parts that are no longer sold in the store. A part is hidden when its year is below `RemoveCarsOlderThanYear`; body parts do not need entries (they are detected from each car's own year). Entries may carry an optional rarity token `year|part_id|rarity` (`common`/`uncommon`/`rare`/`unique`) that drives the rarity price multiplier and the rarity color highlight.
- Whitelist addition: If a desired item is not available or not displayed in the store, it can be added to the configuration file whitelist for visibility.
﻿﻿- You can enable or disable visiting the store. You can also enable or disable the auction where higher-quality cars were sold
﻿﻿- Adding or removing specific brands from the list that won't appear in the shop.
- Change the prices of all spare parts through the configuration file using these variables (PriceParts1950_1959 and etc)  
- Economy realism toggles (on by default in v0.3.8): `EnableSellCap`/`SellRatio` (cap resale price), `EnableLaborCost`/`LaborCostBase` (charge labor per repair, scaled by rarity), `EnableDonorCondition`/`DonorConditionScale` (junkyard donors yield worse parts), and the `PriceRarity*` rarity multipliers.
13. Rarity highlighting: **rare** parts glow green and **unique** parts glow purple on their card in the junkyard, inventory and repair lists; common/uncommon keep the normal grey card, and highlighted slots reset correctly when the game reuses them.  

## Rarity & The New Economy

RealShop v0.3.8 turns parts into a treasure hunt. Every engine, interior and wheel-rim part can now carry a **rarity tier**, and rarity is not just a label — it changes what a part costs, what it takes to restore, and how it glows when you finally spot it in the dirt.

### Rarity tiers

| Tier | Card color | Shop price × | Repair labor × |
| --- | --- | --- | --- |
| Common | default (grey) | 1.0 | 1.0 |
| Uncommon | default (grey) | 1.5 | 1.5 |
| Rare | **green** | 2.5 | 2.5 |
| Unique | **purple** | 4.0 | 4.0 |

- Rarity is stored as an optional third token in the year dictionary: `year|part_id|rarity`. Old `year|part_id` entries still work and read as `common`.
- A **green** or **purple** card is your reward for keeping your eyes open: rare and unique parts are highlighted right in the junkyard, inventory and repair lists, so a once-in-a-restore find no longer hides in plain sight. Common and uncommon parts keep the normal grey card — the glow is reserved for the pieces worth chasing.
- The multipliers are fully configurable (`PriceRarityCommon` / `PriceRarityUncommon` / `PriceRarityRare` / `PriceRarityUnique`).

### What the new economy changes vs. the vanilla game

- **Two-factor pricing.** A hidden retro part's shop price is `base × era multiplier × rarity multiplier`. A 1950s unique engine block is not just old — it is genuinely precious.
- **Resale is capped (anti-flip).** Selling a part pays at most `SellRatio` (default **0.35**, i.e. 35%) of its shop price — and the number you see in the "Sell" dialog is the number you actually receive. Restoring and reselling can no longer print money.
- **Repairs cost labor.** Each successful repair charges `LaborCostBase` scaled by rarity, so a full rebuild of a rare engine is a real investment even when the part came out of a junkyard for free.
- **Old donors yield worse parts.** Junkyard donor cars older than the cutoff have their parts' condition scaled by `DonorConditionScale` (default 0.6), so a donor is a starting point, not a gift.
- **One bad call can end a legend.** A failed repair on a red part destroys it permanently — and now, with rarity, that loss can be a unique part you may never see again.

### Why it's worth it (the upside)

- The junkyard stops being a chore and becomes a **hunt**: spotting a purple card is a genuine jolt.
- Rarity gives old parts a reason to be precious, so a restored retro car feels *earned*.
- The loop is closed: cheap junkyard finds + capped resale + labor cost mean you can't brute-force your way to credits — you have to actually restore.

### The trade-offs (know them before you commit)

- **Rarer = pricier and slower.** A unique part costs 4× to buy and 4× in labor to restore; chasing purples is a long game.
- **Resale is deliberately weak.** At 35% you will almost always lose money flipping parts — that is the point, but it removes the easy-money loop the base game allows.
- **Donor parts arrive tired.** With donor-condition scaling, expect to spend time and labor bringing a donor part back up.
- **Risk is real.** Permanent part loss + rarity means a single mistake can erase a unique find. Save before high-stakes repairs.

### Risks & caveats

- Rarity is **data-driven**: only parts listed with a rarity token in `ListRetroPartYears` are highlighted and priced as rare. Everything else is `common`. If a part you expected to glow doesn't, it simply isn't tagged yet — add it to the dictionary.
- The economy hooks run on an IL2CPP build. The sell-cap (display + payment), labor charge, donor condition and rarity color were **verified in live gameplay** on build 1.0.40, but a future game update can move method signatures and silently disable a hook — the mod logs every patch at startup so you can tell what is active.
- Every economy feature is an independent toggle (`EnableSellCap`, `EnableLaborCost`, `EnableDonorCondition`) — turn any of them off to soften the challenge.

## Last update v 0.3.8

1. Two-factor part pricing: a hidden retro part's price is now `base × era multiplier × rarity multiplier`. `ListRetroPartYears` entries accept an optional third token — `year|part_id|rarity` (rarity = `common`/`uncommon`/`rare`/`unique`). Old two-token entries still work and default to `common`. Rarity multipliers are configurable: `PriceRarityCommon` (1.0), `PriceRarityUncommon` (1.5), `PriceRarityRare` (2.5), `PriceRarityUnique` (4.0).
2. Sell-price cap (anti-flipping): when `EnableSellCap` is on, selling a part pays at most `SellRatio` (default 0.35) of that part's shop price, so restoring and reselling a rare part can no longer print money.
3. Labor cost per repair: when `EnableLaborCost` is on, each successful repair also charges `LaborCostBase` scaled by the part's rarity, so a full rebuild of a 1950s engine costs real money even when the part came from the junkyard.
4. Junkyard donor condition: when `EnableDonorCondition` is on, the parts carried by junkyard donor cars have their condition scaled by `DonorConditionScale` (default 0.6), so old donors yield worse parts. This is scoped to junkyard cars only.
5. Rarity color highlighting: rare parts are tinted green and unique parts purple on their card background across the shop, junkyard, inventory and repair lists. Because the game reuses (pools) item slots, the mod caches each slot's original background color and restores it when a common part takes a previously highlighted slot, so the glow never leaks onto ordinary parts.
6. Repair-list correctness: a part that reaches 100% condition is now reliably removed from the repair table (the mod tracks part condition on the game's own 0..1 scale, not a fixed 0..100 assumption).

Note: the new economy features (`EnableSellCap`, `EnableLaborCost`, `EnableDonorCondition`) and rarity highlighting are **on by default** in v0.3.8 (with `SellRatio=0.35`, `LaborCostBase=100`, `DonorConditionScale=0.6`). Set any of them to `false` in the config to disable that feature independently. These behaviors (sell-cap display + payment, labor charge, donor condition, rarity color) were **verified in live gameplay** on build 1.0.40; because the game is built with IL2CPP, a future game update can still move method signatures, so the mod logs every patch at startup to show what is active.

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
