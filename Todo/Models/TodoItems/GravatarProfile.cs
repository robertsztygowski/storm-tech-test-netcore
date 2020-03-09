using Newtonsoft.Json;
using System.Collections.Generic;

namespace Todo.Models.TodoItems
{
    public class GravatarProfile
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        public bool DoesExist { get; set; }
    }

    public class GravatarEntry
    {
        [JsonProperty(PropertyName = "entry")]
        public IList<GravatarProfile> GravatarProfiles { get; set; }
    }
}
