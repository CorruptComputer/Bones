using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class IpAddressToStringConverter() : ValueConverter<IPAddress, string>(
    // To DB
    ip => ip == null || ip.Equals(IPAddress.None)
        ? string.Empty
        : ip.ToString(),
    // From DB
    str => string.IsNullOrEmpty(str)
        ? IPAddress.None
        : IPAddress.Parse(str)
);