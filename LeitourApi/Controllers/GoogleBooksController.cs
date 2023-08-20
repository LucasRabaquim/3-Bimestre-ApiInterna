using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using System.IO;
using System.Text.Json.Nodes;
using System.Reflection;

namespace LeitourApi.Controllers
{
    [Route("api/SearchBy/")]
    [ApiController]
    public class GoogleBooksController : ControllerBase
    {

        const string API_KEY = "AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";
        const string API_URL = "https://www.googleapis.com/books/v1/volumes"; //volumes?q=intitle:{bookName}&key=AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";//"";

        static HttpClient client = new HttpClient();

        [HttpGet("Title/{title}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByTitle(string title)
        {
            Uri url = new Uri($"{API_URL}?q={title}+intitle:{title}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                JsonObject? jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();
                if(jsonResponse == null)
                    return null;
                return GoogleBooksService.FormatResponse(jsonResponse);
            }
            return NoContent(); 
        }
 
        [HttpGet("Author/{name}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByAuthor(string name)
        {
            Uri url = new Uri($"{API_URL}?q={name}+inauthor:{name}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                JsonObject? jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();
                if(jsonResponse == null)
                    return null;
                return GoogleBooksService.FormatResponse(jsonResponse);
            }
            return NoContent();
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByIsbn(string isbn)
        {
            Uri url = new Uri($"{API_URL}?q=isbn:{isbn}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                JsonObject? jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();
                if(jsonResponse == null)
                    return null;
                return GoogleBooksService.FormatResponse(jsonResponse);
            }
            return NoContent();
        }

        /*
        private static string QUERY_PARAM = "q";
    private static string MODE = "mode";
    private static string LIMIT = "limit";
    private static string ORDER_BY = "orderBy";
    private static string MAX_RESULTS = "maxResults";
    private static string OFFSET = "offset";
    private static string INTITLE = "intitle";
    private static string INAUTHOR = "inauthor";
    private static string INPUBLISHER = "inpublisher";
    private static string SUBJECT = "subject";
    private static string ISBN = "isbn";
    private static string ISBN10 = "ISBN_10";
    private static string ISBN13 = "ISBN_13";
    private static string KEY = "key";

    private static string API_KEY = "AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";

    private static string API_URL = "https://www.googleapis.com/books/v1/volumes?q=intitle:drag%C3%B5es%20de%20eter&key=AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";//"https://www.googleapis.com/books/v1/volumes";


        */
    }
}

