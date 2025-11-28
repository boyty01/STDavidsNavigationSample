
using Microsoft.EntityFrameworkCore;
using StDavidsQRNavigation.Database;
using StDavidsQRNavigation.Models;
using System.Net;

namespace StDavidsQRNavigation.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbNavigationContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DbNavigationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<ServiceResult<T>> Create(T model)
        {
            try
            {
                await _dbSet.AddAsync(model);
                await _context.SaveChangesAsync();
                return ServiceResult<T>.Success(HttpStatusCode.Created, model);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return ServiceResult<T>.Failure(HttpStatusCode.BadRequest, null);
            }
        }

        public async Task<ServiceResult<bool>> Delete(int id)
        {
            try
            {
                Console.WriteLine($"Deleting Id {id}");
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return ServiceResult<bool>.Failure(HttpStatusCode.NotFound, "Not Found");
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<bool>.Failure(HttpStatusCode.BadRequest, "Bad request");
            }

        }

        public async Task<ServiceResult<T>> Get(int id, bool includeRelated = false)
        {
            // if includeRelated is true, we will build a deep relationship query
            if (includeRelated)
            {
                var query = BuildDeepRelationshipQuery(_dbSet.AsQueryable());
                try
                {
                    var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
                    if (entity == null)
                    {
                        return ServiceResult<T>.Failure(HttpStatusCode.NotFound, "Not Found");
                    }
                    return ServiceResult<T>.Success(HttpStatusCode.OK, entity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return ServiceResult<T>.Failure(HttpStatusCode.BadRequest, "Bad Request");
                }
            }

            // if includeRelated is false, we will just return the entity without any relationships
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return ServiceResult<T>.Failure(HttpStatusCode.NotFound, "Not Found");
                }

                return ServiceResult<T>.Success(HttpStatusCode.OK, entity);
            }
            catch (Exception)
            {
                return ServiceResult<T>.Failure(HttpStatusCode.BadRequest, "Bad Request");
            }
        }

        public async Task<ServiceResult<List<T>>> GetAll(bool includeRelated = false)
        {
            // if includeRelated is true, we will build a deep relationship query
            if (includeRelated)
            {
                var query = BuildDeepRelationshipQuery(_dbSet.AsQueryable());
                try
                {
                    var list = await query.ToListAsync();
                    return ServiceResult<List<T>>.Success(HttpStatusCode.OK, list);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return ServiceResult<List<T>>.Failure(HttpStatusCode.BadRequest, "Something went wrong.");
                }
            }

            // if includeRelated is false, we will just return the list without any relationships
            try
            {
                var list = await _dbSet.ToListAsync();
                return ServiceResult<List<T>>.Success(HttpStatusCode.OK, list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<List<T>>.Failure(HttpStatusCode.BadRequest, "Something went wrong.");
            }
        }

        public async Task<ServiceResult<T>> Update(T model)
        {
            var entity = await _dbSet.FindAsync(GetId(model));
            if (entity == null)
                return ServiceResult<T>.Failure(HttpStatusCode.NotFound, "Entity not found");

            _context.Entry(entity).CurrentValues.SetValues(model);
            await _context.SaveChangesAsync();

            return ServiceResult<T>.Success(HttpStatusCode.OK, entity);
        }

        private int GetId(T model)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null)
                throw new InvalidOperationException("No Id property found");

            return (int)(prop.GetValue(model) ?? 0);
        }

        public IQueryable<T> BuildDeepRelationshipQuery(IQueryable<T> values)
        {
            if (typeof(T) == typeof(BuildingModel))
            {
                var castValues = values.Cast<BuildingModel>();
                return (IQueryable<T>)castValues.Include(b => b.CarPark);
            }

            if (typeof(T) == typeof(TenantModel))
            {
                var castValues = values.Cast<TenantModel>();
                return (IQueryable<T>)castValues
                    .Include(t => t.Building)
                        .ThenInclude(b => b.CarPark);
            }

            if (typeof(T) == typeof(TenantEventModel))
            {
                var castValues = values.Cast<TenantEventModel>();
                return (IQueryable<T>)castValues
                    .Include(te => te.InternalNav)
                        .ThenInclude(nav => nav.Directions)
                    .Include(te => te.Tenant)
                        .ThenInclude(t => t.Building)
                            .ThenInclude(b => b.CarPark);
            }

            if (typeof(T) == typeof(InternalNavPathModel))
            {
                var castValues = values.Cast<InternalNavPathModel>();
                return (IQueryable<T>)castValues.Include(nav => nav.Directions);
            }

            return values; // Default case, return as is
        }
    }
}
