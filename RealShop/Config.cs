using CMS.UI.Logic.Paging;
using MelonLoader;
using UnityEngine;

namespace RealShop
{
    public class Config
    {
        public const string ModName = "RealShop";

        public static string[] ListWhiteCarParts = { 
            "I4 DT224", "I4 204PT", "V8 392 Hemi", "V10 OHV MT8", "V10 OHV", "V12 AM11", "B6 MA", "B6 M64", "eDen-1H",
            "Proton e-R", "Proton e-R","I4 DOHC BFM", "I4 SOHC ABT", "V6 DOHC VQ37VHR", "V8 M177 DBX", "V8 J",
            "V6 AJD", "Zonda", "V12 M120", "V12 M158", "CGM",
            "DBS", "DBX", "Avalanche", "991", "GTR", "330", "F350", "Hellcat", "Crown", "Discovery", "EarthQuake",
            "Earthquake Rex", "Evoque", "Grand Club", "GW500", "Imperator", "M3", "Mojave", "Moon", "Mosquito", "Piccolo",
            "SVR", "Town", "Tyro", "Urs",
        };

        public static string[] RetroParts = { 
            "(I6 B)", "(I6 DOHC BU2 T)", "(I6 DOHC BU2)",
            "(R6)", 
            "(V6 TWR JV6)", 
            "(V8 OHV B)", "(V8 OHV C)", "(V8 OHV F)", "(V8 OHV K)",
        };

        public static string[] RetroParts1950_1959 = {
            // DELRAY CUSTOM (1957)
            "car_delraycustom", 

            // SALEM 1500 (1955)
            "car_salem1500", 

            // Engine (V8)
            "pompa_wspomagania", "v8_blok", "v8_filtr_oleju", "v8_gearbox", "v8_glowica", "v8_kolektor", "v8_kolektor_wydechowy", "v8_kolo_pasowe_walu", "v8_miska_olejowa",
            "v8_pasek1",  "v8_pasek2b", "v8_pasek3", "v8_pokrywa_glowicy1", "v8_pokrywa_glowicy2", "v8_pokrywa_swiec", "v8_pompa_wody", "v8_przepustnica", "v8_przepustnica_2",
            "v8_rozrzad_oslona", "v8_walek_rozrzadu", "w_v8_koncowy", "w_v8_poczatkowy", "w_v8_poczatkowy_h1", "w_v8_poczatkowy_h2", "w_v8_poczatkowy_h3",
            "walKorbowy_1", "w_v8_poczatkowy_2", "v8_pokrywa_glowicy1B", "v8_pokrywa_glowicy2B", "v8_supercharger_pasek2b", "v8_supercharger_kolektor_dolotowy",
            "v8_supercharger", "w_v8_koncowy2", "t_w_v8_poczatkowy", "t_w_v8_poczatkowy_2", "t_v8_walek_rozrzadu", "t_pompa_wspomagania", "t_v8_przepustnica",
            "t_v8_kable_stary", "t_v8_glowica", "t_v8_kolektor_wydechowy", "t_w_v8_koncowy", "t_w_v8_koncowy2", "t_v8_przepustnica_2","t_w_v8_poczatkowy_h3",
            "t_w_v8_poczatkowy_h1", "t_w_v8_poczatkowy_h2", "t_v8_supercharger_kolektor_dolotowy", "t_v8_supercharger", "t_v8_kolektor",
            "t_v8_gearbox", "v8_kable_stary", "v8_rozrusznik_1",

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
            // SMITH&PARKER ARIZONA KID (1969)
            "car_smithandparkerarizonakid", 

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

            // ENGINE (I3)
            "i3_blok", "i3_gearbox", "i3_glowica", "i3_glowica_2", "i3_glowica_wkladka", "i3_kolektor_dolotowy", "i3_kolektor_wydechowy", "i3_listwa_wtryskowa",
            "i3_miska_olejowa", "i3_oslona_rozrzadu", "i3_pasek1", "i3_pasek2", "i3_pokrywa_glowicy", "i3_przepustnica", "i3_rolkaWalka", "i3_termostat",
            "i3_turbo", "i3_walek_rozrzadu", "i3_walKorbowy", "w_i3_poczatkowy_2", "w_i3_poczatkowy", "i3_filtr_oleju", "t_i3_glowica", "t_i3_glowica_2",
            "t_i3_listwa_wtryskowa", "t_i3_przepustnica", "t_i3_turbo", "t_i3_walek_rozrzadu", "t_w_i3_poczatkowy", "t_i3_kolektor_dolotowy",
            "t_i3_kolektor_wydechowy", "t_w_i3_poczatkowy_2", "t_i3_gearbox",
        };

