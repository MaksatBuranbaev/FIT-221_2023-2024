// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace SpaceBattle.Lib;
{
    [DataContract(Name = "GameContract", Namespace = "http://example.com")]
    internal class GameContract
    {
        [DataMember(Name = "type", Order = 1)]
        [OpenApiProperty(Description = "command type")]
        public string type { get; set; }

        [DataMember(Name = "game_id", Order = 2)]
        [OpenApiProperty(Description = "game_id")]
        public string game_id { get; set; }

        [DataMember(Name = "game_item_id", Order = 3)]
        [OpenApiProperty(Description = "game_item_id")]
        public string game_item_id { get; set; }

        [DataMember(Name = "properties", Order = 4)]
        [OpenApiProperty(Description = "other propertiesы")]
        public List<int> properties { get; set; }
    }
}
