namespace RealShop
{
    public partial class Config
    {
        public static string[] ListCarPartsCantBeRepair = { };

        public static string[] ListWhiteCarParts = {
            "I4 DT224", "I4 204PT", "V8 AJ133", "V8 392 Hemi", "V10 OHV MT8", "V10 OHV", "V12 AM11", "B6 MA", "B6 M64", "eDen-1H",
            "Proton e-R", "Proton e-R","I4 DOHC BFM", "I4 SOHC ABT", "V6 DOHC VQ37VHR", "V6 VQ37VHR", "V8 M177 DBX", "V8 J",
            "V6 AJD", "Zonda", "V12 M120", "V12 M158", "CGM", "DBS", "DBX", "Avalanche", "991", "GTR", "330",
            "V8 AJ133", "F350", "Hellcat", "Crown", "Discovery", "EarthQuake", "Earthquake Rex", "Evoque",
            "Grand Club", "GW500", "Imperator", "M3", "Mojave", "Moon", "Mosquito", "Piccolo",
            "SVR", "Town", "Tyro", "Urs",
        };

        public static string[] RetroParts = { };

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
    }
}
