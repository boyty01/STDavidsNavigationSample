namespace StDavidsQRNavigation.Models
{
    public class TenantEventModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Representative { get; set; } = string.Empty; // optional: Specific person related to the event. e.g. Dr. Davies.
        public int TenantId { get; set; }
        public TenantModel? Tenant { get; set; }
        public int? InternalNavId { get; set; }
        public InternalNavPathModel? InternalNav { get; set; }
        public bool IsPublished { get; set; } = false; // Indicates if the event is published and visible to users.
    }

    public class TenantEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Representative { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public int? InternalNavId { get; set; }
    }
}
