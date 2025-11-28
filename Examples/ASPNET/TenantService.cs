using Microsoft.EntityFrameworkCore;
using StDavidsQRNavigation.Database;
using System.Net;

namespace StDavidsQRNavigation.Services
{
    public class TenantService : ITenantService
    {

        private readonly DbNavigationContext _dbContext;

        public TenantService(DbNavigationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResult<bool>> AssignNavPathToEvent(int tenantEventId, int navPathId)
        {
            Console.WriteLine($"Linking Event {tenantEventId} to Path {navPathId}");
            var tenantEvent = await _dbContext.TenantEvents.FindAsync(tenantEventId);

            if (tenantEvent == null)
                return ServiceResult<bool>.Failure(HttpStatusCode.NotFound, "Not Found");

            tenantEvent.InternalNavId = navPathId;
            await _dbContext.SaveChangesAsync();
            return ServiceResult<bool>.Success(HttpStatusCode.OK, true);
        }
    }
}
