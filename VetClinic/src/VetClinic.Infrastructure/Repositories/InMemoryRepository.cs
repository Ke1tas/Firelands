using VetClinic.Domain.Entities;
using VetClinic.Domain.Interfaces;

namespace VetClinic.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для хранения данных в памяти
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items;
    private readonly object _lock = new();
    private int _currentId;

    /// <summary>
    /// Инициализация репозитория начальными данными
    /// </summary>
    /// <param name="initialData">Коллекция начальных данных</param>
    public InMemoryRepository(IEnumerable<T> initialData)
    {
        _items = [.. initialData];
        _currentId = _items.Count > 0 ? _items.Max(x => x.Id) : 0;
    }

    /// <inheritdoc />
    public IEnumerable<T> GetAll()
    {
        lock (_lock)
        {
            return [.. _items];
        }
    }

    /// <inheritdoc />
    public T? GetById(int id)
    {
        lock (_lock)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    /// <inheritdoc />
    public void Add(T entity)
    {
        lock (_lock)
        {
            entity.Id = ++_currentId;
            _items.Add(entity);
        }
    }

    /// <inheritdoc />
    public void Update(T entity)
    {
        lock (_lock)
        {
            var index = _items.FindIndex(x => x.Id == entity.Id);
            if (index != -1)
            {
                _items[index] = entity;
            }
        }
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        lock (_lock)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            return item is not null && _items.Remove(item);
        }
    }
}