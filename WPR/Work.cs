using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPR
{
    /// <summary>
    /// Содержит процедуры и функции для различных вычислений
    /// </summary>
    public static class Work
    {

        private static readonly DataTable Table = new(); // Калькулятор

        /// <summary>
        /// Вычислить числовое значение объекта
        /// </summary>
        public static double Va(object value)
        {
            string parse = value.ToString()?.Replace(".", ",");
            if (double.TryParse(parse, out double result))
            {
                return result;
            }
            return 0.0;
        }
        /// <summary>
        /// Вычислить целое значение объекта
        /// </summary>
        public static int VaInt(object value)
        {
            return (int)Math.Round(Va(value));
        }

        /// <summary>
        /// Вычисляет значение строки
        /// </summary>
        /// <param name="Text">Строка для вычисления</param>
        /// <param name="result">Результат вычисления (при ошибке - 0)</param>
        /// <param name="DecimalPlases">Количество десятичных знаков</param>
        /// <returns>True, если строка имела верный формат и успешно вычислилась</returns>
        public static bool Calculator(string Text, out double result, int DecimalPlases = -1)
        {
            result = 0.0;
            if (string.IsNullOrEmpty(Text))
            {
                return false;
            }
            try
            {
                result = Va(Table.Compute(Text.Replace(",", ".").Trim(), null));
                if (DecimalPlases > -1)
                {
                    result = Math.Round(result, DecimalPlases);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Десериализация объекта из файла
        /// </summary>
        public static T LoadObject<T>(string FileName)
        {
            return LoadObject<T>(File.ReadAllBytes(FileName));
        }
        /// <summary>
        /// Десериализация объекта
        /// </summary>
        public static T LoadObject<T>(byte[] ByteArray)
        {
            var readOnlySpan = new ReadOnlySpan<byte>(ByteArray);
            return JsonSerializer.Deserialize<T>(readOnlySpan);
        }
        /// <summary>
        /// Сериализация объекта и запись в файл
        /// </summary>
        public static void SaveObject(object obj, string FileName)
        {
            File.WriteAllBytes(FileName, SaveObject(obj));
        }

        /// <summary>
        /// Сериализация объекта
        /// </summary>
        public static byte[] SaveObject(object obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.SerializeToUtf8Bytes(obj, options);
        }

        /// <summary> Создать копию объекта через сериализацию </summary>
        public static T CloneObject<T>(this T obj)
        {
            return LoadObject<T>(SaveObject(obj));
        }
    }

}

