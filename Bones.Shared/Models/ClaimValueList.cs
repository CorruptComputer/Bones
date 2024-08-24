using System.Text.Json;

namespace Bones.Shared.Models;

public class ClaimValueList : List<string>
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public static implicit operator string(ClaimValueList value)
    {
        return value.ToString();
    }

    public static implicit operator ClaimValueList?(string value)
    {
        return JsonSerializer.Deserialize<ClaimValueList>(value);
    }
}