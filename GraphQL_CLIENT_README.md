# GraphQL Client for FwkWebAppForms1

This document describes the GraphQL client implementation for the FwkWebAppForms1 project.

## Overview

A GraphQL client has been added to the FwkWebAppForms1 ASP.NET Web Forms application, enabling you to query GraphQL APIs and display results in table format.

## Implementation Details

### New Files Created

1. **Services/GraphQLService.cs** - Core GraphQL service class
   - `GraphQLService` class: Handles HTTP communication with GraphQL APIs
   - `GraphQLResponse` class: Encapsulates query responses
   - Supports async query execution with error handling

2. **GraphQL.aspx** - User interface for GraphQL queries
   - Query editor with syntax highlighting area
   - Endpoint configuration
   - Execute and Clear buttons
   - Raw response display
   - Status indicators
   - Results displayed in a Bootstrap-styled GridView table

3. **GraphQL.aspx.cs** - Code-behind logic
   - Query execution handling
   - Response parsing and formatting
   - JSON to DataTable conversion
   - Support for both array and object responses
   - Error handling and user feedback

4. **GraphQL.aspx.designer.cs** - Auto-generated designer file

### Modified Files

- **Site.Master** - Added "GraphQL" navigation link to main menu

## How to Use

### Building and Running

1. Open `FwkWebAppForms1.sln` in Visual Studio
2. Build the solution (Build > Build Solution)
3. Run the application (F5 or Debug > Start Debugging)
4. The application will open in your default browser

### Accessing the GraphQL Client

1. Navigate to the GraphQL page using the navigation menu or directly at `https://localhost:44346/GraphQL`
2. You'll see the GraphQL client interface with:
   - **Query Configuration** section at the top
   - **Query Editor** - Enter your GraphQL query
   - **API Endpoint** - Configure the GraphQL API URL (default: `http://localhost:4000/graphql`)
   - **Execute Query** button - Runs the query
   - **Clear Results** button - Resets the interface

### Sample Query

The default query is:
```graphql
query {
  users {
    id
    name
    email
  }
}
```

Replace this with your own GraphQL queries based on your API schema.

### Results Display

The client displays results in three sections:

1. **Raw Response** - Shows the complete JSON response from the API
2. **Status** - Displays:
   - Status badge (SUCCESS or ERROR)
   - Result message
   - Execution timestamp and duration
3. **Results Table** - Automatically converts query results to a Bootstrap-styled table

## Features

- **Async Query Execution** - Non-blocking API calls
- **Error Handling** - Comprehensive error messages for various failure scenarios
- **Response Formatting** - Pretty-printed JSON display
- **Table Conversion** - Automatic conversion of nested JSON to tables:
  - Handles arrays of objects
  - Supports single object responses
  - Flattens nested structures for display
- **Bootstrap Styling** - Responsive design with Bootstrap 5.2.3
- **Performance Metrics** - Shows query execution time

## Architecture

### GraphQLService Class

```csharp
public class GraphQLService
{
    public GraphQLService(string graphqlEndpoint)
    public async Task<GraphQLResponse> ExecuteQueryAsync(string query, object variables = null)
}

public class GraphQLResponse
{
    public bool Success { get; set; }
    public JToken Data { get; set; }
    public string Error { get; set; }
}
```

### Key Methods in GraphQL.aspx.cs

- `ExecuteButton_Click()` - Handles query execution
- `ClearButton_Click()` - Resets the interface
- `DisplayResults()` - Processes and displays query results
- `ConvertJTokenToDataTable()` - Converts JSON responses to DataTable format
- `ConvertJArrayToDataTable()` - Handles array responses
- `ConvertJObjectToDataTable()` - Handles single object responses

## Dependencies

The implementation uses:
- `Newtonsoft.Json` (Json.NET) v13.0.3 - For JSON parsing and manipulation
- `System.Net.Http` - For HTTP communication
- ASP.NET Web Forms controls (TextBox, Button, Label, GridView)
- Bootstrap 5.2.3 - For styling

## Troubleshooting

### "Connection refused" error
- Ensure the GraphQL API is running at the configured endpoint
- Default endpoint is `http://localhost:4000/graphql`
- Check the firewall settings if running on different machines

### "No data to display"
- Verify the GraphQL query is valid
- Check the Raw Response section for error messages from the API
- Ensure the API returns the expected data structure

### Table not displaying results
- The client expects arrays or objects in the response
- For complex nested structures, you may see the data serialized as text in the table
- Adjust your query to return flatter data structures when possible

## Future Enhancements

Possible improvements:
- Query history/favorites
- Variable editor for parameterized queries
- Query syntax highlighting
- Schema introspection and explorer
- Export results to CSV/Excel
- Subscription support for real-time updates

## Notes

- The GraphQL endpoint can be changed without recompiling the application
- Query execution is async but runs within the ASP.NET postback model
- Large result sets may require pagination (implement in custom queries)
- The implementation uses HttpClient for thread-safe HTTP operations
