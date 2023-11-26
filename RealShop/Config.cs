using CMS.UI.Logic.Paging;
using MelonLoader;
using System.Net.NetworkInformation;
using UnityEngine;
using VehiclePhysics;
using static System.Net.WebRequestMethods;

namespace RealShop
{
    public class Config
    {
        public const string ModName = "RealShop";

        public static string[] ListWhiteCarParts = { 
            "I4 DT224", "I4 204PT", "V8 AJ133", "V8 392 Hemi", "V10 OHV MT8", "V10 OHV", "V12 AM11", "B6 MA", "B6 M64", "eDen-1H",
            "Proton e-R", "Proton e-R","I4 DOHC BFM", "I4 SOHC ABT", "V6 DOHC VQ37VHR", "V6 VQ37VHR", "V8 M177 DBX", "V8 J",
            "V6 AJD", "Zonda", "V12 M120", "V12 M158", "CGM", "DBS", "DBX", "Avalanche", "991", "GTR", "330",
            "V8 AJ133", "F350", "Hellcat", "Crown", "Discovery", "EarthQuake", "Earthquake Rex", "Evoque",
            "Grand Club", "GW500", "Imperator", "M3", "Mojave", "Moon", "Mosquito", "Piccolo",
            "SVR", "Town", "Tyro", "Urs",
        };

        public static string[] RetroParts = {};

        public static string[] RetroParts1950_1959 = {
            // DELRAY CUSTOM (1957)
            "car_delraycustom", 

            // SALEM 1500 (1955)
            "car_salem1500", 

            // Engine V8 2carb OHV || (V8 2carb OHV)
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_2carb_OHV
            "v8_blok_stary", "v8_filtr_oleju_stary", "v8_gearbox_stary", "v8_glowica_stara", "v8_kolektor_wydechowy_stary_1", "v8_kolo_pasowe_walu_stare",
            "v8_lancuch_stary", "v8_miska_olejowa_stara", "v8_pasek_stary_1", "v8_pasek_stary_2", "v8_pokrywa_glowicy_stara_1", "v8_pokrywa_glowicy_stara_2",
            "v8_pompaWody_stara", "v8_rolkaWalka_stara", "v8_rozrzad_oslona_stara", "v8_walek_stary", "v8_walKorbowy_stary", "w_v8_koncowy_stary",
            "w_v8_poczatkowy_stary", "w_v8_poczatkowy_stary_h1", "w_v8_poczatkowy_stary_h2", "w_v8_poczatkowy_stary_h3", "wentylator_2",
            "v8_supercharger_stary", "v8_supercharger_kolektor_dolotowy_stary", "v8_supercharger_pasek_stary", "rolka_pompy_wody_supercharger",
            "v8_filtr_1", "v8_scoop_1", "v8_gearbox_stary3", "t_w_v8_poczatkowy_stary", "t_v8_kolektor_wydechowy_stary_1", "t_v8_glowica_stara",
            "t_w_v8_koncowy_stary", "t_v8_walek_stary", "t_w_v8_poczatkowy_stary_h2", "t_w_v8_poczatkowy_stary_h3",
            "t_v8_supercharger_stary", "t_v8_supercharger_kolektor_dolotowy_stary", "t_w_v8_poczatkowy_stary_h1", "t_v8_filtr_1",
            "t_v8_gearbox_stary", "t_v8_gearbox_stary3",

            // (V8 OHV F)
            "v8_zz_blok", "v8_zz_filtr_dol", "v8_zz_filtr_gora", "v8_zz_filtr_srodek", "v8_zz_gaznik", "v8_zz_gearbox", "v8_zz_glowica",
            "v8_zz_kolektor_dolotowy", "v8_zz_kolektor_wydechowy", "v8_zz_lancuch", "v8_zz_miska_olejowa", "v8_zz_oslona_rozrzadu", "v8_zz_pasek_1",
            "v8_zz_pokrywa_glowicy_1", "v8_zz_pokrywa_glowicy_2", "v8_zz_pompa_wody", "v8_zz_pompa_wspomagania", "v8_zz_popychacz", "v8_zz_raczka_1",
            "v8_zz_raczka_2", "v8_zz_walek_popychaczy", "v8_zz_walKorbowy", "w_v8_zz_poczatkowy", "t_v8_zz_gaznik", "t_v8_zz_pompa_wspomagania","t_v8_zz_filtr_srodek",
            "t_v8_zz_glowica", "t_v8_zz_kable", "t_v8_zz_kolektor_wydechowy", "t_w_v8_zz_poczatkowy", "t_v8_zz_kolektor_dolotowy", "t_v8_zz_walek_popychaczy",
            "t_v8_zz_gearbox", "v8_zz_kable",

            // Engine (V12)
            "v12_blok", "v12_gearbox", "v12_glowica_1", "v12_glowica_2", "v12_glowica_wkladka", "v12_kolektor_dolotowy", "v12_kolektor_wydechowy_1",
            "v12_kolektor_wydechowy_2", "v12_listwa_wtryskowa", "v12_miska_olejowa", "v12_oslona_lancucha", "v12_oslona_rozrzadu_1", "v12_oslona_rozrzadu_2",
            "v12_oslona_rozrzadu_3", "v12_lancuch_1", "v12_pasek_1", "v12_pasek_2", "v12_pokrywa_glowicy_1", "v12_pokrywa_glowicy_2", "v12_pompa_wspomagania",
            "v12_pompaWody", "v12_przepustnica", "v12_slizg_1", "v12_slizg_2", "v12_termostat", "v12_walek_rozrzadu", "v12_walKorbowy", "v12_filtr_oleju",
            "v12_gearbox_2", "v12_walek_rozrzadu_2", "v12_kolektor_wydechowy_3", "t_v12_glowica_1", "t_v12_glowica_2", "t_v12_walek_rozrzadu",
            "t_v12_kolektor_wydechowy_1", "t_v12_kolektor_wydechowy_2", "t_v12_listwa_wtryskowa", "t_v12_przepustnica", "t_v12_pompa_wspomagania",
            "t_v12_kolektor_dolotowy", "t_v12_kolektor_wydechowy_3", "t_v12_walek_rozrzadu_2", "t_v12_gearbox", "t_v12_gearbox_2",
        };

