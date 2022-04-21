using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace graphweb.Pages;

[AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
public class IndexModel : PageModel
{
    public string[]? Messages { get; set; }
    public User? Me { get; set; }

    private readonly ILogger<IndexModel> _logger;

    private readonly IDownstreamWebApi _downstreamWebApi;
    private readonly GraphServiceClient _client;

    public IndexModel(ILogger<IndexModel> logger,
                        GraphServiceClient client,
                        IDownstreamWebApi downstreamWebApi)
    {
        _logger = logger;
        _downstreamWebApi = downstreamWebApi;
        _client = client;

    }

    public async Task OnGet()
    {
        Me = await _client.Me.Request().GetAsync();
        var messages = await _client.Me.Messages.Request().GetAsync();
        Messages = messages.Select(x => x.Subject).ToArray();
    }
}
