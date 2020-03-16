using System.Collections.Generic;
using Todo.Models.TodoItems;

namespace Todo.Services
{
    public interface IGravatarCache
    {
        IDictionary<string, GravatarProfile> GravatarProfiles { get; }
    }

    public class GravatarCache : IGravatarCache
    {
        public IDictionary<string, GravatarProfile> GravatarProfiles { get; }

        public GravatarCache()
        {
            GravatarProfiles = new Dictionary<string, GravatarProfile>();
        }
    }
}
