using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ShadowGaze.Data.Services.Database.Extensions;

public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Конвертер: сохраняем в БД UTC с DateTimeKind.Unspecified,
    /// при чтении всегда получаем Unspecified.
    /// </summary>
    public static PropertyBuilder<DateTime> HasUtcConversion(
        this PropertyBuilder<DateTime> propertyBuilder)
    {
        var converter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Unspecified),
            v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified));
        return propertyBuilder.HasConversion(converter);
    }
    
}