        public static string[] RetroParts1960_1969 = {
            // Fairlady Z (S30) 1969
            "car_nissan240z-bumper_front", "car_nissan240z-bumper_rear", "car_nissan240z-door_front_left", "car_nissan240z-door_front_right", "car_nissan240z-fender_front_left",
            "car_nissan240z-fender_front_right", "car_nissan240z-headlight_left", "car_nissan240z-headlight_right", "car_nissan240z-hood", "car_nissan240z-taillight_left",
            "car_nissan240z-taillight_right", "car_nissan240z-trunk", "car_nissan240z-window_front", "car_nissan240z-window_trunk", "car_nissan240z-window_body_left_1",
            "car_nissan240z-window_body_right_1", "car_nissan240z-window_door_front_left", "car_nissan240z-window_door_front_right", "car_nissan240z-mirror_left",
            "car_nissan240z-mirror_right", 

            // ENGINE Fairlady Z I6 SOHC L24 (I6 L24) 
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_SOHC_L24
            "r6_240z_blok", "r6_240z_gearbox", "r6_240z_glowica", "r6_240z_dzwignia_zaworu", "r6_240z_filtr_dol", "r6_240z_filtr_gora",
            "r6_240z_filtr_srodek", "r6_240z_gaznik", "r6_240z_kolektor_dolotowy", "r6_240z_kolektor_wydechowy", "r6_240z_lancuch", "r6_240z_miska_olejowa",
            "r6_240z_oslona_rozrzadu_1", "r6_240z_oslona_rozrzadu_2", "r6_240z_pasek", "r6_240z_pokrywa_glowicy", "r6_240z_pompa_wody", "r6_240z_raczka",
            "r6_240z_rolka_walka", "r6_240z_slizg", "r6_240z_walek_rozrzadu", "r6_240z_walKorbowy", "r6_240z_wentylator", "r6_240z_wkladka",
            "w_r6_240z_poczatkowy", "t_r6_240z_filtr_srodek", "t_r6_240z_kable", "t_r6_240z_kolektor_wydechowy", "t_r6_240z_glowica", "t_w_r6_240z_poczatkowy",
            "t_r6_240z_gaznik", "t_r6_240z_kolektor_dolotowy", "t_r6_240z_walek_rozrzadu", "t_r6_240z_gearbox", "r6_240z_kable", 

            // Mercedes-Benz W113 Pagoda
            "seat_pagoda", "steering_wheel_pagoda", "car_mercedespagoda-bumper_front",  "car_mercedespagoda-bumper_rear", "car_mercedespagoda-door_front_left",
            "car_mercedespagoda-door_front_right", "car_mercedespagoda-headlight_left", "car_mercedespagoda-headlight_right", "car_mercedespagoda-hood",
            "car_mercedespagoda-mirror_left", "car_mercedespagoda-taillight_left", "car_mercedespagoda-taillight_right", "car_mercedespagoda-trunk",
            "car_mercedespagoda-window_door_front_left", "car_mercedespagoda-window_door_front_right",  "car_mercedespagoda-window_front",
            "car_mercedespagoda-window_body_left_1", "car_mercedespagoda-window_body_right_1", "car_mercedespagoda-window_back", "rim_mercedes_benz_pagoda",

            // Mercedes-Benz W113 Pagoda || Engine  I6 M130
            "r6_m130_lancuch", "r6_m130_walKorbowy", "r6_m130_pasek_1", "r6_m130_dzwignia_zaworu", "r6_m130_pompa_wody", "r6_m130_kolektor_dolotowy",
            "r6_m130_filtr_dol", "r6_m130_pasek_2", "r6_m130_rolka_lancucha", "r6_m130_glowica", "r6_m130_pokrywa_glowicy", "r6_m130_kolektor_wydechowy",
            "r6_m130_blok", "r6_m130_gearbox","r6_m130_filtr_gora", "r6_m130_walek_rozrzadu", "r6_m130_glowica_wkladka", "r6_m130_oslona_rolki",
            "r6_m130_wtrysk", "r6_m130_miska_olejowa", "r6_m130_filtr_srodek", "r6_m130_rura_dolotowa", "w_r6_m130_koncowy_pagoda", "w_r6_m130_poczatkowy_pagoda",
            "t_w_r6_m130_poczatkowy_pagoda", "t_r6_m130_filtr_srodek", "t_r6_m130_kable", "t_r6_m130_glowica", "t_r6_m130_kolektor_wydechowy",
            "t_r6_m130_kolektor_dolotowy", "t_w_r6_m130_koncowy_pagoda", "t_r6_m130_walek_rozrzadu", "t_r6_m130_wtrysk", "t_r6_m130_gearbox", "r6_m130_kable",


            // SMITH&PARKER ARIZONA KID (1969)
            "car_smithandparkerarizonakid", "car_smithandparkerarizonakid-headlight_right",  "car_smithandparkerarizonakid-headlight_left", "car_smithandparkerarizonakid-taillight_left",
            "car_smithandparkerarizonakid-taillight_right", "car_smithandparkerarizonakid-door_front_left", "car_smithandparkerarizonakid-door_front_right",
            "car_smithandparkerarizonakid-trunk", "car_smithandparkerarizonakid-hood", "car_smithandparkerarizonakid-fender_front_right", "car_smithandparkerarizonakid-fender_front_left",
            "car_smithandparkerarizonakid-fender_rear_right", "car_smithandparkerarizonakid-fender_rear_left", "car_smithandparkerarizonakid-bumper_front",
            "car_smithandparkerarizonakid-window_front", "car_smithandparkerarizonakid-window_back", "car_smithandparkerarizonakid-window_door_front_left",
            "car_smithandparkerarizonakid-window_door_front_right", "car_smithandparkerarizonakid-mirror_left", "car_smithandparkerarizonakid-mirror_right", 

            // SMITH&PARKER ARIZONA OLD DADDY T (1962)
            "car_smithandparkerolddaddyt",

            // LUXOR GOLDTRAIL (1965)
            "car_luxorgoldtrail", "steering_wheel_goldtrail", "bench_goldtrail1", "bench_goldtrail2",

            // LUXOR BOWEN (1965)
            "car_luxorbowen", "steering_wheel_bowen", "bench_bowen1", "bench_bowen2", "rim_bowen",

            // bolt cape mk 4 (1965)
            "car_boltcape", "steering_wheel_cape", "bench_cape1", "bench_cape2", "car_boltcape-taillight_right2", "rim_cape",

            // bolt atlanta (1969)
            "car_boltatlanta", "seat_atlanta", "steering_wheel_atlanta", "rim_atlanta",

            // bolt supernova (1969)
            "car_boltsupernova", "seat_supernova", "seat_supernova2", "seat_supernova3", "bench_supernova", "rim_supernova",

            // chieftain tbx (1969)
            "car_chieftaintbx", "seat_tbx", "steering_wheel_tbx", "bench_tbx", "rim_tbx",

            // SALEM SPECTRE FASTBACK (1969)
            "car_salemspectrefastback", "bench_fastback", "bench_fastback_leather1", "bench_fastback_leather2",

            // TEMPEST MAGNUM (1968)
            "car_tempestmagnum", "bench_magnum", "bench_magnum_leather1", "bench_magnum_leather2",

            // ZHONGHUA XIONGYING (1966)
            "car_zhonghuaxiongying", 

            // RETRO TIRES
            "tire_vintage", "tire_vintage_2", "tire_vintage_wall1", "tire_vintage_wall2", "tire_vintage_wall3", "tire_vintage_race", "tire_vintage_race_2",

            // RETRO RIMS
            "rim_5", "rim_4", "rim_12", "rim_13", "rim_21", "rim_24", "rim_15", "rim_25", "rim_31", "rim_5B", "rim_11B", "rim_20B", "rim_25B", "rim_4B", "rim_11C",

            // Engine (V8  OHV) || V8 1carb OHV or V8 2carb OHV or V8 2carb OHV Supercharged || (V8 OHV) 1965 ?
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_1carb_OHV
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_2carb_OHV
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_2carb_OHV_Supercharged
            "v8_blok_stary", "v8_filtr_oleju_stary", "v8_gearbox_stary", "v8_glowica_stara", "v8_kolektor_wydechowy_stary_1",
            "v8_kolo_pasowe_walu_stare", "v8_lancuch_stary", "v8_miska_olejowa_stara", "v8_pasek_stary_1", "v8_pasek_stary_2",
            "v8_pokrywa_glowicy_stara_1", "v8_pokrywa_glowicy_stara_2", "v8_pompaWody_stara", "v8_rolkaWalka_stara", "v8_rozrzad_oslona_stara",
            "v8_walek_stary", "v8_walKorbowy_stary", "w_v8_koncowy_stary", "w_v8_poczatkowy_stary", "w_v8_poczatkowy_stary_h1",
            "w_v8_poczatkowy_stary_h2", "w_v8_poczatkowy_stary_h3", "wentylator_2", "v8_supercharger_stary", "v8_supercharger_kolektor_dolotowy_stary",
            "v8_supercharger_pasek_stary", "rolka_pompy_wody_supercharger", "v8_filtr_1", "v8_scoop_1", "v8_gearbox_stary3",
            "t_w_v8_poczatkowy_stary", "t_v8_kolektor_wydechowy_stary_1", "t_v8_glowica_stara", "t_w_v8_koncowy_stary",
            "t_v8_walek_stary", "t_w_v8_poczatkowy_stary_h2", "t_w_v8_poczatkowy_stary_h3", "t_v8_supercharger_stary", "t_v8_supercharger_kolektor_dolotowy_stary",
            "t_w_v8_poczatkowy_stary_h1", "t_v8_filtr_1", "t_v8_gearbox_stary", "t_v8_gearbox_stary3",

            // V8 OHV 6P || 1968
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_OHV_6P
            "v8_sixpack_filtr_dol", "v8_sixpack_filtr_gora", "v8_sixpack_filtr_srodek", "v8_sixpack_gaznik_1", "v8_sixpack_gaznik_2", "v8_sixpack_kolektor_dolotowy",
            "t_v8_sixpack_gaznik_1", "t_v8_sixpack_gaznik_2", "t_v8_sixpack_filtr_srodek", "t_v8_sixpack_kolektor_dolotowy", 

            // V8 OHV SS
            // The V8 OHV SS is based off the real life Chevrolet W-series big block 409 engine produced from 1961 to 1965.
            "v8_409_blok", "v8_409_filtr_gora", "v8_409_glowica_1", "v8_409_glowica_2", "v8_409_kolektor_dolotowy", "v8_409_kolektor_wydechowy",
            "v8_409_miska_olejowa", "v8_409_oslona_rozrzadu", "v8_409_pasek_1", "v8_409_pasek_2", "v8_409_pokrywa_glowicy_1", "v8_409_pokrywa_glowicy_2",
            "v8_409_pompa_wody", "v8_409_raczka_1", "v8_409_tlok", "v8_409_walek_popychaczy", "v8_409_walKorbowy", "v8_409_tlok_pierscienie_1",
            "v8_409_gaznik_1", "v8_409_gaznik_2", "w_v8_409_poczatkowy", "t_w_v8_409_poczatkowy", "t_v8_409_gaznik_1", "t_v8_409_gaznik_2",
            "t_v8_409_kolektor_wydechowy", "t_v8_409_glowica_1", "t_v8_409_glowica_2", "t_v8_409_kable", "t_v8_409_kolektor_dolotowy",
            "t_v8_409_tlok", "t_v8_409_walek_popychaczy", "v8_409_kable", 


            // Engine I6 OHV || (I6 B)
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_OHV
            // The I6 OHV was based on the fourth generation of the Ford I6 OHV introduced in 1965,
            // this was a 4.6L (or 300 cu in) engine producing 150hp and 353N.m of torque.
            "i6_old_blok", "i6_old_filtr_dol", "i6_old_filtr_gora", "i6_old_filtr_srodek", "i6_old_gearbox", "i6_old_glowica", "i6_old_kolektor_dolotowy", "i6_old_kolektor_wydechowy",
            "i6_old_lancuch", "i6_old_miska_olejowa", "i6_old_oslona_rozrzadu", "i6_old_pasek1", "i6_old_pasek_2", "i6_old_pokrywa_glowicy", "i6_old_pompaWody",
            "i6_old_pompaWspomagania", "i6_old_popychacz", "i6_old_przepustnica", "i6_old_raczka1", "i6_old_raczka2", "i6_old_rolkaWalka", "i6_old_termostat",
            "i6_old_walek", "i6_old_walKorbowy", "w_i6_old_poczatkowy", "t_i6_old_kable", "t_i6_old_filtr_srodek", "t_i6_old_glowica", "t_i6_old_alternator",
            "t_i6_old_kolektor_dolotowy", "t_i6_old_kolektor_wydechowy", "t_i6_old_pompaWspomagania",  "t_i6_old_przepustnica", "t_i6_old_termostat",
            "t_i6_old_walek", "t_w_i6_old_poczatkowy", "t_i6_old_gearbox", "i6_old_alternator", "i6_old_kable",

            // V8 OHV MAG || V8 OHV F // 
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_OHV_MAG
            "v8_zz_blok", "v8_zz_filtr_dol", "v8_zz_filtr_gora", "v8_zz_filtr_srodek", "v8_zz_gaznik", "v8_zz_gearbox", "v8_zz_glowica", "v8_zz_kolektor_dolotowy",
            "v8_zz_kolektor_wydechowy", "v8_zz_lancuch", "v8_zz_miska_olejowa", "v8_zz_oslona_rozrzadu", "v8_zz_pasek_1", "v8_zz_pokrywa_glowicy_1", 
            "v8_zz_pokrywa_glowicy_2", "v8_zz_pompa_wody", "v8_zz_pompa_wspomagania", "v8_zz_popychacz", "v8_zz_raczka_1", "v8_zz_raczka_2",
            "v8_zz_walek_popychaczy", "v8_zz_walKorbowy", "w_v8_zz_poczatkowy", "t_v8_zz_gaznik", "t_v8_zz_pompa_wspomagania", "t_v8_zz_filtr_srodek",
            "t_v8_zz_glowica", "t_v8_zz_kable", "t_v8_zz_kolektor_wydechowy", "t_w_v8_zz_poczatkowy", "t_v8_zz_kolektor_dolotowy", "t_v8_zz_walek_popychaczy",
            "t_v8_zz_gearbox", "v8_zz_kable",

            //         ... (V8 OHV C)
            "v8_hemi_blok", "v8_hemi_miska_olejowa", "v8_hemi_oslona_rozrzadu", "v8_hemi_pasek_1", "v8_hemi_pasek_2", "v8_hemi_pompa_wody",
            "v8_hemi_raczka_1", "v8_hemi_raczka_2", "v8_hemi_tlok", "v8_hemi_walek_popychaczy", "v8_hemi_walKorbowy", "v8_hemi_tlok_pierscienie_1",
            "t_v8_hemi_tlok","t_v8_hemi_walek_popychaczy",

            //         ... (V8 OHV D)
            "v8_magnum_dzwignia_zaworu", "v8_magnum_filtr_gora", "v8_magnum_glowica_1", "v8_magnum_glowica_2", "v8_magnum_kolektor_dolotowy",
            "v8_magnum_kolektor_wydechowy", "v8_magnum_pokrywa_glowicy_1", "v8_magnum_pokrywa_glowicy_2", "w_v8_magnum_poczatkowy",
            "t_v8_magnum_kolektor_wydechowy", "t_w_v8_magnum_poczatkowy", "t_v8_magnum_glowica_1", "t_v8_magnum_glowica_2", "t_v8_magnum_kable",
            "t_v8_magnum_kolektor_dolotowy", "v8_magnum_kable",

        };

