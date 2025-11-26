using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class DateTimeOffsetUtcConverter() : ValueConverter<DateTimeOffset, DateTimeOffset>(
    // To DB
    dto => dto.ToUniversalTime(),
    // From DB
    dto => dto
);
