using System;

namespace DynRenga.Core.Interfaces
{
    /// <summary>
    /// Интерфейс для объектов Renga, имеющих ID и уникальный идентификатор
    /// </summary>
    public interface IRengaIdentifiable
    {
        /// <summary>
        /// Целочисленный идентификатор объекта
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Уникальный GUID идентификатор объекта
        /// </summary>
        Guid UniqueId { get; }
    }
}