        public static string[] RetroParts1970_1979 = {
            // Smith&Parker Arizona Kid Supercharged ???
            // https://car-mechanic-simulator-2021.fandom.com/wiki/Smith%26Parker_Arizona_Kid_Supercharged


            // SMITH&PARKER OUTLAW (1971)
            "car_smithandparkeroutlaw",

            // SMITH&PARKER ARIZONA ELIMINATOR (1977)
            "car_smithandparkereliminator", 

            // PLYMOUTH ROADRUNNER
            "car_plymouthroadrunner", "bench_roadrunner1", "bench_roadrunner2", "rim_roadrunner",

            // FORD MUSTANG MACH 1 (1971)
            "car_fordmustangmach1", "car_fordmustangmach1-headlight_left", "car_fordmustangmach1-headlight_right", "car_fordmustangmach1-door_front_left",
            "car_fordmustangmach1-door_front_right", "car_fordmustangmach1-taillight_right", "car_fordmustangmach1-taillight_left", "car_fordmustangmach1-bumper_front",
            "car_fordmustangmach1-bumper_rear", "car_fordmustangmach1-window_front", "car_fordmustangmach1-window_back", "car_fordmustangmach1-window_door_front_left",
            "car_fordmustangmach1-window_door_front_right", "car_fordmustangmach1-mirror_left", "car_fordmustangmach1-mirror_right", "car_fordmustangmach1-fender_front_left",
            "car_fordmustangmach1-fender_front_right", "car_fordmustangmach1-hood", "car_fordmustangmach1-trunk", "car_fordmustangmach1-window_body_left_1",
            "car_fordmustangmach1-window_body_right_1", "seat_mach", "steering_wheel_mach", "rim_mach",

            // EDGEWOOD HELLCAT (1970)
            "car_edgewoodhellcat", "bench_hellcat", "bench_hellcat_leather1", "bench_hellcat_leather2",

            // DOODGE CHALLENGER R/T (1970)
            "car_dodgechallenger", "w_koncowy_challenger", "seat_challenger", "seat_challenger_srt_hellcat",
            "steering_wheel_challenger_srt_hellcat", "bench_challenger", "bench_challenger_srt_hellcat", 
            "t_w_koncowy_challenger", "rim_challenger_hellcat_1", "rim_challenger_hellcat_2",

            // BOLT REPTILIA (1970)
            "car_boltreptiliar", "seat_reptiliar", "steering_wheel_reptiliar", "rim_reptiliar",

            // EDGEWOOD WILDCAT (1971)
            "car_edgewoodwildcat", "seat_wildcat", "seat_wildcat2", "seat_wildcat3", "bench_wildcat",
            "rim_wildcat",

            // DELRAY WINCHESTER (1971)
            "car_delraywinchester", 

            // BOLT ROLLETT (1971)
            "car_boltrollett", "seat_rollett", "steering_wheel_rollett", "bench_rollett", "rim_rollett",

            // BOLT CHAPMAN (1978)
            "car_boltchapman", "steering_wheel_chapman", "bench_chapman1", "bench_chapman2", "rim_chapman",

            // CHIEFTAIN BANDIT (1978)
            "car_chieftainbandit", "seat_bandit", "steering_wheel_bandit", "bench_bandit",

            // CHRYSLER CORDOBA (1978)
            "car_chryslercordoba", "w_v8_360_poczatkowy_chryslercordoba_1", "w_v8_360_poczatkowy_chryslercordoba_2", "w_v8_360_koncowy_chryslercordoba",
            "seat_chrysler_cordoba", "steering_wheel_chrysler_cordoba", "bench_chrysler_cordoba", "t_w_v8_360_koncowy_chryslercordoba", 
            "t_w_v8_360_poczatkowy_chryslercordoba_1", "t_w_v8_360_poczatkowy_chryslercordoba_2", "rim_cordoba",

            // ENGINE to CHRYSLER CORDOBA (1978) --> V8 LA 360
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_LA_360

            // LUXOR BARONNE (1977)
            "car_luxorbaronne", 

            // LONGTENG FUYUAN (1980)
            "car_longtengfuyuan", 

            // PLYMOUTH BARRACUDA (1971)
            "car_plymouthbarracuda", 

            // SALEM FLAMO (1972)
            "car_salemflamo", "bench_flamo", "bench_flamo_leather1", "bench_flamo_leather2", 

            // ZEPHYR L-SERIES (1979)
            "car_zephyrlseries", "seat_zephyr", "seat_zephyr2", "seat_zephyr3", "bench_zephyr", "rim_zephyr",

            // Mazda RX-3
            "car_mazdarx-3-front_end", "car_mazdarx-3-bumper_front", "car_mazdarx-3-bumper_rear", "car_mazdarx-3-door_front_left",
            "car_mazdarx-3-door_front_right", "car_mazdarx-3-fender_front_left", "car_mazdarx-3-fender_front_right", "car_mazdarx-3-headlight_left",
            "car_mazdarx-3-headlight_right", "car_mazdarx-3-hood", "car_mazdarx-3-taillight_left", "car_mazdarx-3-taillight_right",
            "car_mazdarx-3-trunk", "car_mazdarx-3-window_front", "car_mazdarx-3-window_back", "car_mazdarx-3-window_body_left_1", "car_mazdarx-3-window_body_right_1",
            "car_mazdarx-3-window_door_front_left", "car_mazdarx-3-window_door_front_right", "car_mazdarx-3-mirror_left", "bench_rx-3",
            "seat_rx3", "steering_wheel_rx3",

            // Mazda RX-7
            "car_mazdarx-7-bumper_front", "car_mazdarx-7-bumper_rear", "car_mazdarx-7-door_front_left", "car_mazdarx-7-door_front_right",
            "car_mazdarx-7-fender_front_left", "car_mazdarx-7-fender_front_right", "car_mazdarx-7-headlight_left", "car_mazdarx-7-headlight_right",
            "car_mazdarx-7-hood", "car_mazdarx-7-taillight_left", "car_mazdarx-7-taillight_right", "car_mazdarx-7-trunk", "car_mazdarx-7-window_front",
            "car_mazdarx-7-window_trunk", "car_mazdarx-7-window_door_front_left", "car_mazdarx-7-window_door_front_right", "car_mazdarx-7-mirror_left",
            "car_mazdarx-7-mirror_right", "car_mazdarx-7-trunk2", "bench_rx-7", "seat_rx7", "steering_wheel_rx7", "bench_rx-7", "rim_rx7_1", "rim_rx7_2",

            // Mazda RX-7 -- > engine ROTARY 1 || ROTARY 12A all MAZDA
            "rot_new_blok_przod", "rot_new_blok_srodek_1", "rot_new_blok_srodek_2", "rot_new_blok_tyl", "rot_new_element_nakladka", "rot_new_gearbox",
            "rot_new_kolektor_dolotowy_dolny", "rot_new_kolektor_dolotowy_gorny", "rot_new_kolektor_wydechowy", "rot_new_lancuch", "rot_new_listwa_wtryskowa",
            "rot_new_miska_olejowa", "rot_new_oslona_rozrzadu", "rot_new_pasek_1", "rot_new_pasek_2", "rot_new_pompa_wody", "rot_new_przepustnica",
            "rot_new_raczka_1", "rot_new_raczka_2", "rot_new_tlok", "rot_new_wahadlo", "rot_new_walKorbowy", "rot_new_turbo", "rot_old_filtr_dol",
            "rot_old_filtr_gora", "rot_old_filtr_srodek", "rot_old_kolektor_dolotowy", "rot_old_oslona_rozrzadu", "rot_old_pasek_1", "rot_old_raczka",
            "rot_old_wentylator", "rot_filtr_oleju", "w_rot_new_poczatkowy", "w_rot_old_poczatkowy", "rot_old_gearbox", "t_w_rot_new_poczatkowy",
            "t_w_rot_old_poczatkowy", "rot_new_cewki", "rot_new_kable", "rot_old_kable", "rot_old_kopulkaRozdzielacza",

            // 1974 BMW 2002 Turbo (E10T)
            "car_bmw2002turbo-bumper_front", "car_bmw2002turbo-bumper_rear", "car_bmw2002turbo-door_front_left", "car_bmw2002turbo-door_front_right",
            "car_bmw2002turbo-fender_front_left", "car_bmw2002turbo-fender_front_right", "car_bmw2002turbo-headlight_left", "car_bmw2002turbo-headlight_right",
            "car_bmw2002turbo-hood", "car_bmw2002turbo-mirror_left", "car_bmw2002turbo-mirror_right", "car_bmw2002turbo-taillight_left",
            "car_bmw2002turbo-taillight_right", "car_bmw2002turbo-trunk", "car_bmw2002turbo-window_back", "car_bmw2002turbo-window_body_left_1",
            "car_bmw2002turbo-window_body_right_1", "car_bmw2002turbo-window_door_front_left", "car_bmw2002turbo-window_door_front_right",
            "car_bmw2002turbo-window_front", "seat_2002_turbo", "steering_wheel_2002_turbo", "bench_2002_turbo", "rim_bmw_2002_turbo_1", "rim_bmw_2002_turbo_2", "rim_bmw_2002_turbo_3",

            // ENGINE BMW - R4 M31
            "r4_m31_blok", "r4_m31_glowica", "r4_m31_dzwignia_zaworu_2", "r4_m31_dzwignia_zaworu_1", "r4_m31_kolektor_dolotowy", "r4_m31_kolektor_wydechowy", "r4_m31_kolo_pasowe_walu",
            "r4_m31_lancuch", "r4_m31_miska_olejowa", "r4_m31_old_raczka", "r4_m31_oslona_rozrzadu_1", "r4_m31_oslona_rozrzadu_2", "r4_m31_oslona_rozrzadu_3",
            "r4_m31_pasek_1", "r4_m31_pasek_2", "r4_m31_pokrywa_glowicy", "r4_m31_pompaWody", "r4_m31_turbo", "r4_m31_walek_rozrzadu", "r4_m31_walKorbowy_1",
            "r4_m31_wtrysk", "r4_m31_slizg", "r4_m31_gearbox", "r4_m31_obudowaFiltra_gora", "r4_m31_obudowaFiltra_dol", "w_r4_m31_koncowy_bmw2002turbo", "w_r4_m31_srodkowy_bmw2002turbo",
            "w_r4_m31_poczatkowy_bmw2002turbo", "t_r4_m31_glowica", "t_r4_m31_kable", "t_r4_m31_kolektor_dolotowy", "t_r4_m31_kolektor_wydechowy", "t_r4_m31_kopulkaRozdzielacza",
            "t_r4_m31_rozdzielaczZaplonu_1", "t_r4_m31_turbo", "t_r4_m31_walek_rozrzadu", "t_r4_m31_wtrysk", "t_w_r4_m31_koncowy_bmw2002turbo", "t_w_r4_m31_srodkowy_bmw2002turbo",
            "t_w_r4_m31_poczatkowy_bmw2002turbo","t_r4_m31_gearbox", "r4_m31_kable", "r4_m31_kopulkaRozdzielacza", "r4_m31_rozdzielaczZaplonu_1",

            // 1979 BMW M1 Procar (E26)
            "car_bmwm1procar-bumper_front", "car_bmwm1procar-bumper_rear", "car_bmwm1procar-door_front_left", "car_bmwm1procar-door_front_right", "car_bmwm1procar-fender_front_left",
            "car_bmwm1procar-fender_front_right", "car_bmwm1procar-headlight_left", "car_bmwm1procar-headlight_right", "car_bmwm1procar-hood", "car_bmwm1procar-mirror_left",
            "car_bmwm1procar-mirror_right", "car_bmwm1procar-taillight_left", "car_bmwm1procar-taillight_right", "car_bmwm1procar-trunk", "car_bmwm1procar-window_back",
            "car_bmwm1procar-window_body_left_1", "car_bmwm1procar-window_body_right_1", "car_bmwm1procar-window_door_front_left", "car_bmwm1procar-window_door_front_right",
            "car_bmwm1procar-window_front", "seat_BMW_M1_Procar", "steering_wheel_BMW_M1_Procar", "rim_m1_procar_1", "rim_m1_procar_2", 

            // ENGINE BMW R6 M88
            "r6_m88_blok", "r6_m88_gearbox", "r6_m88_glowica", "r6_m88_kolektor_dolotowy", "r6_m88_kolektor_wodny", "r6_m88_kolektor_wydechowy", "r6_m88_kolo_pasowe_walu",
            "r6_m88_lancuch_1", "r6_m88_lancuch_2", "r6_m88_lancuch_3", "r6_m88_miska_olejowa", "r6_m88_nakladka_glowica", "r6_m88_oslona_rozrzadu", "r6_m88_pasek",
            "r6_m88_pasek_2", "r6_m88_pokrywa_zaworow", "r6_m88_pompa_oleju", "r6_m88_pompa_wody", "r6_m88_przepustnica", "r6_m88_slizg", "r6_m88_walek_rozrzadu_1",
            "r6_m88_walek_rozrzadu_2", "r6_m88_walKorbowy", "r6_m88_wtrysk", "r6_m88_glowica_wkladka", "w_r6_m88_koncowy_bmwm1procar", "t_r6_m88_glowica",
            "t_r6_m88_kable", "t_r6_m88_kolektor_dolotowy", "t_r6_m88_kolektor_wydechowy", "t_r6_m88_kopulkaRozdzielacza", "t_r6_m88_przepustnica",
            "t_r6_m88_rozdzielaczZaplonu_1", "t_r6_m88_walek_rozrzadu_1", "t_r6_m88_walek_rozrzadu_2", "t_r6_m88_wtrysk", "t_w_r6_m88_koncowy_bmwm1procar",
            "t_r6_m88_gearbox", "r6_m88_kable", "r6_m88_kopulkaRozdzielacza", "r6_m88_rozdzielaczZaplonu_1",
        };

