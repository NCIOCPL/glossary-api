using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Models;

namespace NCI.OCPL.Api.Glossary.Services
{
    /// <summary>
    /// Elasticsearch implementation of the service for retrieving suggestions for
    /// GlossaryTerm objects.
    /// </summary>
    public class ESAutosuggestQueryService : IAutosuggestQueryService
    {

        /// <summary>
        /// The elasticsearch client
        /// </summary>
        private ElasticsearchClient _elasticClient;

        /// <summary>
        /// The API options.
        /// </summary>
        protected readonly GlossaryAPIOptions _apiOptions;

        /// <summary>
        /// A logger to use for logging
        /// </summary>
        private readonly ILogger<ESAutosuggestQueryService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESAutosuggestQueryService(ElasticsearchClient client,
            IOptions<GlossaryAPIOptions> apiOptionsAccessor,
            ILogger<ESAutosuggestQueryService> logger)
        {
            _elasticClient = client;
            _apiOptions = apiOptionsAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// </summary>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="searchText">The text to search for.</param>
        /// <param name="matchType">Set to true to allow search to find terms which contain the query string instead of explicitly starting with it.</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <returns>An array of Suggestion objects</returns>
        public async Task<Suggestion[]> GetSuggestions(string dictionary, AudienceType audience, string language, string searchText, MatchType matchType, int size)
        {
            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName );

            SearchResponse<Suggestion> response = null;

            try
            {
                SearchRequestDescriptor<Suggestion> request;
                switch (matchType)
                {
                    default:
                    case MatchType.Begins:
                    case MatchType.Exact:
                        request = BuildNonContainsRequest(index, dictionary, language, audience, searchText, matchType, size);
                        break;
                    case MatchType.Contains:
                        request = BuildContainsRequest(index, dictionary, language, audience, searchText, size);
                        break;
                }

                response = await _elasticClient.SearchAsync<Suggestion>(request);
            }
            catch (Exception ex)
            {
                string msg = $"Could not search dictionary '{dictionary}', audience '{audience}', and language '{language}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValidResponse)
            {
                string msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', query '{searchText}', contains '{matchType}', size '{size}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occurred");
            }

            List<Suggestion> retVal = new List<Suggestion>(response.Documents);

            return retVal.ToArray();
        }

        /// <summary>
        /// Builds the SearchRequest for terms beginning with the search text.
        /// </summary>
        /// <param name="index">The index which will be searched against.</param>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="query">The text to search for.</param>
        /// <param name="matchType">Do a Begins or Exact match search? (Use BuildContainsRequest for Contains searches).</param>
        /// <param name="size">The number of records to retrieve.</param>
        private SearchRequestDescriptor<Suggestion> BuildNonContainsRequest(Indices index, string dictionary, string language, AudienceType audience, string query, MatchType matchType, int size)
        {
            /*
             * Create a query similar to the following.
             *
             * curl -XPOST http://SERVER_NAME/glossaryv1/terms/_search -H 'Content-Type: application/x-ndjson'   -d '{
             *   "query": {
             *     "bool" : {
             *       "must" :
             *         [{"term" : { "language" : "es" }},
             *         {"term": { "audience": "Patient"}},
             *         {"term": { "dictionary": "Cancer.gov"}},
             *         {"prefix" : {"term_name" : "cutáneo"}}
             *       ]
             *     }
             *   }
             * ,"sort": ["term_name"]
             * , "_source": ["term_id", "term_name"]
             * , "size": 10
             * }'
             */
            var boolQuery = new BoolQuery
            {
                Must = new Query[]
                {
                    new TermQuery { Field = "language", Value = language },
                    new TermQuery { Field = "audience", Value = audience.ToString() },
                    new TermQuery { Field = "dictionary", Value = dictionary },
                    matchType == MatchType.Begins
                        ? new PrefixQuery { Field = "term_name", Value = query }
                        : new TermQuery { Field = "term_name", Value = query }
                }
            };

            SearchRequestDescriptor<Suggestion> request = new SearchRequestDescriptor<Suggestion>(index)
                .Query(boolQuery)
                .Sort(new FieldSort(new Field("term_name")))
                .Size(size)
                .Source( new SourceFilter{ Includes = new string[] { "term_id", "term_name" } });

            return request;
        }

        /// <summary>
        /// Builds the SearchRequest for terms containing with the search text.
        /// </summary>
        /// <param name="index">The index which will be searched against.</param>
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="query">The text to search for.</param>
        /// <param name="size">The number of records to retrieve.</param>
        private SearchRequestDescriptor<Suggestion> BuildContainsRequest(Indices index, string dictionary, string language, AudienceType audience, string query, int size)
        {
            /*
             * Create a query similar to the following.
             *
             * curl -XPOST http://SERVER_NAME/glossaryv1/terms/_search -H 'Content-Type: application/x-ndjson'   -d '{
             *   "query": {
             *     "bool" : {
             *       "must" :
             *         [{"term" : { "language" : "es" }},
             *         {"term" : { "dictionary" : "Cancer.gov" }},
             *         {"term": { "audience": "Patient"}},
             *         {"match_phrase":{"term_name._autocomplete":"cutáneo"}}
             *         ],
             *          "must_not" :  {"prefix" : {"term_name" : "cutáneo"}}
             *       }
             *     }
             * ,"sort": ["term_name"]
             * , "_source": ["term_id", "term_name"]
             * , "from": 0
             * , "size": 10
             * }'
             */
            var boolQuery = new BoolQuery
            {
                Must = new Query[]
                {
                    new TermQuery { Field = "language", Value = language },
                    new TermQuery { Field = "audience", Value = audience.ToString() },
                    new TermQuery { Field = "dictionary", Value = dictionary },
                    new MatchPhraseQuery { Field = "term_name._autocomplete", Query = query }
                },
                MustNot = new Query[]
                {
                    new PrefixQuery { Field = "term_name", Value = query }
                }
            };

            SearchRequestDescriptor<Suggestion> request = new SearchRequestDescriptor<Suggestion>(index)
                .Query(boolQuery)
                .Sort(new FieldSort(new Field("term_name")))
                .Source( new SourceFilter{ Includes = new string[] { "term_id", "term_name" } })
                .Size(size);

            return request;
        }

    }
}