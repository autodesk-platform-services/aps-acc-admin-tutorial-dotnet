using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly APS _aps;

    public AdminController(ILogger<AdminController> logger, APS aps)
    {
        _logger = logger;
        _aps = aps;
    }

    [HttpGet("projects")]
    public async Task<ActionResult<string>> ListProjects(string hub)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null){
            return Unauthorized();
        }
        var projects = await _aps.getProjectsACC(Request.Query["accountId"], tokens);
        return JsonConvert.SerializeObject(projects);
    }

    [HttpGet("project")]
    public async Task<ActionResult<string>> ListProject( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var projects = await _aps.GetProjectACC(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(projects);
    }


    [HttpGet("project/users")]
    public async Task<ActionResult<string>> ListProjectUsers( string projectId)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }

        var projects = await _aps.GetProjectUsersACC(Request.Query["projectId"], tokens);
        return JsonConvert.SerializeObject(projects);
    }

    [HttpPost("projects")]
    public async Task<ActionResult> CreateProjects([FromBody] JObject content)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        List<string> projectsCreated = new List<string>();
        List<string> projectsFailed = new List<string>(); 
        string accountId = content["accountId"].Value<string>();
        dynamic projects = content["data"].Value<dynamic>();
        foreach (JObject project in projects)
        {
            try
            {
                JObject projectInfo = await _aps.CreateProject(accountId, project, tokens);
                projectsCreated.Add(projectInfo["name"].Value<string>());
                var profile = await _aps.GetUserProfile(tokens);
                var userInfo = await _aps.AddProjectAdminACC(projectInfo["id"].Value<string>(), profile.Email, tokens);
            }catch(Exception ex)
            {
                Console.WriteLine($"Exception when creating project: {ex.Message}");
                projectsFailed.Add(project["name"].Value<string>());
            }
        }
        return Ok(new {Succeed = projectsCreated, Failed = projectsFailed });
    }

    [HttpPost("project/users")]
    public async Task<IActionResult> CreateProjectUsers([FromBody] JObject content)
    {
        var tokens = await AuthController.PrepareTokens(Request, Response, _aps);
        if (tokens == null)
        {
            return Unauthorized();
        }
        string projectId = content["projectId"].Value<string>();
        dynamic users = content["data"].Value<dynamic>();
        dynamic body = new JObject();
        body.users = users;
        dynamic usersInfo = await _aps.ImportProjectUsersACC(projectId, body, tokens);
        return Ok(new { UserInfo= usersInfo });
    }

}