        public static string[] RetroParts1980_1990 = {
            // SMITH&PARKER ROADSTER (1980)
            "car_smithandparkerroadster", 

            // Mercedes-benz W124 500e (1990)
            "car_mercedes500e", "w_v8_m119_koncowy_500e", "w_v8_m119_poczatkowy_500e", "seat_500e", "steering_wheel_500e",
            "bench_500e", "t_w_v8_m119_koncowy_500e", "t_w_v8_m119_poczatkowy_500e", "rim_mercedes_benz_500e",
            "v8_m119_blok", "v8_m119_glowica_1", "v8_m119_pokrywa_glowicy_2", "v8_m119_rolka_walka_1", "v8_m119_filtr_srodek",
            "v8_m119_pompaWspomagania", "v8_m119_rolkaWalka_3", "v8_m119_przepustnica", "v8_m119_oslona_rozrzadu_2",
            "v8_m119_miska_olejowa", "v8_m119_rolka_walka_2", "v8_m119_pompaWody", "v8_m119_slizg_1", "v8_m119_filtr_gora",
            "v8_m119_walKorbowy", "v8_m119_pokrywa_cewek_1", "v8_m119_glowica_2", "v8_m119_gearbox", "v8_m119_wspornik",
            "v8_m119_slizg_2", "v8_m119_walek_rozrzadu_2", "v8_m119_kolektor_wydechowy", "v8_m119_pokrywa_glowicy_1",
            "v8_m119_rolka_wspornika", "v8_m119_filtr_dol", "v8_m119_pasek_2", "v8_m119_pokrywa_cewek_2", "v8_m119_kolektor_dolotowy",
            "v8_m119_lancuch_1", "v8_m119_walek_rozrzadu_1", "v8_m119_oslona_rozrzadu_1", "v8_m119_listwa_wtryskowa",
            "v8_m119_glowica_wkladka", "w_v8_m119_koncowy_500e", "w_v8_m119_poczatkowy_500e", "t_w_v8_m119_koncowy_500e",
            "t_v8_m119_kolektor_wydechowy", "t_v8_m119_kolektor_dolotowy", "t_v8_m119_glowica_2", "t_v8_m119_walek_rozrzadu_1",
            "t_v8_m119_przepustnica", "t_w_v8_m119_poczatkowy_500e", "t_v8_m119_walek_rozrzadu_2",
            "t_v8_m119_glowica_1", "t_v8_m119_filtr_srodek", "t_v8_m119_listwa_wtryskowa", "t_v8_m119_pompaWspomagania","t_v8_m119_gearbox",

            // Mercedes-benz W124 500e (1990) || Engine V8 DOHC M119
            "v8_m119_blok", "v8_m119_glowica_1", "v8_m119_pokrywa_glowicy_2", "v8_m119_rolka_walka_1", "v8_m119_filtr_srodek", "v8_m119_pompaWspomagania",
            "v8_m119_rolkaWalka_3", "v8_m119_przepustnica", "v8_m119_oslona_rozrzadu_2", "v8_m119_miska_olejowa", "v8_m119_rolka_walka_2",
            "v8_m119_pompaWody", "v8_m119_slizg_1", "v8_m119_filtr_gora", "v8_m119_walKorbowy", "v8_m119_pokrywa_cewek_1", "v8_m119_glowica_2",
            "v8_m119_gearbox", "v8_m119_wspornik", "v8_m119_slizg_2", "v8_m119_walek_rozrzadu_2", "v8_m119_kolektor_wydechowy",
            "v8_m119_pokrywa_glowicy_1", "v8_m119_rolka_wspornika", "v8_m119_filtr_dol", "v8_m119_pasek_2", "v8_m119_pokrywa_cewek_2",
            "v8_m119_kolektor_dolotowy", "v8_m119_lancuch_1", "v8_m119_walek_rozrzadu_1", "v8_m119_oslona_rozrzadu_1", "v8_m119_listwa_wtryskowa",
            "v8_m119_glowica_wkladka", "w_v8_m119_koncowy_500e", "w_v8_m119_poczatkowy_500e", "t_w_v8_m119_koncowy_500e",
            "t_v8_m119_kolektor_wydechowy", "t_v8_m119_kolektor_dolotowy", "t_v8_m119_glowica_2", "t_v8_m119_walek_rozrzadu_1",
            "t_v8_m119_przepustnica", "t_w_v8_m119_poczatkowy_500e", "t_v8_m119_walek_rozrzadu_2", "t_v8_m119_glowica_1",
            "t_v8_m119_filtr_srodek", "t_v8_m119_listwa_wtryskowa", "t_v8_m119_pompaWspomagania", "t_v8_m119_gearbox", 

            // Mercedes-Benz W126 560SEC and  Engine (V8 M117)
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_M117
            "car_mercedes560sec-bumper_rear", "car_mercedes560sec-bumper_front", "car_mercedes560sec-trunk", "car_mercedes560sec-fender_front_left", 
            "car_mercedes560sec-door_front_right", "car_mercedes560sec-taillight_left", "car_mercedes560sec-door_front_left", "car_mercedes560sec-hood",
            "car_mercedes560sec-headlight_right", "car_mercedes560sec-fender_front_right", "car_mercedes560sec-taillight_right", "car_mercedes560sec-headlight_left",
            "car_mercedes560sec-window_front", "car_mercedes560sec-window_back", "car_mercedes560sec-window_body_left_1", "car_mercedes560sec-window_body_right_1",
            "car_mercedes560sec-window_door_front_left", "car_mercedes560sec-window_door_front_right", "car_mercedes560sec-mirror_left", "car_mercedes560sec-mirror_right",
            "v8_2_lancuch", "v8_2_walKorbowy", "v8_2_rolkaLancucha", "v8_2_pokrywa_dystrybutora", "v8_2_walekdystrybutora",
            "v8_2_rozrzad_oslona", "v8_2_rolkaWalka", "v8_2_pokrywa_glowicy_2", "v8_2_przeplywomierz", "v8_2_kolektordolotowy", "v8_2_filtr_sec_dol",
            "v8_2_glowica", "v8_2_blok", "v8_2_wtrysk", "v8_2_walek_rozrzadu", "v8_2_pokrywa_glowicy_1", "w_v8_2_koncowy", "w_v8_2_tlumik_srodkowy",
            "w_v8_2_rezonator",  "t_w_v8_2_tlumik_srodkowy", "t_w_v8_2_rezonator", "t_v8_2_walek_rozrzadu", "t_v8_2_kable", "t_v8_2_glowica",
            "t_w_v8_2_koncowy", "t_v8_2_kolektordolotowy", "t_v8_2_wtrysk", "v8_2_kable",            

            // EMDEN JAGER (1983)
            "car_emdenjager", "bench_jager",

            // BOLT REPTILIA R2 (1984)
            "car_boltreptiliar2", "seat_reptiliar2", "steering_wheel_reptiliar2", "rim_reptiliar2",

            // SAKURA TAKUMI (1983)
            "car_sakuratakumi", "bench_takumi", 

            // SAKURA KIERAN (1989)
            "car_salemkieran", "bench_kieran", "bench_kieran_leather1", "bench_kieran_leather2",

            // SEERING WHEEL
            "steering_wheel_reptiliar2", "steering_wheel_cape", "steering_wheel_chapman", "steering_wheel_bandit", "steering_wheel_lotz",
            "steering_wheel_xj220", "steering_wheel_etype", "steering_wheel_500e",
            "steering_wheel_winchester", "steering_wheel_charger", "steering_wheel_cuda", "steering_wheel_elise", "steering_wheel_240z",
            "steering_wheel_gullwing",  

            // ENGINE (R4)
            "r4_blok", "r4_filtr_oleju", "r4_gearbox_short", "r4_glowica_2", "r4_kolo_pasowe_walu", "r4_miska_olejowa", "r4_oslona",
            "r4_pokrywa_glowicy", "r4_pompa_wody", "r4_pompa_wspomagania", "r4_przepustnica", "w_tlumik_wzdluzny", "walKorbowy_2",
            "t_w_tlumik_wzdluzny", "t_r4_przepustnica", "t_r4_glowica_2", "t_r4_pompa_wspomagania", "t_r4_gearbox_short",


            // Engine I6 DOHC FMW || (I6 DOHC FMW) 1990 ?
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_DOHC_FMW
            "r6_m50_blok", "r6_m50_gearbox", "r6_m50_glowica", "r6_m50_kolektor_dolotowy", "r6_m50_kolektor_wydechowy_1", "r6_m50_kolektor_wydechowy_2",
            "r6_m50_lancuch_1", "r6_m50_lancuch_2", "r6_m50_listwa_wtryskowa", "r6_m50_miska_olejowa", "r6_m50_oslona_rozrzadu", "r6_m50_pasek_1", "r6_m50_pokrywa_glowicy",
            "r6_m50_pompa_wody", "r6_m50_przepustnica", "r6_m50_slizg", "r6_m50_walek_rozrzadu", "r6_m50_walKorbowy", "r6_m50_wkladka", "w_r6_m50_wydech_poczatkowy",
            "t_r6_m50_glowica", "t_r6_m50_kolektor_wydechowy_1", "t_r6_m50_kolektor_wydechowy_2", "t_r6_m50_przepustnica", "t_r6_m50_listwa_wtryskowa", "t_w_r6_m50_wydech_poczatkowy",
            "t_r6_m50_kolektor_dolotowy", "t_r6_m50_walek_rozrzadu", "t_r6_m50_gearbox",

            // Engine V8 SOHC FME || (V8 SOHC FME) 1990 ?
            // The V8 SOHC FME is based on the Ford Modular V8 introduced in 1990
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_SOHC_FME
            "v8_mod_blok", "v8_mod_filtr_dol", "v8_mod_filtr_gora", "v8_mod_filtr_srodek", "v8_mod_gearbox", "v8_mod_glowica_1", "v8_mod_glowica_2",
            "v8_mod_glowica_wkladka", "v8_mod_kolektor_dolotowy", "v8_mod_kolektor_wydechowy", "v8_mod_lancuch_1", "v8_mod_lancuch_2", "v8_mod_listwa_wtryskowa",
            "v8_mod_miska_olejowa", "v8_mod_oslona_rozrzadu", "v8_mod_pasek", "v8_mod_pokrywa_glowicy_1", "v8_mod_pokrywa_glowicy_2", "v8_mod_pompaWody",
            "v8_mod_przepustnica", "v8_mod_slizg", "v8_mod_walek_rozrzadu", "v8_mod_walKorbowy", "t_v8_mod_przepustnica", "t_v8_mod_listwa_wtryskowa",
            "t_v8_mod_kolektor_wydechowy", "t_v8_mod_filtr_srodek", "t_v8_mod_glowica_1", "t_v8_mod_glowica_2", "t_v8_mod_kolektor_dolotowy",
            "t_v8_mod_walek_rozrzadu", "t_v8_mod_gearbox",

            // 1988 BMW 635 CSi (E24)
            "car_bmw635csi-bumper_front", "car_bmw635csi-bumper_rear", "car_bmw635csi-door_front_left", "car_bmw635csi-door_front_right", "car_bmw635csi-fender_front_left",
            "car_bmw635csi-fender_front_right", "car_bmw635csi-headlight_left", "car_bmw635csi-headlight_right", "car_bmw635csi-hood", "car_bmw635csi-mirror_left",
            "car_bmw635csi-mirror_right", "car_bmw635csi-taillight_left", "car_bmw635csi-taillight_right", "car_bmw635csi-trunk", "car_bmw635csi-window_back",
            "car_bmw635csi-window_body_left_1", "car_bmw635csi-window_body_right_1", "car_bmw635csi-window_door_front_left", "car_bmw635csi-window_door_front_right",
            "car_bmw635csi-window_front", "seat_BMW_E24", "steering_wheel_BMW_E24", "bench_BMW_E24", "rim_bmw_635csi_e24_1",

            // ENGINE M30 FOR BMW
            "r6_m30_blok", "r6_m30_gearbox", "r6_m30_glowica", "r6_m30_dzwignia_zaworu_1", "r6_m30_dzwignia_zaworu_2", "r6_m30_kolektor_dolotowy",
            "r6_m30_kolektor_wydechowy_1", "r6_m30_kolektor_wydechowy_2", "r6_m30_lancuch", "r6_m30_listwa_wtryskowa", "r6_m30_miska_olejowa", "r6_m30_old_raczka",
            "r6_m30_oslona_rozrzadu_1", "r6_m30_oslona_rozrzadu_2", "r6_m30_pasek", "r6_m30_pokrywa_zaworow", "r6_m30_pompa_wody", "r6_m30_przepustnica",
            "r6_m30_slizg_1", "r6_m30_slizg_2", "r6_m30_termostat", "r6_m30_walek_rozrzadu", "r6_m30_walKorbowy", "r6_m30_rura_dolotowa", "r6_m30_obudowaFiltra_gora",
            "r6_m30_obudowaFiltra_dol", "w_r6_m30_koncowy_bmw635csi", "w_r6_m30_srodkowy_bmw635csi", "w_r6_m30_poczatkowy_bmw635csi", "t_r6_m30_glowica", "t_r6_m30_kable",
            "t_r6_m30_kolektor_dolotowy", "t_r6_m30_kolektor_wydechowy_1", "t_r6_m30_kolektor_wydechowy_2", "t_r6_m30_kopulkaRozdzielacza", "t_r6_m30_przepustnica",
            "t_r6_m30_rozdzielaczZaplonu_1", "t_r6_m30_walek_rozrzadu", "t_r6_m30_listwa_wtryskowa", "t_w_r6_m30_poczatkowy_bmw635csi", "t_w_r6_m30_koncowy_bmw635csi",
            "t_w_r6_m30_srodkowy_bmw635csi","t_r6_m30_gearbox", "r6_m30_kable", "r6_m30_kopulkaRozdzielacza", "r6_m30_rozdzielaczZaplonu_1",
        
        };

