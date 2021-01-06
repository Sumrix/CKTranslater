namespace Core.Collections
{
    /// <summary>
    ///     Множество флагов
    /// </summary>
    public struct FlagSet
    {
        /// <summary>
        ///     Инициализирует объект целочисленным значением
        /// </summary>
        /// <param name="value">Инициализируемое значение объекта</param>
        public FlagSet(uint value)
        {
            this.Value = value;
        }

        /// <summary>
        ///     Значение флагов
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        ///     Проверить флаг
        /// </summary>
        /// <param name="flag">Проверяемый флаг</param>
        /// <returns>True - флаг установлен, False - нет</returns>
        public bool Has(uint flag)
        {
            return (this.Value & flag) != 0;
        }

        /// <summary>
        ///     Установить флаг
        /// </summary>
        /// <param name="flag">Устанавливаемый флаг</param>
        public void Set(uint flag)
        {
            this.Value |= flag;
        }

        /// <summary>
        ///     Сбросить флаг
        /// </summary>
        /// <param name="flag">Сбрасываемый флаг</param>
        public void Reset(uint flag)
        {
            this.Value &= flag;
        }

        /// <summary>
        ///     Проверить на равество с объектом
        /// </summary>
        /// <param name="obj">Объект равество с которым проверяется</param>
        /// <returns>True - объекты равны, False - нет</returns>
        public override bool Equals(object obj)
        {
            return obj is FlagSet other && this.Value == other.Value;
        }

        /// <summary>
        ///     Получить хэш-код объекта
        /// </summary>
        /// <returns>Хэш-код объекта</returns>
        public override int GetHashCode()
        {
            return (int) this.Value;
        }

        /// <summary>
        ///     Проверить на равество два FlagSet-а
        /// </summary>
        /// <returns>True - объекты равны, False - нет</returns>
        public static bool operator ==(FlagSet left, FlagSet right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        ///     Проверить на неравество два FlagSet-а
        /// </summary>
        /// <returns>True - объекты не равны, False - равны</returns>
        public static bool operator !=(FlagSet left, FlagSet right)
        {
            return left.Value != right.Value;
        }
    }
}