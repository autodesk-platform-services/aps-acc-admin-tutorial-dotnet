using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.Construction.AccountAdmin;
using Autodesk.Construction.AccountAdmin.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class APS
{

    public async Task<IEnumerable<dynamic>> getProjectsACC(string accountId, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var allProjects = new List<Project>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var projects = await adminClient.GetProjectsAsync(tokens.AccessToken, accountId);
            allProjects.AddRange(projects.Results);
            offset +=  (int)projects.Pagination.Limit;
            totalResult = (int)projects.Pagination.TotalResults;
        }while (offset < totalResult);
        return allProjects;
    }

    public async Task<IEnumerable<dynamic>> GetProjectACC( string projectId, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var project = await adminClient.GetProjectAsync(tokens.AccessToken, projectId );
        var projects = new List<Project>();
        projects.Add(project);
        return projects;
    }


    public async Task<IEnumerable<dynamic>> GetProjectUsersACC( string projectId, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var allUsers = new List<ProjectUser>();
        var offset = 0;
        var totalResult = 0;
        do
        {
            var users = await adminClient.GetProjectUsersAsync(tokens.AccessToken, projectId);
            allUsers.AddRange(users.Results);
            offset += (int)users.Pagination.Limit;
            totalResult = (int)users.Pagination.TotalResults;
        } while (offset < totalResult);
        return allUsers;
    }


    public async Task<dynamic> CreateProject(string accountId, JObject body, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var projectPayload = body.ToObject<ProjectPayload>();
        var newProject = await adminClient.CreateProjectAsync(tokens.AccessToken, accountId, projectPayload);
        return newProject;
    }


    public async Task<dynamic> AddProjectAdminACC(string projectId, string email, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        ProjectUserPayload adminUser = new ProjectUserPayload()
        {
            Email = email,
            Products = new List<ProjectUserPayloadProducts>()
            {
                new ProjectUserPayloadProducts()
                {
                    Key = ProductKeys.ProjectAdministration,
                    Access = ProductAccess.Administrator
                },
                new ProjectUserPayloadProducts()
                {
                    Key = ProductKeys.Docs,
                    Access = ProductAccess.Administrator

                }
            }
        };
        var projectUser = await adminClient.AssignProjectUserAsync(tokens.AccessToken, projectId, adminUser);
        return projectUser;
    }


    public async Task<dynamic> ImportProjectUsersACC(string projectId, JObject body, Tokens tokens)
    {
        AdminClient adminClient = new AdminClient(_SDKManager);
        var projectUsersPayload = body.ToObject<ProjectUsersImportPayload>();
        var usersRes = await adminClient.ImportProjectUsersAsync(tokens.AccessToken, projectId, projectUsersPayload);
        return usersRes;
    }
}