        public static string[] RetroParts1991_2000 = {
            // YUNBAO YILONG (1992)
            "car_yunbaoyilong",

            // FMW SENTINEL (1992)
            "car_fmwsentinel", "bench_sentinel", 

            // FMW PANTHER MK1 (1990)
            "car_fmwpanthermk1", "bench_Panther_MK1",

            // KATAGIRI TAMAGO (1992)
            "car_katagiritamago", "bench_tamago",

            // KATAGIRI KATSUMOTO (1996)
            "car_katagirikatsumoto", "bench_katsumoto", "bench_katsumoto_leather1", "bench_katsumoto_leather2",

            // AREZZO CORTONA (1996)
            "car_arezzocortona", "w_v12_3_koncowy_arezzocortona", "w_v12_3_poczatkowy_1_arezzocortona", "w_v12_3_poczatkowy_2_arezzocortona",
            "t_w_v12_3_koncowy_arezzocortona", "t_w_v12_3_poczatkowy_1_arezzocortona", "t_w_v12_3_poczatkowy_2_arezzocortona",

            // JAGUAR XJS (1991)
            "car_jaguarxjs", "rim_xjs", "v12_xjs_blok", "v12_xjs_gearbox", "v12_xjs_glowica_1", "v12_xjs_glowica_2", "v12_xjs_kolektor_dolotowy_1",
            "v12_xjs_kolektor_dolotowy_2", "v12_xjs_kolektor_wydechowy_1", "v12_xjs_kolektor_wydechowy_2", "v12_xjs_lancuch", "v12_xjs_listwa_wtryskowa",
            "v12_xjs_miska_olejowa", "v12_xjs_nakladka_glowica_1", "v12_xjs_nakladka_glowica_2", "v12_xjs_oslona_rozrzadu", "v12_xjs_pasek_1",
            "v12_xjs_pasek_2", "v12_xjs_pokrywa_glowicy_1", "v12_xjs_pokyrwa_glowicy_2", "v12_xjs_pompa_wody", "v12_xjs_walek_rozrzadu", "v12_xjs_walKorbowy",
            "v12_xjs_nakladka_glowicy_1", "v12_xjs_obudowa_filtra_dol", "v12_xjs_obudowa_filtra_gora", "v12_xjs_obudowa_filtra_srodek", "w_v12_xjs_koncowy_jaguarxjs",
            "w_v12_xjs_poczatkowy_jaguarxjs", "seat_xjs", "steering_wheel_xjs", "bench_xjs", "t_w_v12_xjs_poczatkowy_jaguarxjs", "t_w_v12_xjs_koncowy_jaguarxjs",
            "v12_xjs_kable", "v12_xjs_kopulkaRozdzielacza", "v12_xjs_rozdzielaczZaplonu",

            // JAGUAR XJS (1991) || ENGINE (V12 HE)
            "v12_xjs_blok", "v12_xjs_gearbox", "v12_xjs_glowica_1", "v12_xjs_glowica_2", "v12_xjs_kolektor_dolotowy_1", "v12_xjs_kolektor_dolotowy_2",
            "v12_xjs_kolektor_wydechowy_1", "v12_xjs_kolektor_wydechowy_2", "v12_xjs_lancuch", "v12_xjs_listwa_wtryskowa", "v12_xjs_miska_olejowa", 
            "v12_xjs_nakladka_glowica_1", "v12_xjs_nakladka_glowica_2","v12_xjs_oslona_rozrzadu","v12_xjs_pasek_1","v12_xjs_pasek_2",
            "v12_xjs_pokrywa_glowicy_1","v12_xjs_pokyrwa_glowicy_2","v12_xjs_pompa_wody","v12_xjs_walek_rozrzadu","v12_xjs_walKorbowy",
            "v12_xjs_nakladka_glowicy_1","v12_xjs_obudowa_filtra_dol","v12_xjs_obudowa_filtra_gora","v12_xjs_obudowa_filtra_srodek","t_v12_xjs_kopulkaRozdzielacza",
            "t_v12_xjs_kolektor_wydechowy_2", "t_v12_xjs_kolektor_dolotowy_1", "t_v12_xjs_glowica_2", "t_v12_xjs_walek_rozrzadu", "t_v12_xjs_glowica_1",
            "t_v12_xjs_kolektor_dolotowy_2", "t_v12_xjs_kolektor_wydechowy_1", "t_v12_xjs_kable", "t_v12_xjs_obudowa_filtra_srodek", "t_v12_xjs_rozdzielaczZaplonu",
            "t_v12_xjs_listwa_wtryskowa", "t_v12_xjs_gearbox", "v12_xjs_kable", "v12_xjs_kopulkaRozdzielacza", "v12_xjs_rozdzielaczZaplonu",

            // JAGUAR seats etc
            "seat_ftype", "steering_wheel_ftype", "rim_ftype",

            // ENGINE "(I6 XK)"
            "r6_xk_blok", "r6_xk_filtr_dol", "r6_xk_filtr_gora_1", "r6_xk_filtr_gora_2", "r6_xk_filtr_srodek", "r6_xk_gaznik", "r6_xk_gearbox",
            "r6_xk_glowica", "r6_xk_glowica_wkladka", "r6_xk_kolektor_wydechowy", "r6_xk_lancuch_1", "r6_xk_lancuch_2", "r6_xk_miska_olejowa",
            "r6_xk_oslona_rozrzadu", "r6_xk_pasek_1", "r6_xk_pokrywa_glowicy_1", "r6_xk_pokrywa_glowicy_2", "r6_xk_pompa_wody", "r6_xk_walek_rozrzadu",
            "r6_xk_walKorbowy", "r6_xk_wspornik_rozrzadu_1", "r6_xk_wspornik_rozrzadu_2", "r6_xk_rolkaLancucha", "r6_xk_wspornik_nakladka",
            "t_r6_xk_walek_rozrzadu", "t_r6_xk_gaznik", "t_r6_xk_kolektor_wydechowy", "t_r6_xk_filtr_srodek", "t_r6_xk_glowica", "t_r6_xk_kable", "t_r6_xk_gearbox", "r6_xk_kable",

            // BOLT CAPE MK7 (1996)
            "car_boltcapemk7", "seat_mk7", "seat_mk7_2", "seat_mk7_3", "bench_mk7",

            // BOLT CARGO (1995)
            "car_boltcargo", 

            // BOLT HORNET (1996)
            "car_bolthornet", "seat_hornet", "seat_hornet2", "seat_hornet3", "steering_wheel_hornet", "bench_hornet",

            // DELRAY HIGHROAD (1996)
            "car_delrayhighroad", "seat_highroad", "seat_highroad2", "seat_highroad3", "bench_highroad", "rim_highroad",

            // HINATA KAGURA SX
            "car_hinatakagurasx", "bench_kagura_sx",

            // HINATA MOON MK1
            "car_hinatamoonmk1", "bench_moon_mk1",

            // LOTUS ELISE S1 (1994) || ENGINE I4 DOHC K-Series (I4 K-Series)
            "car_lotuselises1",
            "w_r4_2_koncowy_elise", "w_r4_2_poczatkowy_elise", "car_lotuselises1-clamshell_front", "car_lotuselises1-clamshell_rear",
            "car_lotuselises1-door_front_left", "car_lotuselises1-door_front_right", "car_lotuselises1-headlight_left", "car_lotuselises1-headlight_right",
            "car_lotuselises1-hood", "car_lotuselises1-taillight_left", "car_lotuselises1-taillight_right", "car_lotuselises1-trunk",
            "car_lotuselises1-window_front", "car_lotuselises1-window_back", "car_lotuselises1-mirror_left", "car_lotuselises1-mirror_right",
            "seat_elise", "steering_wheel_elise", "t_w_r4_2_koncowy_elise", "rim_elise",
            "r4_2_blok", "r4_2_glowica", "r4_2_lozyskowanie_walka", "r4_2_miska_olejowa", "r4_2_kolektor_dolotowy", "r4_2_kolektor_wydechowy",
            "r4_2_oslona_rozrzadu_przod_1", "r4_2_oslona_rozrzadu_przod_2", "r4_2_oslona_rozrzadu_tyl_1", "r4_2_oslona_rozrzadu_tyl_2", "r4_2_pasek_1",
            "r4_2_pasek_2", "r4_2_pokrywa_kabli", "r4_2_walek_rozrzadu", "r4_2_walKorbowy", "r4_2_gearbox", "r4_2_pokrywa_glowicy", "t_r4_2_glowica",
            "t_r4_2_kable", "t_r4_2_kolektor_dolotowy", "t_r4_2_kolektor_wydechowy", "t_r4_2_walek_rozrzadu", "t_r4_2_gearbox", "r4_2_kable",

            // SAKURA SUPA (1993)
            "car_sakurasupa", "bench_supa",

            // RIBBSAN STARLINE (1998)
            "car_ribbsanstarline", "bench_starline",

            // SALEM OVERKING (1998)
            "car_salemoverking", "seat_overking", "seat_overking2", "seat_overking3", "steering_wheel_overking", "bench_overking",

            // VALLSEN 2040 (1993)
            "car_vallsen2040", "seat_vallsen", "seat_vallsen2", "bench_vallsen",

            // Mercedes-Benz W124 500E
            "car_mercedes500e-bumper_front", "car_mercedes500e-bumper_rear", "car_mercedes500e-door_front_left", "car_mercedes500e-door_front_right",
            "car_mercedes500e-door_rear_left", "car_mercedes500e-door_rear_right", "car_mercedes500e-fender_front_left", "car_mercedes500e-fender_front_right",
            "car_mercedes500e-headlight_left", "car_mercedes500e-headlight_right", "car_mercedes500e-hood", "car_mercedes500e-mirror_left",
            "car_mercedes500e-mirror_right", "car_mercedes500e-taillight_left", "car_mercedes500e-taillight_right", "car_mercedes500e-trunk",
            "car_mercedes500e-window_back", "car_mercedes500e-window_door_front_left", "car_mercedes500e-window_door_front_right",
            "car_mercedes500e-window_door_rear_left", "car_mercedes500e-window_door_rear_right", "car_mercedes500e-window_front", "seat_500e",
            "steering_wheel_500e", "bench_500e", "rim_mercedes_benz_500e",
            
            // SEAT and etc BELT
            "seat_atlanta", "seat_cuda", "seat_gullwing", "seat_sec",  "seat_bandit", "seat_typhoon", "seat_reptiliar2", 
            "seat_rollett", "seat_tbx", "seat_charger", "seat_challenger", "seat_elise", 
            "seat_mach", "seat_carreragt", "seat_964", "seat_991", 
            "seat_500e", "seat_chrysler_cordoba", "seat_300c", "seat_gw500", "seat_highroad",
            "seat_hornet", "seat_mk7", "seat_mosquito", "seat_mosquito2016", "seat_overking", "seat_renton", "seat_supernova", "seat_vallsen", "seat_wildcat",
            "seat_zephyr", "seat_240z", "seat_etype", "seat_xj220", "seat_xjs", 
            "seat_db5", "seat_1_leather5", "seat_2_leather5", "seat_3_leather5", "seat_3_leather1", "seat_3_leather2", "seat_3_leather3", "seat_3_leather5", "seat_3_leather4",
            "seat_4_leather5", "seat_5_leather5", "seat_6_leather5", "seat_7_leather5", "seat_8_leather5", "seat_9_leather5", "seat_10_leather5", "seat_11_leather5",
            "seat_1_leather6", "seat_2_leather6", "seat_3_leather6", "seat_4_leather6", "seat_5_leather6", "seat_6_leather6", "seat_7_leather6", "seat_8_leather6",
            "seat_9_leather6", "seat_10_leather6", "seat_11_leather6", "seat_gw500_2", "seat_highroad2", "seat_hornet2", "seat_mk7_2", "seat_mosquito2","seat_mosquito2016_2",
            "seat_overking2", "seat_supernova2", "seat_vallsen2", "seat_wildcat2", "seat_zephyr2", "seat_typhoon2", "seat_gw500_3", "seat_highroad3", "seat_hornet3",
            "seat_mk7_3", "seat_mosquito3", "seat_mosquito2016_3", "seat_overking3", "seat_supernova3", "seat_wildcat3", "seat_zephyr3", "seat_typhoon3",
            "bench_custom", "bench_custom_leather1", "bench_custom_leather2", "bench_magnum", "bench_magnum_leather1", "bench_magnum_leather2", 
            "bench_moon", "bench_moon_leather1", "bench_moon_leather2", "bench_sec", "bench_katsumoto", "bench_katsumoto_leather1", "bench_katsumoto_leather2",
            "bench_kieran", "bench_kieran_leather1", "bench_kieran_leather2", "bench_flamo", "bench_flamo_leather1", "bench_flamo_leather2", 
            "bench_fastback", "bench_fastback_leather1", "bench_fastback_leather2", "bench_cuda", 
            "bench_hellcat", "bench_hellcat_leather1", "bench_hellcat_leather2", "bench_bowen1",
            "bench_bowen2", "bench_cape1", "bench_cape2", "bench_chapman1", "bench_chapman2", "bench_bandit", "bench_rollett", "bench_tbx", 
            "bench_roadrunner1", "bench_roadrunner2", "bench_charger", "bench_challenger",
            "bench_991", "bench_lx550", "bench_500e", "bench_chrysler_cordoba",
            "bench_300c", "bench_baronne1", "bench_baronne2", "bench_goldtrail1", "bench_goldtrail2", "bench_highroad", "bench_hornet",
            "bench_mk7", "bench_mosquito", "bench_mosquito2016", "bench_overking", "bench_supernova", "bench_vallsen", "bench_wildcat", "bench_zephyr",
            "bench_cebulion",  "bench_jager", "bench_Panther_MK1", "bench_sentinel", "bench_kagura_sx", "bench_tamago",
            "bench_supa", "bench_winchester1", "bench_winchester2", "bench_takumi", "bench_starline", "bench_xjs",
            "bench_earthquake", "bench_earthquake_rex", "bench_earthquake_rex_leather1", "bench_earthquake_rex_leather2", "bench_earthquake_leather1",
            "bench_earthquake_leather2",

            // Engine I6 DOHC BU2 T || (I6 DOHC BU2 T) 1993/1997 ?
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_DOHC_BU2_Turbo
            // This engine is based on the Toyota 2JZ. The I6 DOHC BU2 Turbo and I6 DOHC BU2 are similar using the same parts
            // with the exceptions being the Engine Block, Exhaust Manifold, Turbocharger, Intake Manifold and Throttle.
            "r6_2jz_t_blok", "r6_2jz_t_gearbox", "r6_2jz_t_glowica", "r6_2jz_t_glowica_wkladka", "r6_2jz_t_kolektor_dolotowy", "r6_2jz_t_kolektor_wydechowy",
            "r6_2jz_t_listwa_wtryskowa", "r6_2jz_t_miska_olejowa", "r6_2jz_t_napinacz", "r6_2jz_t_oslona_rozrzadu_1", "r6_2jz_t_oslona_rozrzadu_2", "r6_2jz_t_pasek_1",
            "r6_2jz_t_pasek_2", "r6_2jz_t_pokrywa_glowicy_1", "r6_2jz_t_pokrywa_glowicy_2", "r6_2jz_t_pokrywa_swiec", "r6_2jz_t_pompaWody", "r6_2jz_t_przepustnica",
            "r6_2jz_t_turbo", "r6_2jz_t_walek_rozrzadu", "r6_2jz_t_walKorbowy", "w_r6_2jz_t_wydech_poczatkowy", "w_r6_2jz_t_wydech_poczatkowy_h1",
            "t_r6_2jz_t_przepustnica", "t_r6_2jz_t_turbo", "t_r6_2jz_t_kolektor_wydechowy", "t_r6_2jz_t_listwa_wtryskowa", "t_r6_2jz_t_glowica", "t_w_r6_2jz_t_wydech_poczatkowy",
            "t_w_r6_2jz_t_wydech_poczatkowy_h1", "t_r6_2jz_t_kolektor_dolotowy", "t_r6_2jz_t_walek_rozrzadu", "t_r6_2jz_t_gearbox",

            // Engine I6 DOHC BU2 || (I6 DOHC BU2) 1993/1997 ?
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_DOHC_BU2
            "r6_2jz_blok", "r6_2jz_kolektor_dolotowy", "r6_2jz_kolektor_wydechowy_1", "r6_2jz_kolektor_wydechowy_2", "r6_2jz_przepustnica",
            "w_r6_2jz_wydech_poczatkowy", "w_r6_2jz_wydech_poczatkowy_h1", "t_r6_2jz_kolektor_wydechowy_1", "t_r6_2jz_kolektor_wydechowy_2", "t_r6_2jz_przepustnica",
            "t_w_r6_2jz_wydech_poczatkowy_h1", "t_w_r6_2jz_wydech_poczatkowy", "t_r6_2jz_kolektor_dolotowy", 
        };

