using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bones.Database.Converters;

internal class IpAddressToStringConverter() : ValueConverter<IPAddress, string>(
        ip => ip.ToString(),
        str => IPAddress.Parse(str)
    );