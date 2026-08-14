using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SGTitansManager.Server.Database;

public class UtcToLocalDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcToLocalDateTimeConverter() : base(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime())
    {
    }
}

public class UtcToLocalNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public UtcToLocalNullableDateTimeConverter() : base(
        v => v.HasValue ? v.Value.ToUniversalTime() : v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc).ToLocalTime() : v)
    {
    }
}