        public static string[] RetroParts2001_2005 = {
            // CHRYSLER 300C SRT8
            "car_chrysler300csrt8", "w_v8_61hemi_poczatkowy_chrysler300c", "w_v8_61hemi_koncowy__chrysler300c", "seat_300c",
            "steering_wheel_300c", "bench_300c", "t_w_v8_61hemi_poczatkowy_chrysler300c", "t_w_v8_61hemi_koncowy__chrysler300c", "rim_300c",

            // CHRYSLER 300C SRT8 -> Engine V8 Hemi 6.1
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V8_Hemi_6.1

            // FMW PANTHER MK2 (2003)
            "car_fmwpanther", "bench_Panther_MK1",

            // ELENTI CALLOPE (2003)
            "car_elenticallope", "bench_FMW_Panther", "bench_FMW_Panther_leather1", "bench_FMW_Panther_leather2",

            // MAYEN M6 (2002)
            "car_mayenm6", "bench_mayenm6","bench_mayenm6_leather1","bench_mayenm6_leather2",
            "bench_mayenm6_leather8","bench_mayenm6_leather8",

            // MAYEN M8 (2003)
            "car_mayenm8", "bench_mayenm8", "bench_mayenm8", "bench_mayenm8_leather1","bench_mayenm8_leather2", 

            // RIBBSAN Z530 (2003)
            "car_ribbsanz530", "w_v6_37n_koncowy_ribbsanz530", "t_w_v6_37n_koncowy_ribbsanz530",

            // SCEO LX550 (2003)
            "car_sceolx550", "LX550",

            // SIXON CEBULION (2003)
            "car_sixoncebulion", "bench_cebulion",

            // Nissan Silvia (S15) (2002)
            "car_nissans15silvia-bumper_front", "car_nissans15silvia-bumper_rear", "car_nissans15silvia-door_front_left", "car_nissans15silvia-door_front_right",
            "car_nissans15silvia-fender_front_left", "car_nissans15silvia-fender_front_right", "car_nissans15silvia-headlight_left", "car_nissans15silvia-headlight_right",
            "car_nissans15silvia-hood", "car_nissans15silvia-taillight_left", "car_nissans15silvia-taillight_right", "car_nissans15silvia-trunk", "car_nissans15silvia-window_front",
            "car_nissans15silvia-window_back", "car_nissans15silvia-window_body_left_1", "car_nissans15silvia-window_body_right_1", "car_nissans15silvia-window_door_front_left",
            "car_nissans15silvia-window_door_front_right", "car_nissans15silvia-mirror_left", "car_nissans15silvia-mirror_right", "seat_Silvia_S15", "bench_silvia_s15",

            // Nissan Silvia (S15) (2002) || I4 DOHC SR20DET
            "r4_sr20_blok", "r4_sr20_gearbox", "r4_sr20_glowica", "r4_sr20_glowica_wkladka", "r4_sr20_kolektor_dolotowy", "r4_sr20_kolektor_wydechowy",
            "r4_sr20_lancuch", "r4_sr20_listwa_wtryskowa", "r4_sr20_miska_olejowa", "r4_sr20_oslona_rozrzadu", "r4_sr20_pasek_1", "r4_sr20_pasek_2",
            "r4_sr20_pokrywa_glowicy", "r4_sr20_pompa_wody", "r4_sr20_przepustnica", "r4_sr20_rolka_pompy_wody", "r4_sr20_rolka_walka_1", "r4_sr20_rolka_walka_2",
            "r4_sr20_slizg", "r4_sr20_walek_rozrzadu", "r4_sr20_turbo", "r4_sr20_wentylator", "r4_sr20_walKorbowy", "w_r4_poczatkowy_wzdluzny_sr20",
            "t_w_r4_poczatkowy_wzdluzny_sr20", "t_r4_sr20_kolektor_dolotowy", "t_r4_sr20_walek_rozrzadu",

            // Engine V6 DOHC AKZ || (V6 B) 2003 ?  // (V6 DOHC CHG) 2007 ?
            // https://car-mechanic-simulator-2021.fandom.com/wiki/V6_DOHC_AKZ
            // The V6 DOHC AKZ and V6 DOHC CHG are identical using the same parts and generating the same power, the only difference being the transmission used.
            "v6_30_blok", "v6_30_gearbox_2", "v6_30_gearbox", "v6_30_glowica_1", "v6_30_glowica_2", "v6_30_glowica_raczka", "v6_30_kolektor_dolotowy", 
            "v6_30_kolektor_dolotowy_dolny", "v6_30_kolektor_wydechowy", "v6_30_listwa_wtryskowa", "v6_30_miska_olejowa", "v6_30_oslona_rozrzadu_1",
            "v6_30_oslona_rozrzadu_2", "v6_30_oslona_rozrzadu_3", "v6_30_pasek_1", "v6_30_pasek_2", "v6_30_pokrywa_glowicy_1", "v6_30_pokrywa_glowicy_2",
            "v6_30_pokrywa_glowicy_3", "v6_30_pompaWspomagania", "v6_30_pompa_wody", "v6_30_przepustnica", "v6_30_raczka_1", "v6_30_rolkaWalka",
            "v6_30_termostat", "v6_30_walek_rozrzadu", "v6_30_walKorbowy", "w_v6_30_poczatkowy_1", "w_v6_30_poczatkowy_2", "w_v6_30_poczatkowy_h0", "w_v6_30_poczatkowy_h1",
            "w_v6_30_poczatkowy_h2", "w_v6_30_poczatkowy_h3", "w_v6_30_poczatkowy_h4",  "t_v6_30_glowica_1", "t_v6_30_glowica_2", "t_v6_30_listwa_wtryskowa",
            "t_v6_30_przepustnica", "t_v6_30_kolektor_dolotowy", "t_v6_30_kolektor_dolotowy_dolny", "t_v6_30_kolektor_wydechowy",
            "t_v6_30_pompaWspomagania", "t_v6_30_walek_rozrzadu", "t_w_v6_30_poczatkowy_1", "t_w_v6_30_poczatkowy_2", "t_w_v6_30_poczatkowy_h1",
            "t_w_v6_30_poczatkowy_h4", "t_w_v6_30_poczatkowy_h3", "t_w_v6_30_poczatkowy_h2", "t_w_v6_30_poczatkowy_h0", "t_v6_30_gearbox", "t_v6_30_gearbox_2",

            // 2002 BMW M3 (E46)
            "car_bmwm3-e46-bumper_front", "car_bmwm3-e46-bumper_rear",  "car_bmwm3-e46-door_front_left", "car_bmwm3-e46-door_front_right", "car_bmwm3-e46-fender_front_left",
            "car_bmwm3-e46-fender_front_right", "car_bmwm3-e46-headlight_left", "car_bmwm3-e46-headlight_right", "car_bmwm3-e46-hood", "car_bmwm3-e46-mirror_left",
            "car_bmwm3-e46-mirror_right", "car_bmwm3-e46-taillight_left", "car_bmwm3-e46-taillight_left_trunk", "car_bmwm3-e46-taillight_right",
            "car_bmwm3-e46-taillight_right_trunk", "car_bmwm3-e46-trunk", "car_bmwm3-e46-window_back", "car_bmwm3-e46-window_body_left_1",
            "car_bmwm3-e46-window_body_right_1", "car_bmwm3-e46-window_door_front_left", "car_bmwm3-e46-window_door_front_right", "car_bmwm3-e46-window_front", "seat_m3_e46",
            "steering_wheel_m3_e46", "bench_m3_e46", "t_w_r4_m31_koncowy_bmw2002turbo", "t_w_r4_m31_srodkowy_bmw2002turbo", "t_w_r4_m31_poczatkowy_bmw2002turbo","rim_bmw_m3_e46_1",

            // ENGINE BMW S54
            "r6_s54_wkladka", "r6_s54_blok", "r6_s54_gearbox", "r6_s54_glowica", "r6_s54_kolektor_dolotowy", "r6_s54_kolektor_wydechowy_1", "r6_s54_kolektor_wydechowy_2",
            "r6_s54_lancuch_1", "r6_s54_lancuch_2", "r6_s54_listwa_wtryskowa", "r6_s54_miska_olejowa", "r6_s54_oslona_rozrzadu", "r6_s54_pasek_1", "r6_s54_pokrywa_glowicy",
            "r6_s54_pokrywa_silnika", "r6_s54_pompa_wody", "r6_s54_przepustnica", "r6_s54_slizg", "r6_s54_walek_rozrzadu", "r6_s54_walKorbowy", "r6_s54_wentylator",
            "m3_filtr_srodek", "m3_filtr_gora", "m3_filtr_dol", "m3_rura_dolotowa", "w_r6_s54_koncowy", "w_r6_s54_srodkowy", "w_r6_s54_poczatkowy_1", "w_r6_s54_poczatkowy_2",
            "t_r6_s54_glowica", "t_r6_s54_kolektor_dolotowy", "t_r6_s54_kolektor_wydechowy_1", "t_r6_s54_kolektor_wydechowy_2", "t_r6_s54_listwa_wtryskowa", "t_r6_s54_przepustnica",
            "t_r6_s54_walek_rozrzadu", "t_m3_filtr_srodek", "t_w_r6_s54_koncowy", "t_w_r6_s54_srodkowy", "t_w_r6_s54_poczatkowy_1","t_w_r6_s54_poczatkowy_2", "t_r6_s54_gearbox",

        };

