using System.Text.Json;
using Bones.Shared;
using GeoJSON.Text.Feature;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class GeoJsonFeatureToStringConverter() : ValueConverter<Feature?, string?>(
        feature => feature == null ? null : JsonSerializer.Serialize(feature, StandardJsonSerializerOptions.Default),
        str => str == null ? null : JsonSerializer.Deserialize<Feature>(str, StandardJsonSerializerOptions.Default)
    );

