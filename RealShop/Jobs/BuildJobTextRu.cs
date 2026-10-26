using System;
using System.Collections.Generic;

namespace RealShop
{
    internal static class BuildJobTextRu
    {
        private static readonly Dictionary<string, string> Values = new Dictionary<string, string>
        {
            { "difficulty", "КРАЙНЕ СЛОЖНОЕ" },
            { "body", "КУЗОВ" },
            { "parts", "ДЕТАЛИ" },
            { "frame", "РАМА" },
            { "interior", "ИНТЕРЬЕР" },
            { "engine", "ДВИГАТЕЛЬ" },
            { "engineInstalled", "установлен" },
            { "engineMissing", "отсутствует" },
            { "reward", "цена готовой машины" },
            { "advance", "ПРЕДОПЛАТА" },
            { "advanceSuffix", "при принятии; удерживается из итоговой выплаты" },
            { "advancePopup", "Предоплата за заказ" },
            { "takePopup", "принято: собери автомобиль с нуля. Кузов, детали и интерьер имеют минимальное качество." },
            { "notReadyPrefix", "автомобиль не соответствует требованиям задания" },
            { "modOff", "мод выключен" },
            { "jobMissing", "задание не найдено" },
            { "carMissing", "автомобиль задания не найден в гараже" },
            { "unmounted", "снято деталей" },
            { "mounted", "установлено деталей" },
        };

        internal static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            string value;
            if (Values.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        internal static bool IsUsable(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            bool hasLetter = false;
            int latinSupplement = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '\uFFFD')
                {
                    return false;
                }

                if (c >= '\u00C0' && c <= '\u00FF')
                {
                    latinSupplement++;
                }

                if (char.IsLetter(c))
                {
                    hasLetter = true;
                }
            }

            if (!hasLetter)
            {
                return false;
            }

            return latinSupplement < 2;
        }
    }
}