        public static string[] SportGT = {
            // PAGANI ZONDA REVOLUTION
            "car_paganizondarevolucion", "v12_zonda_gearbox", "v12_zonda_kolektor_dolotowy", "v12_zonda_kolektor_wydechowy_1", "v12_zonda_kolektor_wydechowy_2",
            "v12_zonda_listwa_wtryskowa", "v12_zonda_pokrywa_glowicy_1", "v12_zonda_pokrywa_glowicy_1_2", "v12_zonda_pokrywa_glowicy_2", "v12_zonda_pokrywa_glowicy_2_2",
            "v12_zonda_oslona_kolektora", "w_v12zonda_koncowy", "v12_zonda_filtr", "seat_zonda", "steering_wheel_zonda", "t_v12_zonda_filtr", "t_v12_zonda_kolektor_dolotowy",
            "t_v12_zonda_kolektor_wydechowy_1", "t_v12_zonda_kolektor_wydechowy_2", "t_v12_zonda_listwa_wtryskowa", "t_w_v12zonda_koncowy", "rim_zonda_A", "rim_zonda_B",
            "t_v12_zonda_gearbox",
        
            // PORSCHE 911 RS AMERICA (964) ||  PORSCHE 911 Turbo S (964)  (1993)
            "car_porsche964", "car_porsche964rs", "w_b62_koncowy_A_porshe964", "w_b62_koncowy_B_porshe964", "w_b62_koncowy_A_porshe964RS", "w_b62_koncowy_B_porshe964RS",
            "w_b62_poczatkowy_porshe964RS", "seat_964", "steering_wheel_964", "t_w_b62_koncowy_A_porshe964", "t_w_b62_koncowy_A_porshe964RS", "t_w_b62_koncowy_B_porshe964",
            "t_w_b62_koncowy_B_porshe964RS", "t_w_b62_poczatkowy_porshe964RS", "rim_porsche_964RS", 

            // PORSCHE CARRERA GT (2003) || PORSCHE 911 CARRERA 4S (991) (2016) ??????
            "car_porsche991", "car_porschecarreragt", "w_v10_koncowy_carreragt", "w_v10_srodkowy_carreragt", "w_v10_poczatkowy_carreragt", "seat_carreragt", "steering_wheel_carreragt",
            "t_w_v10_koncowy_carreragt", "t_w_v10_poczatkowy_carreragt", "t_w_v10_srodkowy_carreragt", "rim_porshe_991_carrera_4s", "rim_porshe_carrera_gt",

            // ENGINE FOR 911 Turbo S (964) || B6 M64.50
            // https://car-mechanic-simulator-2021.fandom.com/wiki/B6_M64.50
            "b62_2_filtr_dol", "b62_2_filtr_gora", "b62_2_filtr_srodek", "b62_2_intercooler", "b62_2_kolektor_dolotowy", "b62_2_pasek",
            "b62_2_pokrywa", "b62_2_przepustnica", "b62_2_wtrysk", "b62_2_wtrysk_dol", "b62_turbo", "t_b62_2_filtr_srodek", "t_b62_2_intercooler",
            "t_b62_2_kolektor_dolotowy", "t_b62_2_przepustnica", "t_b62_2_wtrysk", "t_b62_turbo",

            // ENGINE FOR 911 RS America (964) || B6 M64.01
            // https://car-mechanic-simulator-2021.fandom.com/wiki/B6_M64.01
            "b62_kolektor_dolotowy_1", "b62_kolektor_dolotowy_2", "b62_przepustnica", "b62_dmuchawa", "b62_filtr_srodek", "b62_filtr_dol", "b62_filtr_gora",
            "b6_filtr_oleju", "t_b62_filtr_srodek", "t_b62_kolektor_dolotowy_1", "t_b62_kolektor_dolotowy_2", "t_b62_przepustnica",

            // Carrera GT || V10 DOHC 980/01
            "v10_blok", "v10_filtr", "v10_filtr_dol", "v10_filtr_gora", "v10_gearbox", "v10_glowica_1", "v10_glowica_2", "v10_kolektor_dolotowy",
            "v10_kolektor_wydechowy", "v10_lancuch", "v10_listwa_wtryskowa", "v10_miska_olejowa", "v10_obudowa_sprzegla", "v10_oslona_rozrzadu", "v10_pasek_1",
            "v10_pasek_2", "v10_pokrywa_glowicy_1", "v10_pokrywa_glowicy_2", "v10_rolka_walka_1", "v10_rolka_walka_2", "v10_slizg", "v10_sprzeglo", "v10_walek_rozrzadu",
            "v10_walKorbowy", "v10_pompaWody", "v10_rolka_pompy_wody", "t_v10_filtr", "t_v10_glowica_1", "t_v10_glowica_2", "t_v10_kolektor_dolotowy",
            "t_v10_kolektor_wydechowy", "t_v10_listwa_wtryskowa", "t_v10_sprzeglo", "t_v10_walek_rozrzadu", "t_v10_gearbox", 

            // 911 Carrera 4S (991.2) ||  B6 MA 1.03
            // https://car-mechanic-simulator-2021.fandom.com/wiki/B6_MA_1.03
            "b61_blok_1", "b61_blok_2", "b61_gearbox", "b61_glowica_1", "b61_glowica_2", "b61_glowica_wkladka", "b61_kolektor_dolotowy_1",
            "b61_kolektor_dolotowy_2", "b61_lancuch", "b61_listwa_wtryskowa", "b61_miska_olejowa", "b61_pasek", "b61_pokrywa_glowicy_1", "b61_pokrywa_glowicy_2",
            "b61_pompaWody", "b61_przepustnica", "b61_rolka_pompy_wody", "b61_rolka_walka_1", "b61_rolka_walka_2", "b61_slizg", "b61_turbo",
            "b61_walek_rozrzadu", "b61_walKorbowy", "b61_wlew_oleju", "b61_blok_nakladka", "b61_filtr_dol", "b61_filtr_gora",
            "b61_filtr_srodek", "b61_glowica_1_pokrywa", "b61_glowica_2_pokrywa", "b61_dyferPrzod",
        
            // 2016 BMW M4 GTS Special Edition (F82)
            "car_bmwm4gts-bumper_front", "car_bmwm4gts-bumper_rear", "car_bmwm4gts-door_front_left", "car_bmwm4gts-door_front_right", "car_bmwm4gts-fender_front_left",
            "car_bmwm4gts-fender_front_right", "car_bmwm4gts-headlight_left", "car_bmwm4gts-headlight_right", "car_bmwm4gts-hood", "car_bmwm4gts-mirror_left",
            "car_bmwm4gts-mirror_right", "car_bmwm4gts-taillight_left", "car_bmwm4gts-taillight_left_trunk", "car_bmwm4gts-taillight_right",
            "car_bmwm4gts-taillight_right_trunk", "car_bmwm4gts-trunk", "car_bmwm4gts-window_back", "car_bmwm4gts-window_body_left_1", "car_bmwm4gts-window_body_right_1",
            "car_bmwm4gts-window_door_front_left", "car_bmwm4gts-window_door_front_right", "car_bmwm4gts-window_front", "seat_m4_gts", "steering_wheel_m4_gts",
            "rim_bmw_m4_gts_1", "rim_bmw_m4_gts_2", "rim_bmw_m4_gts_3",

            // ENGINE BMW 2016 - S55
            "r6_s55_blok", "r6_s55_rura_intercooler_2", "r6_s55_rura_intercooler_1", "r6_s55_walek_rozrzadu", "r6_s55_gearbox", "r6_s55_glowica",
            "r6_s55_intercooler", "r6_s55_kolektor_dolotowy", "r6_s55_kolektor_wydechowy", "r6_s55_kolo_pasowe_walu", "r6_s55_lancuch", "r6_s55_listwa_wtryskowa", "r6_s55_miska_olejowa",
            "r6_s55_pasek_1", "r6_s55_pasek_2", "r6_s55_pokrywa_glowicy","r6_s55_pokrywa_silnika", "r6_s55_pompa_wody", "r6_s55_przepustnica", "r6_s55_rolka_walka", "r6_s55_slizg_1",
            "r6_s55_slizg_2", "r6_s55_slizg_3", "r6_s55_sprezarka_klimatyzacji", "r6_s55_walKorbowy", "r6_s55_wkladka", "w_r6_s55_koncowy_bmwm4gts", "w_r6_s55_srodkowy_bmwm4gts",
            "w_r6_s55_poczatkowy_1_bmwm4gts", "w_r6_s55_poczatkowy_2_bmwm4gts", "t_r6_s55_walek_rozrzadu", "t_r6_s55_glowica", "t_r6_s55_intercooler", "t_r6_s55_kolektor_dolotowy",
            "t_r6_s55_kolektor_wydechowy", "t_r6_s55_listwa_wtryskowa", "t_r6_s55_przepustnica", "t_r6_s55_rura_intercooler_2", "t_r6_s55_rura_intercooler_1", "t_w_r6_s55_koncowy_bmwm4gts",
            "t_w_r6_s55_srodkowy_bmwm4gts", "t_w_r6_s55_poczatkowy_1_bmwm4gts", "t_w_r6_s55_poczatkowy_2_bmwm4gts", "t_r6_s55_gearbox",

        };

        public static string[] SpecialRetro = {
            //  Mercedes-Benz W198 300SL (1954)
            "car_mercedes300slgullwing",
            "car_mercedes300slgullwing-hood", "car_mercedes300slgullwing-headlight_left", "car_mercedes300slgullwing-bumper_rear", "car_mercedes300slgullwing-bumper_front",
            "car_mercedes300slgullwing-door_front_right", "car_mercedes300slgullwing-door_front_left", "car_mercedes300slgullwing-taillight_right", "car_mercedes300slgullwing-taillight_left",
            "car_mercedes300slgullwing-trunk", "car_mercedes300slgullwing-headlight_right", "car_mercedes300slgullwing-window_front", "car_mercedes300slgullwing-window_back",
            "car_mercedes300slgullwing-window_body_left_1", "car_mercedes300slgullwing-window_body_right_1", "car_mercedes300slgullwing-window_door_front_left",
            "car_mercedes300slgullwing-window_door_front_right", "car_mercedes300slgullwing-mirror_left",

            // Aston martin DB5 (1963)         
            "car_astonmartindb5", "w_r6_tm_koncowy_1_astonmartindb5", "w_r6_tm_koncowy_2_astonmartindb5", "w_r6_tm_poczatkowy_1_astonmartindb5", 
            "w_r6_tm_poczatkowy_2_astonmartindb5", "w_r6_tm_srodkowy_1_astonmartindb5", "w_r6_tm_srodkowy_2_astonmartindb5", "seat_db5",
            "steering_wheel_db5", "bench_db5", "t_w_r6_tm_koncowy_1_astonmartindb5", "t_w_r6_tm_koncowy_2_astonmartindb5", "t_w_r6_tm_poczatkowy_1_astonmartindb5",
            "t_w_r6_tm_poczatkowy_2_astonmartindb5", "t_w_r6_tm_srodkowy_1_astonmartindb5", "t_w_r6_tm_srodkowy_2_astonmartindb5", "rim_db5",

            // Engine Aston martin DB5 --> I6 Tadek Marek
            // https://car-mechanic-simulator-2021.fandom.com/wiki/I6_Tadek_Marek

            // JUEYUAN QIANFENG (1959)
            "car_jueyuanqianfeng", 

            // DC TYPHOON (1964)
            "car_dctyphoon", "seat_typhoon", "seat_typhoon2", "seat_typhoon3", "steering_wheel_typhoon", "rim_typhoon",

            // FORD GT40 (1965) || V8 Ford Small Block 289 M
            "car_fordgt40", "seat_fordgt40", "steering_wheel_fordgt40", "rim_fordgt40", 
            "w_koncowy_gt40", "car_fordgt40-headlight_left", "car_fordgt40-headlight_right", "car_fordgt40-door_front_left",
            "car_fordgt40-door_front_right",  "car_fordgt40-taillight_right", "car_fordgt40-taillight_left", "car_fordgt40-clamshell_front",
            "car_fordgt40-clamshell_rear", "car_fordgt40-window_front", "car_fordgt40-window_trunk", "car_fordgt40-window_door_front_left",
            "car_fordgt40-window_door_front_right", "car_fordgt40-mirror_left", "car_fordgt40-mirror_right",
            "t_w_koncowy_gt40", "rim_fordgt40",

            // V8 Ford Small Block 289 M
            "v8_289_blok", "v8_289_filtr_dol",  "v8_289_filtr_gora", "v8_289_filtr_srodek", "v8_289_glowica", "v8_289_kolektor_dolotowy",
            "v8_289_kolektor_dolotowy2", "v8_289_kolektor_wydechowy", "v8_289_kolektor_wydechowy_gt40", "v8_289_miska_olejowa", "v8_289_pasek_1",
            "v8_289_pasek_2", "v8_289_pokrywa_glowicy_1", "v8_289_pokrywa_glowicy_2", "v8_289_raczka", "v8_289_rolka_pompy_wody", "v8_289_rozrzad_oslona",
            "v8_289_wentylator", "v8_289_trabka",


            // JAGUAR E-TYPE (1961)
            "car_jaguaretype", "w_r6_xk_koncowy_jaguaretype", "w_r6_xk_poczatkowy_A_jaguaretype", "w_r6_xk_poczatkowy_B_jaguaretype",
            "t_w_r6_xk_poczatkowy_A_jaguaretype", "t_w_r6_xk_koncowy_jaguaretype", "t_w_r6_xk_poczatkowy_B_jaguaretype",

            // JAGUAR XJ220 (1992)
            "car_jaguarxj220", "w_v6_35tt_koncowy_jaguarxj220", "w_v6_35tt_poczatkowy_A_jaguarxj220", "w_v6_35tt_poczatkowy_B_jaguarxj220", "seat_xj220",
            "steering_wheel_xj220", "t_w_v6_35tt_poczatkowy_B_jaguarxj220", "t_w_v6_35tt_poczatkowy_A_jaguarxj220", "t_w_v6_35tt_koncowy_jaguarxj220", "rim_xj220_front",
            "rim_xj220_back",

            // LOTUS ESPRIT S1 (1975)  || I4 DOHC Lotus 907
            "car_lotusesprits1", "w_r4_old_koncowy_esprit", "w_r4_old_poczatkowy_esprit", "seat_esprit", "steering_wheel_esprit", "t_w_r4_old_koncowy_esprit", 
            "t_w_r4_old_poczatkowy_esprit", "rim_esprit", "seat_esprit", "car_lotusesprits1-bumper_front", "car_lotusesprits1-bumper_rear", "car_lotusesprits1-door_front_left",
            "car_lotusesprits1-door_front_right", "car_lotusesprits1-clamshell_front", "car_lotusesprits1-engine_cover", "car_lotusesprits1-headlight_left",
            "car_lotusesprits1-headlight_right", "car_lotusesprits1-hood", "car_lotusesprits1-taillight_left", "car_lotusesprits1-taillight_right",
            "car_lotusesprits1-trunk", "car_lotusesprits1-window_front", "car_lotusesprits1-window_trunk", "car_lotusesprits1-window_back",
            "car_lotusesprits1-window_body_left_1", "car_lotusesprits1-window_body_right_1", "car_lotusesprits1-window_door_front_left", "car_lotusesprits1-window_door_front_right",
            "car_lotusesprits1-mirror_left", "car_lotusesprits1-mirror_right",
            "r4_old_blok", "r4_old_gearbox", "r4_old_glowica", "r4_old_glowica_2_1","r4_old_glowica_2_2", "r4_old_kolektor_dolotowy",
            "r4_old_kolektor_wydechowy", "r4_old_pasek_1", "r4_old_pasek_2", "r4_old_walKorbowy", "r4_old_pokrywa_glowicy_1",
            "r4_old_pokrywa_glowicy_2", "r4_old_miska_olejowa", "r4_old_puszkaFiltra_tyl", "r4_old_puszkaFiltra_przod",
            "t_r4_old_glowica", "t_r4_old_glowica_2_1", "t_r4_old_glowica_2_2", "t_r4_old_kable", "t_r4_old_kolektor_dolotowy",
            "t_r4_old_kolektor_wydechowy", "t_r4_old_kopulkaRozdzielacza", "t_r4_old_gearbox", "r4_old_kable", "r4_old_kopulkaRozdzielacza",

            // SAKURA GT20 (1969)
            "car_sakuragt20", 

            //
            "steering_wheel1", "steering_wheel3", "steering_wheel4", "steering_wheel5", "steering_wheel6", "steering_wheel7", "steering_wheel8", "steering_wheel9",
            "steering_wheel10", "steering_wheel11", "steering_wheel12", "steering_wheel13", "steering_wheel14", "steering_wheel15", "steering_wheel16", "steering_wheel17",
        };

