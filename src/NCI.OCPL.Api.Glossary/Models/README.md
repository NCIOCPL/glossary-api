## Serialization Notes

By default, .NET uses `PascalCase` when performing JSON serialization. WebApis change this to `camelCase`. So, we need to get fancy.

Property names are stored in the Elasticsearch database using `snake_case` naming conventions.  The `NCI.OCPL.Api.Common` package accommodates this by overriding the serializer's property naming policy when creating the `ElasticsearchClient` instance.  (For unit tests, the `NCI.OCPL.Api.Common.Testing` package creates the `NciElasticsearchClientSettingsFactory` which provides the same functionality.)

For backwards compatibility, property names in the `relatedResources` and `media` arrays are presented to the end user with `PascalCase`.  For related resources, the property naming policy is overridden in `RelatedResourceJsonConverter::Write()`.  For media objects, it is overridden in `MediaJsonConverter::Write()`.  Both overrides are accomplished by removing the provided naming policy and letting .NET fall back to `PascalCase`.  (For whatever reason, the .NET framework does not provide an explicit native PascalCase policy.)

Example:
```json
    "relatedResources": [
        {
            "Type": "External",
            "Url": "https://www.cancer.gov/types/lung",
            "Text": "Lung Cancer"
        }
    ],
    "media": [
        {
            "Type": "Image",
            "ImageSources": [
                {
                    "Size": "original",
                    "Src": "https://nci-media.cancer.gov/pdq/media/images/466533.jpg"
                },
                {
                    "Size": "571",
                    "Src": "https://nci-media.cancer.gov/pdq/media/images/466533-571.jpg"
                },
                {
                    "Size": "750",
                    "Src": "https://nci-media.cancer.gov/pdq/media/images/466533-750.jpg"
                }
            ],
            "Ref": "CDR0000415520",
            "Alt": "Respiratory system anatomy; drawing shows the right lung with the upper, middle, and lower lobes, the left lung with the upper and lower lobes, and the trachea, bronchi, lymph nodes, and diaphragm. An inset shows the bronchioles, alveoli, artery, and vein.",
            "Caption": "Anatomy of the respiratory system showing the trachea, the right and left lungs and their lobes, and the bronchi. The lymph nodes and the diaphragm are also shown. Oxygen is inhaled into the lungs and passes through the alveoli (the tiny air sacs at the end of the bronchioles) and into the bloodstream (see inset), where it travels to the tissues throughout the body."
        }
    ]
```

### Additional Information

- [JsonSerialization overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)
- [JsonNamingPolicy](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonnamingpolicy?view=net-8.0)
