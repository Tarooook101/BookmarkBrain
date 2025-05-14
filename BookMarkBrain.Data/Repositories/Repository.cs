﻿using BookMarkBrain.Core.Entities;
using BookMarkBrain.Core.RepoInterfaces;
using BookMarkBrain.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BookMarkBrain.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                               string includeString = null,
                                               bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (disableTracking)
            query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString))
            query = query.Include(includeString);

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                               List<Expression<Func<T, object>>> includes = null,
                                               bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (disableTracking)
            query = query.AsNoTracking();

        if (includes != null)
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
