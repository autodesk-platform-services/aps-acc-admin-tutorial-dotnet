using System.Collections.Generic;
using System.Threading.Tasks;
// using Autodesk.Forge;
// using Autodesk.Forge.Model;
using System;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class APS
{

    public async Task<IEnumerable<dynamic>> getProjectsACC(string accountId, Tokens tokens)
    {
        var client = new RestClient(Base_Domain);
        var request = new RestRequest("/construction/admin/v1/accounts/"+accountId+"/projects", Method.Get);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        RestResponse response = await client.ExecuteAsync(request);
        dynamic projectsData = JsonConvert.DeserializeObject(response.Content);
        Console.WriteLine(projectsData.results);
        return projectsData.results;
    }

    public async Task<IEnumerable<dynamic>> GetProjectACC( string projectId, Tokens tokens)
    {
        var client = new RestClient(Base_Domain);
        var request = new RestRequest("/construction/admin/v1/projects/"+projectId, Method.Get);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        RestResponse response = await client.ExecuteAsync(request);
        dynamic projectsData = JsonConvert.DeserializeObject(response.Content);
        Console.WriteLine(projectsData);
        return new List<dynamic> { projectsData };
    }


    public async Task<IEnumerable<dynamic>> GetProjectUsersACC( string projectId, Tokens tokens)
    {
        var client = new RestClient(Base_Domain);
        var request = new RestRequest("/construction/admin/v1/projects/" + projectId+"/users", Method.Get);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        RestResponse response = await client.ExecuteAsync(request);
        dynamic projectsData = JsonConvert.DeserializeObject(response.Content);
        Console.WriteLine(projectsData.results);
        return projectsData.results;
    }


    public async Task<dynamic> CreateProject(string accountId, JObject body, Tokens tokens)
    {
        var client = new RestClient(Base_Domain);
        var request = new RestRequest("/construction/admin/v1/accounts/" + accountId + "/projects", Method.Post);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody( JsonConvert.SerializeObject( body));
        RestResponse response = await client.ExecuteAsync(request);
        dynamic projectsData = JsonConvert.DeserializeObject<JObject>(response.Content);
        Console.WriteLine(response.StatusCode);
        return projectsData;
    }


    public async Task<dynamic> AddProjectAdminACC(string projectId, string email, Tokens tokens)
    {
        var client = new RestClient(Base_Domain);
        var request = new RestRequest("/construction/admin/v1/projects/" + projectId + "/users", Method.Post);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        request.AddHeader("Content-Type", "application/json");
        string body = @"{
        'email': '"+ email +@"',
        'products': [{
            'key': 'projectAdministration',
            'access': 'administrator'
        }, {
            'key': 'docs',
            'access': 'administrator'
        }]
        }";

        var userBody = JsonConvert.SerializeObject(JObject.Parse(body));
        request.AddJsonBody(userBody);
        RestResponse response = await client.ExecuteAsync(request);
        dynamic projectsData = JsonConvert.DeserializeObject(response.Content);
        Console.WriteLine(projectsData);
        return projectsData;
    }


    public async Task<dynamic> ImportProjectUsersACC(string projectId, JObject body, Tokens tokens)
    {
        var options = new RestClientOptions("https://developer.api.autodesk.com");
        var client = new RestClient(options);
        var request = new RestRequest(String.Format("/construction/admin/v2/projects/{0}/users:import", projectId), Method.Post);
        request.AddHeader("Authorization", "Bearer " + tokens.AccessToken);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(JsonConvert.SerializeObject( body));
        RestResponse response = await client.ExecuteAsync(request);
        dynamic usersInfo = JsonConvert.DeserializeObject<JObject>(response.Content);
        Console.WriteLine(usersInfo);
        return usersInfo;
    }
}
