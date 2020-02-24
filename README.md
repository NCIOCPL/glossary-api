# glossary-api
API for Dictionary of Cancer Terms, Dictionary of Genetics Terms, and other Glossary documents.

## Acceptance (integration) Tests
Acceptance tests are run using [Karate](https://intuit.github.io/karate/), which is an API testing tool that supports Gherkin tests.

### Prerequities
1. Install Java (at least 8)

### Running Tests
1. Open a terminal window
2. Change to the root of the project
3. ELASTICSEARCH HERE
3. Run the API `dotnet run --project src/NCI.OCPL.Api.Glossary`
3. Run `./integration-tests/karate ./integration-tests/features`