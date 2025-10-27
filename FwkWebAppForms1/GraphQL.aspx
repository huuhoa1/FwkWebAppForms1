<%@ Page Title="GraphQL Client" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GraphQL.aspx.cs" Inherits="FwkWebAppForms1.GraphQL" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5">
        <h1>GraphQL Client</h1>
        <p class="text-muted">Query data from http://localhost:4000</p>

        <div class="row mb-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h5>Query Configuration</h5>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <label for="QueryTextBox">GraphQL Query:</label>
                            <asp:TextBox ID="QueryTextBox" runat="server" TextMode="MultiLine" Rows="6"
                                CssClass="form-control" Placeholder="Enter your GraphQL query...">
query {
  users {
    id
    name
    email
  }
}
                            </asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label for="EndpointTextBox">API Endpoint:</label>
                            <asp:TextBox ID="EndpointTextBox" runat="server" CssClass="form-control"
                                Text="http://localhost:4000/graphql" />
                        </div>

                        <asp:Button ID="ExecuteButton" runat="server" Text="Execute Query"
                            CssClass="btn btn-primary" OnClick="ExecuteButton_Click" />
                        <asp:Button ID="ClearButton" runat="server" Text="Clear Results"
                            CssClass="btn btn-secondary ms-2" OnClick="ClearButton_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h5>Raw Response</h5>
                    </div>
                    <div class="card-body">
                        <asp:TextBox ID="ResponseTextBox" runat="server" TextMode="MultiLine" Rows="8"
                            CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card">
                    <div class="card-header">
                        <h5>Status</h5>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <label>Status:</label>
                            <asp:Label ID="StatusLabel" runat="server" CssClass="badge bg-secondary"></asp:Label>
                        </div>
                        <div class="mb-3">
                            <label>Message:</label>
                            <asp:Label ID="MessageLabel" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                        <div>
                            <label>Execution Time:</label>
                            <asp:Label ID="TimeLabel" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header">
                        <h5>Results Table</h5>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="ResultsGridView" runat="server" CssClass="table table-striped table-hover"
                            AutoGenerateColumns="true" />
                        <asp:Label ID="NoDataLabel" runat="server" Text="No data to display. Execute a query first."
                            CssClass="text-muted" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