        public static string[] RetroParts1970_1979 = {
            // SMITH&PARKER OUTLAW (1971)
            "car_smithandparkeroutlaw",

            // SMITH&PARKER ARIZONA ELIMINATOR (1977)
            "car_smithandparkereliminator", 

            // PLYMOUTH ROADRUNNER
            "car_plymouthroadrunner", "bench_roadrunner1", "bench_roadrunner2", "rim_roadrunner",

            // FORD MUSTANG MACH 1
            "car_fordmustangmach1", "seat_mach", "steering_wheel_mach", "t_v8_kolo_zamachowe", "rim_mach",

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

            // Engine (V8  OHV)
            "v8_blok_stary", "v8_filtr_oleju_stary", "v8_gearbox_stary", "v8_glowica_stara", "v8_kolektor_wydechowy_stary_1",
            "v8_kolo_pasowe_walu_stare", "v8_lancuch_stary", "v8_miska_olejowa_stara", "v8_pasek_stary_1", "v8_pasek_stary_2",
            "v8_pokrywa_glowicy_stara_1", "v8_pokrywa_glowicy_stara_2", "v8_pompaWody_stara", "v8_rolkaWalka_stara", "v8_rozrzad_oslona_stara",
            "v8_walek_stary", "v8_walKorbowy_stary", "w_v8_koncowy_stary", "w_v8_poczatkowy_stary", "w_v8_poczatkowy_stary_h1",
            "w_v8_poczatkowy_stary_h2", "w_v8_poczatkowy_stary_h3", "wentylator_2", "v8_supercharger_stary", "v8_supercharger_kolektor_dolotowy_stary",
            "v8_supercharger_pasek_stary", "rolka_pompy_wody_supercharger", "v8_filtr_1", "v8_scoop_1", "v8_gearbox_stary3",
            "t_w_v8_poczatkowy_stary", "t_v8_kolektor_wydechowy_stary_1", "t_v8_glowica_stara", "t_w_v8_koncowy_stary",
            "t_v8_walek_stary", "t_w_v8_poczatkowy_stary_h2", "t_w_v8_poczatkowy_stary_h3", "t_v8_supercharger_stary", "t_v8_supercharger_kolektor_dolotowy_stary",
            "t_w_v8_poczatkowy_stary_h1", "t_v8_filtr_1", "t_v8_gearbox_stary", "t_v8_gearbox_stary3",
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
            "steering_wheel_fordgt40", "steering_wheel_xj220", "steering_wheel_etype", "steering_wheel_ftype", "steering_wheel_500e",
            "steering_wheel_winchester", "steering_wheel_charger", "steering_wheel_cuda", "steering_wheel_elise", "steering_wheel_240z",
            "steering_wheel_gullwing", "steering_wheel_pagoda"
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

            // ENGINE (V12 HE)
            "v12_xjs_blok", "v12_xjs_gearbox", "v12_xjs_glowica_1", "v12_xjs_glowica_2", "v12_xjs_kolektor_dolotowy_1", "v12_xjs_kolektor_dolotowy_2",
            "v12_xjs_kolektor_wydechowy_1", "v12_xjs_kolektor_wydechowy_2", "v12_xjs_lancuch", "v12_xjs_listwa_wtryskowa", "v12_xjs_miska_olejowa", 
            "v12_xjs_nakladka_glowica_1", "v12_xjs_nakladka_glowica_2","v12_xjs_oslona_rozrzadu","v12_xjs_pasek_1","v12_xjs_pasek_2",
            "v12_xjs_pokrywa_glowicy_1","v12_xjs_pokyrwa_glowicy_2","v12_xjs_pompa_wody","v12_xjs_walek_rozrzadu","v12_xjs_walKorbowy",
            "v12_xjs_nakladka_glowicy_1","v12_xjs_obudowa_filtra_dol","v12_xjs_obudowa_filtra_gora","v12_xjs_obudowa_filtra_srodek","t_v12_xjs_kopulkaRozdzielacza",
            "t_v12_xjs_kolektor_wydechowy_2", "t_v12_xjs_kolektor_dolotowy_1", "t_v12_xjs_glowica_2", "t_v12_xjs_walek_rozrzadu", "t_v12_xjs_glowica_1",
            "t_v12_xjs_kolektor_dolotowy_2", "t_v12_xjs_kolektor_wydechowy_1", "t_v12_xjs_kable", "t_v12_xjs_obudowa_filtra_srodek", "t_v12_xjs_rozdzielaczZaplonu",
            "t_v12_xjs_listwa_wtryskowa", "t_v12_xjs_gearbox", "v12_xjs_kable", "v12_xjs_kopulkaRozdzielacza", "v12_xjs_rozdzielaczZaplonu",

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

            // LOTUS ELISE S1 (1994)
            "car_lotuselises1",

            // SAKURA SUPA (1993)
            "car_sakurasupa", "bench_supa",

            // RIBBSAN STARLINE (1998)
            "car_ribbsanstarline", "bench_starline",

            // SALEM OVERKING (1998)
            "car_salemoverking", "seat_overking", "seat_overking2", "seat_overking3", "steering_wheel_overking", "bench_overking",

            // VALLSEN 2040 (1993)
            "car_vallsen2040", "seat_vallsen", "seat_vallsen2", "bench_vallsen",

            // ENIGNE (I4 907)
            "r4_old_blok", "r4_old_gearbox", "r4_old_glowica", "r4_old_glowica_2_1","r4_old_glowica_2_2", "r4_old_kolektor_dolotowy",
            "r4_old_kolektor_wydechowy", "r4_old_pasek_1", "r4_old_pasek_2", "r4_old_walKorbowy", "r4_old_pokrywa_glowicy_1", 
            "r4_old_pokrywa_glowicy_2", "r4_old_miska_olejowa", "r4_old_puszkaFiltra_tyl", "r4_old_puszkaFiltra_przod",
            "t_r4_old_glowica", "t_r4_old_glowica_2_1", "t_r4_old_glowica_2_2", "t_r4_old_kable", "t_r4_old_kolektor_dolotowy",
            "t_r4_old_kolektor_wydechowy", "t_r4_old_kopulkaRozdzielacza", "t_r4_old_gearbox", "r4_old_kable", "r4_old_kopulkaRozdzielacza", 
            
            // SEAT and etc BELT
            "seat_atlanta", "seat_cuda", "seat_gullwing", "seat_sec",  "seat_bandit", "seat_typhoon", "seat_reptiliar2", 
            "seat_rollett", "seat_tbx", "seat_charger", "seat_challenger", "seat_elise", "seat_esprit",
            "seat_fordgt40", "seat_mach", "seat_carreragt", "seat_964", "seat_991", 
            "seat_500e", "seat_pagoda", "seat_chrysler_cordoba", "seat_300c", "seat_gw500", "seat_highroad",
            "seat_hornet", "seat_mk7", "seat_mosquito", "seat_mosquito2016", "seat_overking", "seat_renton", "seat_supernova", "seat_vallsen", "seat_wildcat",
            "seat_zephyr", "seat_240z", "seat_370z", "seat_Silvia_S15", "seat_etype", "seat_xj220", "seat_xjs", 
            "seat_db5", "seat_1_leather5", "seat_2_leather5", "seat_3_leather5",
            "seat_4_leather5", "seat_5_leather5", "seat_6_leather5", "seat_7_leather5", "seat_8_leather5", "seat_9_leather5", "seat_10_leather5", "seat_11_leather5",
            "seat_1_leather6", "seat_2_leather6", "seat_3_leather6", "seat_4_leather6", "seat_5_leather6", "seat_6_leather6", "seat_7_leather6", "seat_8_leather6",
            "seat_9_leather6", "seat_10_leather6", "seat_11_leather6", "seat_gw500_2", "seat_highroad2", "seat_hornet2", "seat_mk7_2", "seat_mosquito2","seat_mosquito2016_2",
            "seat_overking2", "seat_supernova2", "seat_vallsen2", "seat_wildcat2", "seat_zephyr2", "seat_typhoon2", "seat_gw500_3", "seat_highroad3", "seat_hornet3",
            "seat_mk7_3", "seat_mosquito3", "seat_mosquito2016_3", "seat_overking3", "seat_supernova3", "seat_wildcat3", "seat_zephyr3", "seat_typhoon3",
            "bench_custom",
            "bench_custom_leather1", "bench_custom_leather2", "bench_magnum", "bench_magnum_leather1", "bench_magnum_leather2", "bench_mayenm6", "bench_mayenm6_leather1",
            "bench_mayenm6_leather2", "bench_mayenm8", "bench_mayenm8_leather1", "bench_mayenm8_leather2", 
            "bench_moon", "bench_moon_leather1", "bench_moon_leather2", 
            "bench_sec", "bench_katsumoto", "bench_katsumoto_leather1", "bench_katsumoto_leather2",
            "bench_kieran", "bench_kieran_leather1", "bench_kieran_leather2", "bench_flamo", "bench_flamo_leather1", "bench_flamo_leather2", 
            "bench_fastback", "bench_fastback_leather1", "bench_fastback_leather2", "bench_cuda", 
            "bench_hellcat", "bench_hellcat_leather1", "bench_hellcat_leather2", "bench_bowen1",
            "bench_bowen2", "bench_cape1", "bench_cape2", "bench_chapman1", "bench_chapman2", "bench_bandit", "bench_rollett", "bench_tbx", 
            "bench_roadrunner1", "bench_roadrunner2", "bench_charger", "bench_rx-3", "bench_rx-7", "bench_challenger",
            "bench_991", "bench_lx550", "bench_500e", "bench_chrysler_cordoba",
            "bench_300c", "bench_baronne1", "bench_baronne2", "bench_goldtrail1", "bench_goldtrail2", "bench_highroad", "bench_hornet",
            "bench_mk7", "bench_mosquito", "bench_mosquito2016", "bench_overking", "bench_supernova", "bench_vallsen", "bench_wildcat", "bench_zephyr",
            "bench_cebulion",  "bench_jager", "bench_Panther_MK1", "bench_sentinel", "bench_kagura_sx", "bench_silvia_s15", "bench_tamago",
            "bench_supa", "bench_winchester1", "bench_winchester2", "bench_takumi", "bench_starline", "bench_xjs",
            "bench_earthquake", "bench_earthquake_rex", "bench_earthquake_rex_leather1", "bench_earthquake_rex_leather2", "bench_earthquake_leather1",
            "bench_earthquake_leather2",
        };

