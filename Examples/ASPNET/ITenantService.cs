namespace StDavidsQRNavigation.Services
{
    public interface ITenantService
    {
        public Task<ServiceResult<bool>> AssignNavPathToEvent(int tenantEventId, int navPathId);
    }
}
