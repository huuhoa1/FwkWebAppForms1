using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using FwkWebAppForms1.Services;
using Newtonsoft.Json.Linq;

namespace FwkWebAppForms1
{
    public partial class GraphQL : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NoDataLabel.Visible = true;
                ResultsGridView.Visible = false;
            }
        }

        protected void ExecuteButton_Click(object sender, EventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                string endpoint = EndpointTextBox.Text.Trim();
                string query = QueryTextBox.Text.Trim();

                if (string.IsNullOrEmpty(endpoint))
                {
                    ShowError("Endpoint URL is required");
                    return;
                }

                if (string.IsNullOrEmpty(query))
                {
                    ShowError("GraphQL query is required");
                    return;
                }

                ShowStatus("Executing query...", "Executing query");
                ResponseTextBox.Text = $"Connecting to {endpoint}...\r\nSending query...\r\n\r\nPlease wait...";

                var service = new GraphQLService(endpoint);
                // Call the synchronous method directly
                var response = service.ExecuteQuery(query);

                stopwatch.Stop();

                if (response.Success)
                {
                    ResponseTextBox.Text = response.Data.ToString(Newtonsoft.Json.Formatting.Indented);
                    DisplayResults(response.Data);
                    ShowSuccess($"Query executed successfully in {stopwatch.ElapsedMilliseconds}ms");
                }
                else
                {
                    ResponseTextBox.Text = response.Error;
                    ShowError($"Query failed: {response.Error}");
                    NoDataLabel.Visible = true;
                    ResultsGridView.Visible = false;
                }
            }
            catch (AggregateException aex)
            {
                stopwatch.Stop();
                string errorMsg = string.Join("; ", aex.InnerExceptions.Select(ex => ex.Message));
                ResponseTextBox.Text = errorMsg;
                ShowError($"Error: {errorMsg}");
                NoDataLabel.Visible = true;
                ResultsGridView.Visible = false;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                ResponseTextBox.Text = ex.Message + "\r\n\r\nInner Exception: " + (ex.InnerException?.Message ?? "None");
                ShowError($"Error: {ex.Message}");
                NoDataLabel.Visible = true;
                ResultsGridView.Visible = false;
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            ResponseTextBox.Text = string.Empty;
            QueryTextBox.Text = @"query {
  users {
    id
    name
    email
  }
}";
            StatusLabel.Text = string.Empty;
            MessageLabel.Text = string.Empty;
            TimeLabel.Text = string.Empty;
            NoDataLabel.Visible = true;
            ResultsGridView.Visible = false;
            ResultsGridView.DataSource = null;
            ResultsGridView.DataBind();
        }

        private void DisplayResults(JToken data)
        {
            try
            {
                if (data == null)
                {
                    NoDataLabel.Visible = true;
                    ResultsGridView.Visible = false;
                    return;
                }

                DataTable dt = ConvertJTokenToDataTable(data);

                if (dt != null && dt.Rows.Count > 0)
                {
                    ResultsGridView.DataSource = dt;
                    ResultsGridView.DataBind();
                    ResultsGridView.Visible = true;
                    NoDataLabel.Visible = false;
                }
                else
                {
                    NoDataLabel.Text = "Query returned no data.";
                    NoDataLabel.Visible = true;
                    ResultsGridView.Visible = false;
                }
            }
            catch (Exception ex)
            {
                NoDataLabel.Text = $"Error displaying results: {ex.Message}";
                NoDataLabel.Visible = true;
                ResultsGridView.Visible = false;
            }
        }

        private DataTable ConvertJTokenToDataTable(JToken data)
        {
            DataTable dt = new DataTable();

            // Handle case where data is an object with arrays
            if (data is JObject jObject)
            {
                // Try to find the first array property
                var arrayProperty = jObject.Properties()
                    .FirstOrDefault(p => p.Value is JArray);

                if (arrayProperty != null && arrayProperty.Value is JArray jArray)
                {
                    return ConvertJArrayToDataTable(jArray);
                }
                else
                {
                    // Single object - convert to single row table
                    return ConvertJObjectToDataTable(jObject);
                }
            }
            // Handle direct array
            else if (data is JArray jArray)
            {
                return ConvertJArrayToDataTable(jArray);
            }

            return dt;
        }

        private DataTable ConvertJArrayToDataTable(JArray jArray)
        {
            DataTable dt = new DataTable();

            if (jArray.Count == 0)
                return dt;

            // Create columns based on first item
            var firstItem = jArray[0];
            if (firstItem is JObject jObj)
            {
                foreach (var prop in jObj.Properties())
                {
                    dt.Columns.Add(prop.Name);
                }

                // Add rows
                foreach (var item in jArray)
                {
                    if (item is JObject rowObj)
                    {
                        var row = dt.NewRow();
                        foreach (var prop in rowObj.Properties())
                        {
                            row[prop.Name] = FormatValue(prop.Value);
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return dt;
        }

        private DataTable ConvertJObjectToDataTable(JObject jObject)
        {
            DataTable dt = new DataTable();

            foreach (var prop in jObject.Properties())
            {
                dt.Columns.Add(prop.Name);
            }

            var row = dt.NewRow();
            foreach (var prop in jObject.Properties())
            {
                row[prop.Name] = FormatValue(prop.Value);
            }
            dt.Rows.Add(row);

            return dt;
        }

        private object FormatValue(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return DBNull.Value;

            switch (token.Type)
            {
                case JTokenType.String:
                    return token.Value<string>();
                case JTokenType.Integer:
                    return token.Value<int>();
                case JTokenType.Float:
                    return token.Value<double>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.Array:
                case JTokenType.Object:
                    return token.ToString();
                default:
                    return token.ToString();
            }
        }

        private void ShowStatus(string status, string message)
        {
            StatusLabel.Text = status;
            StatusLabel.CssClass = "badge bg-info";
            MessageLabel.Text = message;
            TimeLabel.Text = $"Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        private void ShowSuccess(string message)
        {
            StatusLabel.Text = "SUCCESS";
            StatusLabel.CssClass = "badge bg-success";
            MessageLabel.Text = message;
            TimeLabel.Text = $"Executed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        private void ShowError(string message)
        {
            StatusLabel.Text = "ERROR";
            StatusLabel.CssClass = "badge bg-danger";
            MessageLabel.Text = message;
            TimeLabel.Text = $"Executed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