        public static string[] RetroParts2001_2005 = {
            // CHRYSLER 300C SRT8
            "car_chrysler300csrt8", "w_v8_61hemi_poczatkowy_chrysler300c", "w_v8_61hemi_koncowy__chrysler300c", "seat_300c",
            "steering_wheel_300c", "bench_300c", "t_w_v8_61hemi_poczatkowy_chrysler300c", "t_w_v8_61hemi_koncowy__chrysler300c", "rim_300c",

            // FMW PANTHER MK2 (2003)
            "car_fmwpanther", "bench_Panther_MK1",

            // ELENTI CALLOPE (2003)
            "car_elenticallope", "bench_FMW_Panther", "bench_FMW_Panther_leather1", "bench_FMW_Panther_leather2",

            // MAYEN M6 (2002)
            "car_mayenm6", "bench_mayenm6","bench_mayenm6_leather1","bench_mayenm6_leather2",

            // MAYEN M8 (2003)
            "car_mayenm8", "bench_mayenm8","bench_mayenm6_leather8","bench_mayenm6_leather8",

            // RIBBSAN Z530 (2003)
            "car_ribbsanz530", "w_v6_37n_koncowy_ribbsanz530", "t_w_v6_37n_koncowy_ribbsanz530",

            // SCEO LX550 (2003)
            "car_sceolx550", "LX550",

            // SIXON CEBULION (2003)
            "car_sixoncebulion", "bench_cebulion",
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

        };

