# RealShop
This mod was created with the aim of adding more realism to the game. In real life, it's unlikely that you can buy a new body part or engine for cars older than 2005. In this mod, I'm trying to address this issue to introduce more realism. There's still a lot of work to be done, but you can already experience and evaluate a certain part of the new gameplay. I hope you'll enjoy this challenge because now, to assemble an old car, you'll have to visit junkyards, auctions, or barns to find the necessary parts. Starting from v0.3.4, this gameplay is available from the beginning of a save because the junkyard and repair restrictions are opened earlier by the mod. Good luck in restoring retro cars! 😊

From v0.3.9 the hunt has a color **and a ceiling**: no part can be restored to a flawless 100% anymore — every part can only be brought back so far, and the better it can get, the brighter it glows. Ordinary parts top out around 30–50%, good ones reach 50–70% and glow **green**, and only rare survivors can be pushed to 70–100% and glow **purple**. A car you raise from the junkyard is never perfect — and that is the whole challenge. Bodywork is the one place the rule bends: rusted metal can be cut, welded and patched, so panels can always be brought back further than the rest of the car. **The deal is simple:** a car you pull from the junkyard will never be a flawless 100% — but hunt patiently, weld well and choose your parts, and you can still bring them **above 70%** and build a machine that turns heads. Perfect is gone; remarkable is still yours to earn.

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
8. Junkyard car spawns are filtered by `RemoveCarsOlderThanYear`; with the default value `2005`, only cars older than 2005 can appear there. The auction is filtered by the same cutoff (v0.3.11-dev6): cars from the cutoff year and newer are not offered there at all.
9. Repair is riskier: red parts can be restored, but any failed repair attempt immediately and permanently destroys the part, even when the part was already in good condition.
10. Fully repaired parts are hidden from the repair table list.
11. All repairable parts are available from the first repair level.
12. Configuration file allows for customization, including:
- Changing the year for cars that will not appear in the shop (add name to config section).
- Customizing the key binding to enable or disable the mod.
﻿﻿- A year dictionary `ListRetroPartYears` of `year|part_id` entries for engine, interior and wheel-rim parts that are no longer sold in the store. A part is hidden when its year is below `RemoveCarsOlderThanYear`; body parts do not need entries (they are detected from each car's own year). As of v0.3.9 the optional third token `year|part_id|rarity` is **ignored**: a part's rarity is no longer stored in the config but derived from its hidden restoration ceiling (see *Rarity, Restoration Ceilings & The New Economy*); shop prices for hidden retro parts are now era-based only.
- Whitelist addition: If a desired item is not available or not displayed in the store, it can be added to the configuration file whitelist for visibility.
﻿﻿- You can enable or disable visiting the store. You can also enable or disable the auction where higher-quality cars were sold
﻿﻿- Adding or removing specific brands from the list that won't appear in the shop.
- Change the prices of all spare parts through the configuration file using these variables (PriceParts1950_1959 and etc)  
- Economy realism toggles (on by default in v0.3.8): `EnableSellCap`/`SellRatio` (cap resale price), `EnableLaborCost`/`LaborCostBase` (charge labor per repair, scaled by grade), `EnableDonorCondition`/`DonorConditionScale` (junkyard donors yield worse parts), and the `PriceRarity*` labor multipliers.
- Restoration ceilings (`EnableMaxQuality`, on by default): the per-part maximum repair condition, the `MaxQuality*Min/Max` ranges and the `MaxQuality*Weight` odds. Body panels are the deliberate exception — they roll their own restoration range `MaxQualityBodyMin`–`MaxQualityBodyMax` (default 50–100%), so a rusted panel can come back anywhere from tired to as-new. Starting balance for a new game via `StartingMoney` (default 20000).
13. Rarity highlighting: a part's card glows **green** (restoration ceiling 50–70%) or **purple** (ceiling 70–100%) in the junkyard, inventory and repair lists; ordinary parts stay grey, and highlighted slots reset correctly when the game reuses them. A part that has reached its ceiling shows a **`MAX xx%`** marker; below it the card reads `current-max%`.  
14. Restoration ceilings: no part can be repaired to a flawless 100%. Each part has a hidden maximum it can be restored to, decided once per part and stable across saves; `EnableMaxQuality` toggles the whole system.
15. Junkyard need/upgrade markers: an orange `!` marks a part your cars are missing, and a green `▲` marks a part you already own that has a better-condition copy on the junkyard — checked against every car in the garage and on the parking lot.  
16. Auction quality cap: no car offered at the auction is rated above `AuctionMaxRating` (default **1 star**). The star rating drives the car's value, starting price, the AI's max bid and its generated condition, so the auction you can reach from level 1 sells only the cheapest, most tired machines. Toggle with `EnableAuctionRatingCap`.
17. Dealership year filter: the car dealership shows only modern cars. Every car whose own `config.txt` year is below `RemoveCarsOlderThanYear` (default 2005) is hidden from the showroom grid — the same single switch as the junkyard and auction. The year is read from each car's id (base game and DLC alike) and falls back to the year in the car's name, so cars whose name carries no year are filtered correctly too.

## Rarity, Restoration Ceilings & The New Economy

RealShop v0.3.9 changes what "repair" means. In the vanilla game any part can be ground back to a flawless 100% — a rust-eaten junkyard block comes out looking factory-new. That is the illusion this mod breaks.

### You can no longer restore a part to 100%
Every part now carries a **hidden restoration ceiling** — the best condition its worn core will ever allow. It is decided once, the moment the part enters the world, and it never changes (not across saves, not across repairs, and not with your skill level). How far a part can be brought back is a property of the part, not of your patience.

- Most parts top out around **30–50%** — ordinary, tired, honest about it.
- Some reach **50–70%** — worth the detour.
- A lucky few can be pushed to **70–100%** — the survivors worth building a car around.

The ceiling is rolled from the part's unique id, so it is stable and reproducible. A part that has hit its ceiling stops improving, drops out of the repair list, and shows a **`MAX xx%`** marker (grey bar) so you know the hunt for that piece is over. Below the ceiling the card reads **`current-max%`** (e.g. `12-45%`) so you always know the target.

### Rarity is now the ceiling — and it has a color
Rarity is no longer a label in a config file. It is simply **how good a part can get**, and the card color falls out of it:

| Grade | Card color | Restoration ceiling | How often | Repair labor × |
| --- | --- | --- | --- | --- |
| Common | grey | 30–50% | ~65% | 1.0 |
| Rare | **green** | 50–70% | ~25% | 2.5 |
| Unique | **purple** | 70–100% | ~10% | 4.0 |

- A **green** or **purple** card is no longer a pre-tagged treasure — it is a part that happens to have a high ceiling, and it is genuinely rare (only about 1 in 10 parts can be pushed past 70%).
- The grade also drives **repair labor cost** (green 2.5×, purple 4×), so the parts worth keeping are also the parts that cost to restore.
- The odds and ranges are configurable: `MaxQualityCommonWeight/RareWeight/UniqueWeight` (65/25/10) and `MaxQualityCommonMin/Max`, `MaxQualityRareMin/Max`, `MaxQualityUniqueMin/Max`.
- New parts bought in the shop are **not** colored — a part's ceiling only exists once the part is yours (in the inventory or lying in the junkyard).

### What the new economy changes vs. the vanilla game

- **Era-based pricing.** A hidden retro part's shop price is `base × era multiplier` — the older the era, the dearer the part. (The old config rarity multiplier was retired when rarity became a per-part ceiling.)
- **Resale is capped (anti-flip).** Selling a part pays at most `SellRatio` (default **0.35**, i.e. 35%) of its shop price — and the number you see in the "Sell" dialog is the number you actually receive. Restoring and reselling can no longer print money. Since v0.3.11 the cap scales with the part's condition: `SellRatio × (0.4 + 0.6 × condition)` — a restored part sells for more than the same part in rust.
- **Repairs cost labor.** Each successful repair charges `LaborCostBase` scaled by the part's grade (green 2.5×, purple 4×), so a full rebuild of a high-ceiling engine is a real investment even when the part came out of a junkyard for free. Since v0.3.11 the fee also grows with the part's condition — see "The Condition Economy".
- **Old donors yield worse parts.** Junkyard donor cars older than the cutoff have their parts' condition scaled by `DonorConditionScale` (default 0.6), so a donor is a starting point, not a gift.
- **One bad call can end a legend.** A failed repair on a red part destroys it permanently — and now a high-ceiling part is painful to lose.

### Why it's worth it (the upside)

- A car you raise from the junkyard is **never perfect**. Every panel and every block carries its own ceiling, so a full restoration is a long, deliberate hunt for the few parts that can actually reach high condition — and the finished car wears that history. The ceiling is real, but so is the reward: the parts that clear 70% are exactly the ones worth chasing, and a car built from them is a trophy, not a showroom copy.
- The junkyard stops being a chore and becomes a **hunt**: spotting a purple card is a genuine jolt, because it means this piece can be brought back almost to new.
- The loop is closed: capped resale + labor cost + restoration ceilings mean you can't brute-force a flawless build — you have to actually hunt.

### The trade-offs (know them before you commit)

- **Nothing is ever 100%.** If you want a pristine, all-green garage, this is not it — most parts cap low, and that is the point.
- **Rarer = slower and dearer to restore.** A purple part costs 4× in labor to bring up, and there are few of them; chasing purples is a long game.
- **Resale is deliberately weak.** At 35% you will almost always lose money flipping parts — that is the point, but it removes the easy-money loop the base game allows.
- **Donor parts arrive tired.** With donor-condition scaling, expect to spend time and labor bringing a donor part back up.
- **Risk is real.** Permanent part loss + ceilings means a single mistake can erase a part that could have been brought back to near-new. Save before high-stakes repairs.

### Risks & caveats

- Rarity is **per-part and random**: it comes from each part's restoration ceiling, rolled from its id. Two used copies of the same part can roll different ceilings — that is intended (no two worn parts are equally tired). There is nothing to tag by hand anymore.
- The economy hooks run on an IL2CPP build. The sell-cap (display + payment), labor charge, donor condition, ceilings and rarity color were **verified in live gameplay** on build 1.0.40, but a future game update can move method signatures and silently disable a hook — the mod logs every patch at startup so you can tell what is active.
- Every economy feature is an independent toggle (`EnableSellCap`, `EnableLaborCost`, `EnableDonorCondition`, `EnableMaxQuality`) — turn any of them off to soften the challenge.

## The Condition Economy (v0.3.11)

v0.3.11 changes what a repair is *worth*. Before, the money you got for selling a car barely cared about how good the car was, while every repair cycle cost the same flat fee — so restoring a 70% body panel was a guaranteed loss, and the rational play was to flip the car untouched. The new model follows how real restoration works: **the market pays for condition, and the last percents are the expensive ones.**

Three formulas now drive the loop:

| Lever | Formula | Default |
| --- | --- | --- |
| Car sale price | `baseCarPrice × (CarSaleConditionFloor + CarSaleConditionSpan × condition)` | `0.5 + 0.5 × cond` |
| Part resale | `shopPrice × SellRatio × (SellConditionFloor + SellConditionSpan × condition)` | `0.35 × (0.4 + 0.6 × cond)` |
| Repair labor | `LaborCostBase × grade × (1 + LaborCurveK × cond / (1 − cond))` | `k = 0.25`, cond capped at 98% |

- **A restored car sells for more than the same car in rust.** A car at 10% condition sells for ~60% of its base value; at 90% — for ~95%. Condition is what the buyer pays for, exactly like in real life.
- **Labor is a hyperbola, not a flat fee.** Lifting a part from 10% to 40% costs ~1× the base fee per cycle; pushing it from 90% toward 95% costs ~3.5× per cycle. The first cycles of a tired part are cheap, the last cycles near the ceiling are a luxury.
- **The optimum is not 100%.** Because labor grows hyperbolically while the sale price grows linearly, the best margin sits around **70%** condition. Pushing a panel from 70% to 99% is a deliberate, expensive choice — a trophy, not a chore. That is the golden mean: no grind on the cheap cycles, no free money on the expensive ones.
- **The repair button tells the truth.** The Start button now shows the next cycle: `+3%  -1250` — the condition gain and the full price (game repair price + labor) before you commit. A decision you can see is a real decision. Since 0.3.11-dev4 the button is **green** while the cycle pays for itself, **red** once the golden mean is behind you, and keeps its **normal game color** for parts that are not money-makers at any condition — red now means "you crossed the line", never "this part is worthless".
- **A green `$` marks the golden mean.** Under the `!`/`▲` corner marker, a part shows a green `$` while repairing it is still profitable — the next cycle adds more value than it costs, through the part's own resale price (junkyard find → repair → sell) or through a car that needs it. The `$` disappears the moment you cross the line into spending more than you gain. No `$` at the restoration ceiling (nothing left to fix).
- **Failure still destroys the part.** Risk was left untouched on purpose: a ruined panel is lost for good, so what you find and keep has real value.

Everything is toggleable: `EnableCarSaleCondition`, `EnableSellConditionFactor`, `EnableLaborCurve` (with `LaborCurveK`, `LaborCurveMaxCond`), `EnableRepairCostPreview`, `EnableProfitMarker`.

## Last update v 0.3.11-dev6 (in development, unreleased)

1. **The auction sells retro only.** The auction's car pool is now filtered by the same `RemoveCarsOlderThanYear` cutoff as the junkyard, and the generated auction list is filtered once more as a safety net. With the default `2005`, the auction draws from the **101** pre-2005 cars the game allows there (out of 163), so the list stays full — and every car you can bid on is a car the shop no longer supplies parts for. Combined with the one-star cap, the auction becomes what the loop needs it to be: a source of old, tired machines.
2. **Version 0.3.11-dev6.** No new config key: `RemoveCarsOlderThanYear` stays the single switch, exactly as for the junkyard filter.

Note: each car's year is read from its own `config.txt` first (base game and DLC alike), falling back to the game's own year API — the same rule that made the junkyard filter robust in v0.3.7. The hooks log `[Auction] pool year filter: cutoff=2005 kept=... removed=...` when the auction builds its pool and `[Auction] list year filter: type=Normal cutoff=2005 kept=... removed=...` for every generated list; if a list would end up empty it is left untouched and a `Warning` is logged, so the auction screen is never offered empty. Verified by build; live acceptance: those log lines and no car from the cutoff year or newer in the auction list.

## Last update v 0.3.11-dev7 (in development, unreleased)

1. **The dealership shows only modern cars.** The showroom grid is now filtered by the same `RemoveCarsOlderThanYear` cutoff as the junkyard and auction: every car whose year is below the cutoff is hidden, so the shop offers only cars from the cutoff year and newer. The year is now resolved from each car's own id via its `config.txt` (base game and DLC alike), falling back to the year in the car's name. The old code read the year only from the name text — which the game fills in lazily and which carries no year for some cars — and it only ever hid a card, never restored one, so a card the filter mis-served (name not yet populated, year read as `-1`) was hidden and the shelf ended up showing the wrong cars. Reading the real year from the car's id removes that dependency: a car is hidden only when its true year is below the cutoff, and a modern car is never hidden.
2. **Version 0.3.11-dev7.** No new config key: `RemoveCarsOlderThanYear` stays the single switch, exactly as for the junkyard and auction filters.

Note: the filter runs on the live `ShopCarWindow` grid each frame and hides only cars it can confirm are older than the cutoff (a readable year below the cutoff); a car it cannot date is left as the game shows it, so a modern car is never hidden by a missing year. Verified by build; live acceptance: open the dealership — no car older than the cutoff is on the shelf, and modern cars (including those whose name has no year) are all present.

## Last update v 0.3.11-dev5 (in development, unreleased)

1. **The auction stops handing out winners.** Every car at the auction carries a star rating, and the game rolls it per car (`AuctionHelper.GetRatingForCar`): regular auctions draw good and very good machines, salvage ones draw mid-grade wrecks. The mod clamps that roll to `AuctionMaxRating` (default **1**). The rating is the game's own source for the car's value, its starting price, the AI's maximum bid and the condition the car is generated in, so capping it in one place makes the whole auction — regular, salvage and the additional one — a one-star yard: the cheapest, most tired machines, priced like them.
2. **Version 0.3.11-dev5.** Toggles: `EnableAuctionRatingCap` (on by default), `AuctionMaxRating` (default `1`; set it to `2` to allow two stars, `5` to return to vanilla).

Note: the cap is a postfix on the game's own rating roll, so it applies to every car the auction generates and to every re-roll of the list, and it never leaves a car behind that the UI shows with more stars than the price was computed for. The hook logs `[Auction] hook active: GetRatingForCar cap=1` on the first roll and `[Auction] cap type=Normal rating 4 -> 1` for each capped car, so a game update that moves the method is visible in the log. Verified by build; live acceptance: open the auction, every card shows a single star, and the log shows the `[Auction] cap ...` lines.

## Last update v 0.3.11-dev4 (in development, unreleased)

1. **Green `$` profit marker.** Part cards (inventory, junkyard, repair list) show a green `$` under the need/upgrade corner marker while the *next repair cycle is profitable*: the condition it adds is worth more than the cycle costs — via the part's own resale value (`shopPrice × SellRatio × SellConditionSpan × Δ`, the junkyard-find-repair-sell loop) or via a car that needs it (`baseCarPrice × CarSaleConditionSpan × Δ / partCount`). The marker vanishes past the golden mean and at the ceiling. The car channel needs no Sell-tab visit: the mod reads the car's price from the save profile (`CarInfoData.BuyPrice`) and refines it with the live `baseCarPrice` once the Sell tab has been opened.
2. **The Start button is color-coded in three states.** Green while the next cycle pays for itself; red only when the part *was* profitable at low condition and the next cycle crosses the line (the golden mean is behind you); the game's own color when the part never pays for itself through resale (a cheap panel repaired for the car, not for money) — no more all-red noise on parts where every cycle is a resale loss by construction.
3. **The Sell tab shows the price the game actually pays.** Live evidence (logs + screenshots) proved the game pays `parts value + body value + tuning + condition bonus` through its own sell coroutine (`_SellCarCoroutine`), while the mod's Sell-tab line overwrite showed a different, formula-based number (7,984 vs the real 14,848). The mod no longer overwrites the car sale display — line, dialog and payout are one and the same game number; the mod keeps caching the car's base price for the `$` marker.
4. **Version 0.3.11-dev4.** Dev suffix per the versioning rule (release number bumps only after merge to `main`). Toggle: `EnableProfitMarker`.

Note: the marker is advisory, not accounting — the game's exact weighting of one part inside the car's global condition is not exposed, so the mod uses `1/partCount` and an estimated 15% cycle step. It marks the *zone*, not the last kopeck. Verified by build.

## Last update v 0.3.11

1. **Car sale price now scales with condition.** A new hook on the car Sell tab recalculates the price as `baseCarPrice × (0.5 + 0.5 × globalCondition)`; the sell-value and profit lines are rewritten to match. Restoring a car is now the way to turn labor into money — flipping it "as is" stays viable but pays less. *(Superseded in 0.3.11-dev3: the game pays its own formula — parts + body + tuning + condition bonus — and the mod no longer overwrites the car sale display; see the dev3 entry.)*
2. **Part resale pays for condition.** The sell cap is now `SellRatio × (0.4 + 0.6 × condition)`: a restored part sells for up to ~2.4× more than the same part in rust. The anti-flip cap stays; the reward for quality appears.
3. **Labor cost is a curve.** Each successful repair charges `LaborCostBase × grade × (1 + 0.25 × cond/(1 − cond))` (cond capped at `LaborCurveMaxCond` = 0.98): cheap early cycles, expensive late cycles.
4. **Repair preview on the Start button.** The button now reads `+Δ%  -total` — the condition the next cycle adds and its full price (game repair price + labor) — before you start the mini-game.
5. **Version 0.3.11.**

Note: the car-sale hook is new; it logs `[CarSale] cond=... base=... game=... new=...` every time the Sell tab opens, so a future game update that moves the method is visible in the log. All hooks log at startup like the rest of the mod.

## Last update v 0.3.10

1. **The junkyard now flags what your cars need.** Every part card in the junkyard and inventory is checked against the cars you actually own — the ones in the garage *and* the ones you just bought and parked. Two corner markers were added:
   - **Orange `!`** — this part is **missing** on one of your cars (it sits unmounted somewhere), so it is worth picking up.
   - **Green `▲`** — you **already have** this part fitted, but the copy on the junkyard is in **better current condition** than the best copy installed on any of your cars — a genuine upgrade you can swap in.
   The "already have it" check takes the **best** installed copy across all your cars, so a car you just bought at the junkyard counts too (it is picked up within a few seconds in the same session).

<p align="center">
  <img src="/Assets/new_junk_list_items.png" alt="Junkyard cards with rarity colors, current-max condition ranges and an orange ! on a missing part">
</p>
<p align="center"><em>Found items: the card color shows the restoration ceiling (green/purple), the bar reads <code>current-max%</code> (current repair value vs. the part's hidden maximum), and the orange <code>!</code> marks a part your car is missing.</em></p>

<p align="center">
  <img src="/Assets/can_be_upgrade.png" alt="Junkyard card with a green up-arrow meaning a better-condition copy of a part you already own">
</p>
<p align="center"><em>The green <code>▲</code> means you already own this part, but a better-condition copy is available — an upgrade you can grab.</em></p>

2. **Build quality now really drives the car.** Because no part can be restored to a flawless 100%, a car's overall condition — and with it its handling, braking and drivability — can no longer be maxed out by grinding every part back to perfect. A truly perfect build is now the rare exception rather than the default: the best you can do is hunt down high-ceiling survivors and fit them well. A well-restored old car should feel restored, not factory-new — and that is exactly what the ceilings enforce.

Note: the junkyard need/upgrade markers are always on (no config flag) and read your cars from the saved profile, so a car bought at the junkyard is included in the same session. Verified in live gameplay on build 1.0.40.

## Last update v 0.3.9

1. **Restoration ceilings (the big one).** Parts can no longer be ground to a perfect 100%. Every part gets a hidden maximum condition it can be restored to, rolled once from its id and stable across saves. Most parts cap at 30–50%, some at 50–70%, a few at 70–100%. A part at its ceiling stops improving, leaves the repair list, and shows a `MAX xx%` marker; below it the card shows `current-max%`. Toggle with `EnableMaxQuality`.
2. **Rarity is now the ceiling.** Rarity is no longer a config tag — it is simply how high a part can be restored, and the card color follows it: grey (30–50%), green (50–70%), purple (70–100%). Odds are configurable (`MaxQualityCommonWeight/RareWeight/UniqueWeight`, default 65/25/10) and so are the ranges. The old `year|part_id|rarity` token is ignored, and shop prices for hidden retro parts are now era-based only.
3. **Repair fix.** Repairs now key off the part's *displayed* condition, so body panels and previously "stuck" parts (hoods, doors) repair correctly and are added to the repair window as intended.
4. **Labor cost follows the new grade.** Green parts cost 2.5× and purple 4× the base labor; common 1×.
5. **Starting balance.** A new game now starts with `StartingMoney` (default **20000**), applied reliably at new-game creation. The cushion is deliberate: with resale capped, labor billed on every repair and junkyard donors handing over tired parts, the opening economy is genuinely tighter — so 20000 gives you a real first move: buy the **welding machine** and the **pressure washer**, and still have enough to **pull a cheap donor car off the junkyard** and start a first build, instead of stalling broke in the first hour.
6. **Marker polish.** The at-ceiling marker reads `MAX xx%` (with the percentage); below-ceiling cards read `current-max%`.

7. **Bodywork is restorable.** Body panels (doors, hoods, fenders, bumpers, ...) are the one place the ceiling bends: because you can cut, weld and patch metal, every body part rolls its own ceiling in `MaxQualityBodyMin`–`MaxQualityBodyMax` (default **50–100%**), so panels vary from tired to as-new instead of all topping out at one number, and its card color follows that true ceiling. Engines, drivetrain and electrics keep the hard ceiling — worn internals stay worn. The "a junkyard car is never perfect" promise holds where it matters, while a rusted shell can genuinely be resurrected. To keep green meaningful as a value signal, body panels are **not** tinted green — a green-tier panel reads as ordinary, and only purple bodywork (71–100%) glows.

Note: `EnableMaxQuality` (default on) is the master switch for ceilings and the new color-by-ceiling; set it to `false` to return to vanilla 100% repairs and drop the coloring. Verified in live gameplay on build 1.0.40. The starting-balance override was confirmed in live gameplay: a new game now opens at the configured `StartingMoney`.

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
