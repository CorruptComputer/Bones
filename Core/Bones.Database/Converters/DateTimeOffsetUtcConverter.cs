using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class DateTimeOffsetUtcConverter()
    : ValueConverter<DateTimeOffset, DateTimeOffset>(dto => dto.ToUniversalTime(), dto => dto);
