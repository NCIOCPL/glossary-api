using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Elasticsearch implementation of the service for retrieving multiple
    /// GlossaryTerm objects.
    /// </summary>
    public class ESTermsQueryService : ITermsQueryService
    {
        /// <summary>
        /// A list of all of public, instance properties in the GlossaryTerm type except for
        /// the ones which Search, Expand and GetAll don't return by default.
        /// </summary>
        static readonly IEnumerable<PropertyInfo> DEFAULT_FIELDS =
            typeof(GlossaryTerm).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                    pi.Name.CompareTo(nameof(GlossaryTerm.RelatedResources)) != 0
                    && pi.Name.CompareTo(nameof(GlossaryTerm.Media)) != 0
                )
            .Select(pi => pi);

        /// <summary>
        /// A list of all of public, instance properties in the GlossaryTerm type.
        /// </summary>
        static readonly IEnumerable<PropertyInfo> ALL_FIELDS =
            typeof(GlossaryTerm).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(pi => pi);

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
        private readonly ILogger<ESTermsQueryService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ESTermsQueryService(ElasticsearchClient client, IOptions<GlossaryAPIOptions> apiOptionsAccessor,
            ILogger<ESTermsQueryService> logger)
        {
            _elasticClient = client;
            _apiOptions = apiOptionsAccessor.Value;
            _logger = logger;
        }

        /// <summary>
        /// Get Term details based on the input values
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="id">The Id for the term</param>
        /// <returns>The GlossaryTerm or null if not found.</returns>
        /// </summary>
        public async Task<GlossaryTerm> GetById(string dictionary, AudienceType audience, string language, long id)
        {
            GetResponse<GlossaryTerm> response = null;

            try
            {
                string idValue = $"{id}_{dictionary?.ToLowerInvariant()}_{language?.ToLowerInvariant()}_{audience.ToString().ToLowerInvariant()}";
                response = await _elasticClient.GetAsync<GlossaryTerm>(idValue, g => g.Index(this._apiOptions.AliasName));
            }
            catch (Exception ex)
            {
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIInternalException(msg);
            }

            // This is a little weird because "Not Found" is considered a "successful" response
            // as in "succeeded in looking for the term, and it just doesn't exist".
            // So first we check whether the response was successful (versus something like a 500 error),
            // and then we check whether the term was found.
            if (!response.ApiCallDetails.HasSuccessfulStatusCode)
            {
                String msg = $"Invalid Elasticsearch response for dictionary '{dictionary}', audience '{audience}', language '{language}' and id '{id}'.\n\n{response.DebugInformation}"
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIInternalException(msg);
            }
            else if (!response.Found)
            {
                return null;
            }

            return response.Source;
        }

        /// <summary>
        /// Search for Term based on the pretty URL name passed.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="prettyUrlName">The pretty url name to search for</param>
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        public async Task<GlossaryTerm> GetByName(string dictionary, AudienceType audience, string language, string prettyUrlName)
        {
            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName);

            var query = new BoolQuery
            {
                Must = new Query[]
                {
                    new TermQuery { Field = "language", Value = language.ToString() },
                    new TermQuery { Field = "audience", Value = audience.ToString() },
                    new TermQuery { Field = "dictionary", Value = dictionary.ToString() },
                    new TermQuery { Field = "pretty_url_name", Value = prettyUrlName.ToString() }
                }
            };

            SearchRequestDescriptor<GlossaryTerm> request = new SearchRequestDescriptor<GlossaryTerm>(index)
                .Query(query)
                .Sort(new FieldSort(new Field("term_name")));

            SearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = ($"Unexpected error while searching dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'."
                  .Replace(Environment.NewLine, String.Empty));
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIInternalException(msg);
            }

            if (!response.IsValidResponse)
            {
                string msg = $"Invalid Elasticsearch response for dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'."
                  .Replace(Environment.NewLine, String.Empty)
                  + $"\n\n{response.DebugInformation}";
                _logger.LogError(msg);
                throw new APIInternalException("errors occurred");
            }

            GlossaryTerm glossaryTerm = new GlossaryTerm();

            // If there is only one term in the response, then the search by pretty URL name was successful.
            if (response.Total == 1)
            {
                glossaryTerm = response.Documents.First();
            }
            else if (response.Total == 0)
            {
                glossaryTerm = null;
            }
            else
            {
                string msg = $"Multiple results for dictionary '{dictionary}', audience '{audience}', language '{language}', pretty URL name '{prettyUrlName}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIInternalException("errors occurred");
            }

            return glossaryTerm;
        }

        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        public async Task<GlossaryTermResults> GetAll(string dictionary, AudienceType audience, string language, int size, int from, bool includeAdditionalInfo)
        {
            // Elasticsearch knows how to figure out what the ElasticSearch name is for
            // a given field when given a PropertyInfo.
            PropertyInfo[] requestedESFields = (includeAdditionalInfo ? ALL_FIELDS : DEFAULT_FIELDS).ToArray();

            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName);

            var query = new BoolQuery
            {
                Must = new Query[]
                {
                    new TermQuery { Field = "language", Value = language.ToString() },
                    new TermQuery { Field = "audience", Value = audience.ToString() },
                    new TermQuery { Field = "dictionary", Value = dictionary.ToString() }
                }
            };

            SearchRequestDescriptor<GlossaryTerm> request = new SearchRequestDescriptor<GlossaryTerm>(index)
                .Query(query)
                .Sort(new FieldSort(new Field("term_name")))
                .Size(size)
                .From(from)
                .Source(new SourceFilter{ Includes = requestedESFields });

            SearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not get dictionary '{dictionary}', audience '{audience}', language '{language}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error Fetching All from index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValidResponse)
            {
                String msg = $"Invalid response when getting dictionary '{dictionary}', audience '{audience}', language '{language}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occurred");
            }

            GlossaryTermResults glossaryTermResults = new GlossaryTermResults();

            if (response.Total > 0)
            {
                // Build the array of glossary terms for the returned results.
                List<GlossaryTerm> termResults = new List<GlossaryTerm>();
                foreach (GlossaryTerm res in response.Documents)
                {
                    termResults.Add(res);
                }

                glossaryTermResults.Results = termResults.ToArray();

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }
            else if (response.Total == 0) {
                // Add the default value of empty GlossaryTerm list.
                glossaryTermResults.Results = new GlossaryTerm[] {};

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }

            return glossaryTermResults;
        }

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <param name="matchType">Defines if the search should begin with or contain the keyword</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A list of GlossaryTerm</returns>
        /// </summary>
        public async Task<GlossaryTermResults> Search(string dictionary, AudienceType audience, string language, string query, MatchType matchType, int size, int from, bool includeAdditionalInfo)
        {
            // Elasticsearch knows how to figure out what the ElasticSearch name is for
            // a given field when given a PropertyInfo.
            PropertyInfo[] requestedESFields = (includeAdditionalInfo ? ALL_FIELDS : DEFAULT_FIELDS).ToArray();

            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName);

            // Build the Must queries based on the match type
            List<Query> mustQueries = new List<Query>
            {
                new TermQuery { Field = "language", Value = language.ToString() },
                new TermQuery { Field = "audience", Value = audience.ToString() },
                new TermQuery { Field = "dictionary", Value = dictionary.ToString() }
            };

            // Add the match type specific query
            switch (matchType)
            {
                case MatchType.Begins:
                    mustQueries.Add(new PrefixQuery { Field = "term_name", Value = query });
                    break;
                case MatchType.Contains:
                    mustQueries.Add(new MatchQuery { Field = "term_name._contain", Query = query });
                    break;
                case MatchType.Exact:
                    mustQueries.Add(new TermQuery { Field = "term_name", Value = query });
                    break;
                default:
                    throw new ArgumentException($"Unknown matchType value '{matchType}'.");
            }

            var boolQuery = new BoolQuery
            {
                Must = mustQueries.ToArray()
            };

            SearchRequestDescriptor<GlossaryTerm> request = new SearchRequestDescriptor<GlossaryTerm>(index)
                .Query(boolQuery)
                .Sort(new FieldSort(new Field("term_name")))
                .Size(size)
                .From(from)
                .Source(new SourceFilter{ Includes = requestedESFields });

            SearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}', query '{query}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValidResponse)
            {
                String msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}', query '{query}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occurred");
            }

            GlossaryTermResults glossaryTermResults = new GlossaryTermResults();

            if (response.Total > 0)
            {
                // Build the array of glossary terms for the returned results.
                List<GlossaryTerm> termResults = new List<GlossaryTerm>();
                foreach (GlossaryTerm res in response.Documents)
                {
                    termResults.Add(res);
                }

                glossaryTermResults.Results = termResults.ToArray();

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }
            else if (response.Total == 0) {
                // Add the default value of empty GlossaryTerm list.
                glossaryTermResults.Results = new GlossaryTerm[] {};

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }

            return glossaryTermResults;

        }


        /// <summary>
        /// Get all Terms starting with the character passed.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="expandCharacter">The character to search the query</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="includeAdditionalInfo">If true, the RelatedResources and Media fields will be populated. Else, they will be empty.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        /// </summary>
        public async Task<GlossaryTermResults> Expand(string dictionary, AudienceType audience, string language, string expandCharacter, int size, int from, bool includeAdditionalInfo)
        {
            // Elasticsearch knows how to figure out what the ElasticSearch name is for
            // a given field when given a PropertyInfo.
            PropertyInfo[] requestedESFields = (includeAdditionalInfo ? ALL_FIELDS : DEFAULT_FIELDS).ToArray();

            // Set up the SearchRequest to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName);

            var query = new BoolQuery
            {
                Must = new Query[]
                {
                    new TermQuery { Field = "language", Value = language.ToString() },
                    new TermQuery { Field = "audience", Value = audience.ToString() },
                    new TermQuery { Field = "dictionary", Value = dictionary.ToString() },
                    new TermQuery { Field = "first_letter", Value = expandCharacter.ToString() }
                }
            };

            SearchRequestDescriptor<GlossaryTerm> request = new SearchRequestDescriptor<GlossaryTerm>(index)
                .Query(query)
                .Sort(new FieldSort(new Field("term_name")))
                .Size(size)
                .From(from)
                .Source(new SourceFilter { Includes = requestedESFields });

            SearchResponse<GlossaryTerm> response = null;
            try
            {
                response = await _elasticClient.SearchAsync<GlossaryTerm>(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not search dictionary '{dictionary}', audience '{audience}', language '{language}', character '{expandCharacter}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error searching index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if (!response.IsValidResponse)
            {
                String msg = $"Invalid response when searching for '{dictionary}', audience '{audience}', language '{language}', character '{expandCharacter}', size '{size}', from '{from}'."
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occurred");
            }

            GlossaryTermResults glossaryTermResults = new GlossaryTermResults();

            if (response.Total > 0)
            {
                // Build the array of glossary terms for the returned results.
                List<GlossaryTerm> termResults = new List<GlossaryTerm>();
                foreach (GlossaryTerm res in response.Documents)
                {
                    termResults.Add(res);
                }

                glossaryTermResults.Results = termResults.ToArray();

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }
            else if (response.Total == 0) {
                // Add the default value of empty GlossaryTerm list.
                glossaryTermResults.Results = new GlossaryTerm[] {};

                // Add the metadata for the returned results
                glossaryTermResults.Meta = new ResultsMetadata() {
                    TotalResults = (int)response.Total,
                    From = from
                };
            }

            return glossaryTermResults;
        }

        /// <summary>
        /// Get the total number of terms available in the version of a dictionary matching a specific audience and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <returns>The number of terms available.</returns>
        public async Task<long> GetCount(string dictionary, AudienceType audience, string language)
        {
            // Set up the count request to send to elasticsearch.
            Indices index = Indices.Index(this._apiOptions.AliasName);
            CountResponse response = null;
            try
            {
                var query = new BoolQuery
                {
                    Must = new Query[]
                    {
                        new TermQuery { Field = "language", Value = language },
                        new TermQuery { Field = "audience", Value = audience.ToString() },
                        new TermQuery { Field = "dictionary", Value = dictionary }
                    }
                };

                CountRequestDescriptor<GlossaryTerm> request = new CountRequestDescriptor<GlossaryTerm>(index)
                    .Query(query);
                response = await _elasticClient.CountAsync(request);
            }
            catch (Exception ex)
            {
                String msg = $"Could not get a count for dictionary '{dictionary}', audience '{audience}', language '{language}'"
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError($"Error getting count on index: '{this._apiOptions.AliasName}'.");
                _logger.LogError(ex, msg);
                throw new APIErrorException(500, msg);
            }

            if(!response.IsValidResponse)
            {
                String msg = $"Invalid response when searching for dictionary '{dictionary}', audience '{audience}', language '{language}'"
                  .Replace(Environment.NewLine, String.Empty);
                _logger.LogError(msg);
                throw new APIErrorException(500, "errors occurred");
            }

            return response.Count;
        }

    }
}
