using System.Text.Json;
using Bones.Shared;
using GeoJSON.Text.Feature;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class GeoJsonFeatureCollectionToStringConverter() : ValueConverter<FeatureCollection?, string?>(
        feature => feature == null ? null : JsonSerializer.Serialize(feature, StandardJsonSerializerOptions.Default),
        str => str == null ? null : JsonSerializer.Deserialize<FeatureCollection>(str, StandardJsonSerializerOptions.Default)
    );
    