        public static string[] CarNames = { 
            "Aston Martin DBS", "Aston Martin DBX", "Aston Martin Vantage", "Atom 330", "Atom Renton", "Autotutto Lavetino",
            "Bolt Mosquito MK5", "Bolt Mosquito MK6", "Bolt Reptilia R6", "Bolthorn Grand Mojave", "Castor Avalanche",
            "Dodge Challenger SRT Hellcat Widebody", "Dodge Challenger SRT Hellcat", "Dodge Viper GTS", "Echos Cobra", 
            "Echos Imperator", "Elenti Izi", "Emden Lotz", "FMW Roadster", "Ford F350 Super Duty", "Ford Mustang",
            "Griffin Tyro", "Hammerdal Strale S", "Hinata Moon MK2", "Jaguar F-Type", "Lacroix Fret",
            "Land Rover Defender 90", "Land Rover Discovery", "Land Rover Range Rover Evoque", "Land Rover Range Rover Sport SVR",
            "Mayen M3", "Mayen T8", "Mercedes-Benz SLS AMG", "Mioveni Urs", "Nissan 370z Nismo", "Nissan GT-R (R35)",
            "Olsen Grand Club", "Olsen GTR", "Pagani Huayra Tempesta", "Pagani Huayra", "Pagani Zonda Revolucion",
            "Porsche 911 Carrera 4S (991)", "Rino Bianco", "Rino Piccolo", "Royale Town", "Royale Crown", 
            "Sakura Yaro", "Salem Earthquake Rex", "Salem Earthquake", "Salem GW500", "Salem Spectre"
        };

        private const uint RemoveCarsOlderYear = 2005;

        private const string ConfigCategoryName = "MainSection";
        private const string ConfigCarSection = "CarSection";
        private const string ConfigRepairSection = "RepairSection";
        private const string ConfigCategory = "Category";

        private readonly MelonPreferences_Category _config;
        private readonly MelonPreferences_Category _repair;
        private readonly MelonPreferences_Category _carSection;
        private readonly MelonPreferences_Category _category;
        private readonly MelonPreferences_Entry<KeyCode> _isKeyBindEnableSwitchMode;
        private readonly MelonPreferences_Entry<KeyCode> _printStackBindKey;
        private readonly MelonPreferences_Entry<uint> _removeCarsOlderThanYear;
        private readonly MelonPreferences_Entry<bool> _isEnableBrandFilter;
        private readonly MelonPreferences_Entry<bool> _isEnableShop;
        private readonly MelonPreferences_Entry<bool> _isEnableAdditionalAuction;
        private readonly MelonPreferences_Entry<bool> _isEnableShopCarParts;
        private readonly MelonPreferences_Entry<string[]> _carNames;
        private readonly MelonPreferences_Entry<string[]> _whiteCarParts;

        private readonly MelonPreferences_Entry<string[]> _retroParts;

        private readonly MelonPreferences_Entry<string[]> _retroParts1950_1959;
        private readonly MelonPreferences_Entry<string[]> _retroParts1960_1969;
        private readonly MelonPreferences_Entry<string[]> _retroParts1970_1979;
        private readonly MelonPreferences_Entry<string[]> _retroParts1980_1990;
        private readonly MelonPreferences_Entry<string[]> _retroParts1991_2000;
        private readonly MelonPreferences_Entry<string[]> _retroParts2001_2005;
        private readonly MelonPreferences_Entry<string[]> _sportGT;
        private readonly MelonPreferences_Entry<string[]> _specialRetro;

        public KeyCode IsKeyBindEnableSwitchMode => _isKeyBindEnableSwitchMode.Value;
        public uint RemoveCarsOlderThanYear => _removeCarsOlderThanYear.Value;
        public string[] ListCarNames => _carNames.Value;
        public string[] ListRetroParts => _retroParts.Value;
        public string[] WhiteCarParts => _whiteCarParts.Value;
        public bool IsEnableBrandFilter => _isEnableBrandFilter.Value;

        public bool IsEnableShop => _isEnableShop.Value;
        public bool IsEnableAdditionalAuction => _isEnableAdditionalAuction.Value;
        public bool IsEnableShopCarParts => _isEnableShopCarParts.Value;

        public string[] ListRetroParts1950_1959 => _retroParts1950_1959.Value;
        public string[] ListRetroParts1960_1969 => _retroParts1960_1969.Value;
        public string[] ListRetroParts1970_1979 => _retroParts1970_1979.Value;
        public string[] ListRetroParts1980_1990 => _retroParts1980_1990.Value;
        public string[] ListRetroParts1991_2000 => _retroParts1991_2000.Value;
        public string[] ListRetroParts2001_2005 => _retroParts2001_2005.Value;
        public string[] ListSportGT => _sportGT.Value;
        public string[] ListSpecialRetro => _specialRetro.Value;

        public Config()
        {
            _config = MelonPreferences.CreateCategory(ConfigCategoryName);
            _config.SetFilePath("Mods/RealShop.cfg");
            _isKeyBindEnableSwitchMode = _config.CreateEntry(nameof(IsKeyBindEnableSwitchMode), KeyCode.F9);
            _isEnableShop = _config.CreateEntry(nameof(IsEnableShop), false);
            _isEnableAdditionalAuction = _config.CreateEntry(nameof(IsEnableAdditionalAuction), false);
            _isEnableShopCarParts = _config.CreateEntry(nameof(IsEnableShopCarParts), false);

            _carSection = MelonPreferences.CreateCategory(ConfigCarSection);
            _carSection.SetFilePath("Mods/RealShop.cfg");
            _removeCarsOlderThanYear = _carSection.CreateEntry(nameof(RemoveCarsOlderThanYear), RemoveCarsOlderYear);
            _whiteCarParts = _carSection.CreateEntry(nameof(WhiteCarParts), ListWhiteCarParts);
            _isEnableBrandFilter = _carSection.CreateEntry(nameof(IsEnableBrandFilter), true);
            _retroParts = _carSection.CreateEntry(nameof(ListRetroParts), RetroParts);

            _repair = MelonPreferences.CreateCategory(ConfigRepairSection);
            _repair.SetFilePath("Mods/RealShop.cfg");
            _carNames = _repair.CreateEntry(nameof(ListCarNames), CarNames);

            _category = MelonPreferences.CreateCategory(ConfigCategory);
            _category.SetFilePath("Mods/RealShop.cfg");

            _retroParts1950_1959 = _category.CreateEntry(nameof(ListRetroParts1950_1959), RetroParts1950_1959);
            _retroParts1960_1969 = _category.CreateEntry(nameof(ListRetroParts1960_1969), RetroParts1960_1969);
            _retroParts1970_1979 = _category.CreateEntry(nameof(ListRetroParts1970_1979), RetroParts1970_1979);
            _retroParts1980_1990 = _category.CreateEntry(nameof(ListRetroParts1980_1990), RetroParts1980_1990);
            _retroParts1991_2000 = _category.CreateEntry(nameof(ListRetroParts1991_2000), RetroParts1991_2000);
            _retroParts2001_2005 = _category.CreateEntry(nameof(ListRetroParts2001_2005), RetroParts2001_2005);

            _sportGT = _category.CreateEntry(nameof(ListSportGT), SportGT);
            _specialRetro = _category.CreateEntry(nameof(ListSpecialRetro), SpecialRetro);
        }

        public void Reload()
        {
            _config.LoadFromFile();
        }
    }
}

// ENGINE (I3)  2012 ????
// https://car-mechanic-simulator-2021.fandom.com/wiki/I3_DOHC
// The I3 DOHC is based on the Ford Fox 1.0 L of the EcoBoost family, this engine was introduced in 2012.
//"i3_blok", "i3_gearbox", "i3_glowica", "i3_glowica_2", "i3_glowica_wkladka", "i3_kolektor_dolotowy", "i3_kolektor_wydechowy", "i3_listwa_wtryskowa",
//"i3_miska_olejowa", "i3_oslona_rozrzadu", "i3_pasek1", "i3_pasek2", "i3_pokrywa_glowicy", "i3_przepustnica", "i3_rolkaWalka", "i3_termostat",
//"i3_turbo", "i3_walek_rozrzadu", "i3_walKorbowy", "w_i3_poczatkowy_2", "w_i3_poczatkowy", "i3_filtr_oleju", "t_i3_glowica", "t_i3_glowica_2",
//"t_i3_listwa_wtryskowa", "t_i3_przepustnica", "t_i3_turbo", "t_i3_walek_rozrzadu", "t_w_i3_poczatkowy", "t_i3_kolektor_dolotowy",
//"t_i3_kolektor_wydechowy", "t_w_i3_poczatkowy_2", "t_i3_gearbox",


// Engine (V8) - это базовая часть для других движ
// V8 DOHC AXK https://car-mechanic-simulator-2021.fandom.com/wiki/V8_DOHC_AXK
// V8 DOHC 4x4 https://car-mechanic-simulator-2021.fandom.com/wiki/V8_DOHC_4x4
// V8 DOHC Supercharged https://car-mechanic-simulator-2021.fandom.com/wiki/V8_DOHC_Supercharged
// V8 DOHC CGM  (V8 DOHC CGM) https://car-mechanic-simulator-2021.fandom.com/wiki/V8_DOHC_CGM (2014)
// V8 OHV CGM-SC M1DV8 OHV CGM-SC M1D https://car-mechanic-simulator-2021.fandom.com/wiki/V8_OHV_CGM-SC_M1D (2020)
// V8 OHV CGM M1D  https://car-mechanic-simulator-2021.fandom.com/wiki/V8_OHV_CGM_M1D  
// "pompa_wspomagania", "v8_blok", "v8_filtr_oleju", "v8_gearbox", "v8_glowica", "v8_kolektor", "v8_kolektor_wydechowy", "v8_kolo_pasowe_walu", "v8_miska_olejowa",
// "v8_pasek1",  "v8_pasek2b", "v8_pasek3", "v8_pokrywa_glowicy1", "v8_pokrywa_glowicy2", "v8_pokrywa_swiec", "v8_pompa_wody", "v8_przepustnica", "v8_przepustnica_2",
// "v8_rozrzad_oslona", "v8_walek_rozrzadu", "w_v8_koncowy", "w_v8_poczatkowy", "w_v8_poczatkowy_h1", "w_v8_poczatkowy_h2", "w_v8_poczatkowy_h3",
// "walKorbowy_1", "w_v8_poczatkowy_2", "v8_pokrywa_glowicy1B", "v8_pokrywa_glowicy2B", "v8_supercharger_pasek2b", "v8_supercharger_kolektor_dolotowy",
// "v8_supercharger", "w_v8_koncowy2", "t_w_v8_poczatkowy", "t_w_v8_poczatkowy_2", "t_v8_walek_rozrzadu", "t_pompa_wspomagania", "t_v8_przepustnica",
// "t_v8_kable_stary", "t_v8_glowica", "t_v8_kolektor_wydechowy", "t_w_v8_koncowy", "t_w_v8_koncowy2", "t_v8_przepustnica_2","t_w_v8_poczatkowy_h3",
// "t_w_v8_poczatkowy_h1", "t_w_v8_poczatkowy_h2", "t_v8_supercharger_kolektor_dolotowy", "t_v8_supercharger", "t_v8_kolektor",
// "t_v8_gearbox", "v8_kable_stary", "v8_rozrusznik_1",

// Engine  (V6 VQ37VHR) || 2008 - 2021 ?
// https://car-mechanic-simulator-2021.fandom.com/wiki/V6_DOHC_NSN
// The V6 DOHC NSN is based off the real life Nissan VQ37VHR 3.7L engine produced during 2008 to 2021.
// "v6_37n_blok", "v6_37n_dzwignia_zaworu_3", "v6_37n_dzwignia_zaworu_2", "v6_37n_gearbox", "v6_37n_glowica_1", "v6_37n_glowica_2", "v6_37n_glowica_wkladka",
// "v6_37n_kolektor_dolotowy", "v6_37n_kolektor_dolotowy_dolny", "v6_37n_kolektor_wydechowy", "v6_37n_lancuch_1", "v6_37n_lancuch_2", "v6_37n_listwa_wtryskowa",
// "v6_37n_miska_olejowa", "v6_37n_nakladka_glowica_2", "v6_37n_nakladka_glowica_1", "v6_37n_oslona_rozrzadu_1", "v6_37n_oslona_rozrzadu_2",
// "v6_37n_oslona_rozrzadu_3", "v6_37n_oslona_rozrzadu_4", "v6_37n_pasek", "v6_37n_pokrywa_glowicy_1", "v6_37n_pokrywa_glowicy_2",
// "v6_37n_pompa_wody", "v6_37n_przepustnica", "v6_37n_rolka_walka_1", "v6_37n_rolka_walka_2", "v6_37n_slizg_1", "v6_37n_slizg_2",
// "v6_37n_walek_rozrzadu_1", "v6_37n_walek_rozrzadu_2", "v6_37n_walKorbowy", "v6_37n_pokrywa_silnika_1", "v6_37n_pokrywa_silnika_2",
// "t_v6_37n_kolektor_wydechowy", "t_v6_37n_glowica_2", "t_v6_37n_listwa_wtryskowa", "t_v6_37n_przepustnica", "t_v6_37n_glowica_1", "t_v6_37n_kolektor_dolotowy",
// "t_v6_37n_kolektor_dolotowy_dolny", "t_v6_37n_walek_rozrzadu_1", "t_v6_37n_walek_rozrzadu_2", "t_v6_37n_gearbox", 