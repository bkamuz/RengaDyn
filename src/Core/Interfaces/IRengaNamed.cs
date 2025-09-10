namespace DynRenga.Core.Interfaces
{
    /// <summary>
    /// Интерфейс для объектов Renga, имеющих имя
    /// </summary>
    public interface IRengaNamed
    {
        /// <summary>
        /// Имя/название объекта
        /// </summary>
        string Name { get; }
    }
}