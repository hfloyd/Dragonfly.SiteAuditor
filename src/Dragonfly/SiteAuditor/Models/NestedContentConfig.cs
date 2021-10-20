namespace Dragonfly.SiteAuditor.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    class NestedContentConfig
    {
        [JsonProperty(propertyName: "confirmDeletes")]
        public int ConfirmDeletes { get; set; }

        [JsonProperty(propertyName: "contentTypes")]
        public IEnumerable<NestedContentContentTypesConfigItem> ContentTypes { get; set; }

        [JsonProperty(propertyName: "hideLabel")]
        public int HideLabel { get; set; }


        [JsonProperty(propertyName: "maxItems")]
        public int MaxItems { get; set; }


        [JsonProperty(propertyName: "minItems")]
        public int MinItems { get; set; }


        [JsonProperty(propertyName: "showIcons")]
        public int ShowIcons { get; set; }

        public NestedContentConfig(IDictionary<string, object> AdditionalDataDictionary)
        {
            this.ConfirmDeletes = (int)AdditionalDataDictionary["confirmDeletes"];
            this.ContentTypes = (IEnumerable<NestedContentContentTypesConfigItem>)AdditionalDataDictionary["contentTypes"];
            this.HideLabel = (int)AdditionalDataDictionary["hideLabel"];
            this.MaxItems = (int)AdditionalDataDictionary["maxItems"];
            this.MinItems = (int)AdditionalDataDictionary["minItems"];
            this.ShowIcons = (int)AdditionalDataDictionary["ShowIcons"];
        }
    }

    public class NestedContentContentTypesConfigItem
    {
        [JsonProperty(propertyName: "ncAlias")]
        public string DocTypeAlias { get; set; }

        [JsonProperty(propertyName: "ncTabAlias")]
        public string DocTypeTabAlias { get; set; }

        [JsonProperty(propertyName: "nameTemplate")]
        public string ItemNameTemplate { get; set; }

    }


}