        public static string[] SpecialRetro = {
            //  Mercedes-Benz W198 300SL (1954)
            "car_mercedes300slgullwing",

            // Aston martin DB5 (1963)         
            "car_astonmartindb5", "w_r6_tm_koncowy_1_astonmartindb5", "w_r6_tm_koncowy_2_astonmartindb5", "w_r6_tm_poczatkowy_1_astonmartindb5", 
            "w_r6_tm_poczatkowy_2_astonmartindb5", "w_r6_tm_srodkowy_1_astonmartindb5", "w_r6_tm_srodkowy_2_astonmartindb5", "seat_db5",
            "steering_wheel_db5", "bench_db5", "t_w_r6_tm_koncowy_1_astonmartindb5", "t_w_r6_tm_koncowy_2_astonmartindb5", "t_w_r6_tm_poczatkowy_1_astonmartindb5",
            "t_w_r6_tm_poczatkowy_2_astonmartindb5", "t_w_r6_tm_srodkowy_1_astonmartindb5", "t_w_r6_tm_srodkowy_2_astonmartindb5", "rim_db5",

            // JUEYUAN QIANFENG (1959)
            "car_jueyuanqianfeng", 

            // DC TYPHOON (1964)
            "car_dctyphoon", "seat_typhoon", "seat_typhoon2", "seat_typhoon3", "steering_wheel_typhoon", "rim_typhoon",

            // FORD GT 40 (1965)
            "car_fordgt40", "seat_fordgt40", "steering_wheel_fordgt40", "rim_fordgt40",

            // JAGUAR E-TYPE (1961)
            "car_jaguaretype", "w_r6_xk_koncowy_jaguaretype", "w_r6_xk_poczatkowy_A_jaguaretype", "w_r6_xk_poczatkowy_B_jaguaretype",
            "t_w_r6_xk_poczatkowy_A_jaguaretype", "t_w_r6_xk_koncowy_jaguaretype", "t_w_r6_xk_poczatkowy_B_jaguaretype",

            // JAGUAR XJ220 (1992)
            "car_jaguarxj220", "w_v6_35tt_koncowy_jaguarxj220", "w_v6_35tt_poczatkowy_A_jaguarxj220", "w_v6_35tt_poczatkowy_B_jaguarxj220", "seat_xj220",
            "steering_wheel_xj220", "t_w_v6_35tt_poczatkowy_B_jaguarxj220", "t_w_v6_35tt_poczatkowy_A_jaguarxj220", "t_w_v6_35tt_koncowy_jaguarxj220", "rim_xj220_front",
            "rim_xj220_back",

            // LOTUS ESPRIT S1 (1975)
            "car_lotusesprits1", "w_r4_old_koncowy_esprit", "w_r4_old_poczatkowy_esprit", "seat_esprit", "steering_wheel_esprit", "t_w_r4_old_koncowy_esprit", 
            "t_w_r4_old_poczatkowy_esprit", "rim_esprit",

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
