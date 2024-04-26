using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace WebHttp
{
    [DataContract(Name = "GameContract", Namespace = "http://example.com")]
    public class GameContract
    {
        [DataMember(Name = "type", Order = 1)]
        [OpenApiProperty(Description = "command type")]
        public string type { get; set; }

        [DataMember(Name = "game_id", Order = 2)]
        [OpenApiProperty(Description = "game_id")]
        public string game_id { get; set; }

        [DataMember(Name = "game_item_id", Order = 3)]
        [OpenApiProperty(Description = "game_item_id")]
        public int game_item_id { get; set; }

        [DataMember(Name = "properties", Order = 4)]
        [OpenApiProperty(Description = "other properties")]
        public List<int> properties { get; set; }
    }
}
