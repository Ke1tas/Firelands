using VetClinic.Domain.Entities;

namespace VetClinic.Domain.Interfaces;

/// <summary>
/// Обобщенный интерфейс репозитория для базовых операций
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public interface IRepository<T> where T : class, IEntity
{
    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <returns>Коллекция сущностей</returns>
    public IEnumerable<T> GetAll();

    /// <summary>
    /// Получить сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Найденная сущность или пустое значение</returns>
    public T? GetById(int id);

    /// <summary>
    /// Добавить новую сущность
    /// </summary>
    /// <param name="entity">Добавляемая сущность</param>
    public void Add(T entity);

    /// <summary>
    /// Обновить существующую сущность
    /// </summary>
    /// <param name="entity">Обновляемая сущность</param>
    public void Update(T entity);

    /// <summary>
    /// Удалить сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Признак успешного удаления</returns>
    public bool Delete(int id);
}