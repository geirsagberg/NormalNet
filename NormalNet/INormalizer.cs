using System.Collections.Generic;

namespace NormalNet
{
    public interface INormalizer
    {
        Dictionary<string, object> Normalize(object obj);
    }
}