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
using LeitourApi.Services.BookApiService;
using LeitourApi.Services.MsgActionResult;

namespace LeitourApi.Controllers
{
    [Route("api/SearchBy/")]
    [ApiController]
    public class BookApiController : ControllerBase
    {

        const string API_KEY = "AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";
        const string API_URL = "https://www.googleapis.com/books/v1/volumes"; //volumes?q=intitle:{bookName}&key=AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";//"";
        static HttpClient client = new HttpClient();

        public readonly BookApiService _bookApi;
        public readonly MsgActionResultService _httpMessage;

        public BookApiController(BookApiService bookApi,MsgActionResultService httpMessage){
            _bookApi = bookApi;
            _httpMessage = httpMessage;    
        }

        [HttpGet("Title/{title}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByTitle(string title)
        {
            Uri url = new($"{API_URL}?q={title}+intitle:{title}&key={API_KEY}");
            JsonObject? reponse = await _bookApi.HttpGet(url);
            if(reponse == null)
                return _httpMessage.MsgNotReturned();
            return
            return NoContent(); 
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByIsbn(string isbn)
        {
            Uri url = new($"{API_URL}/isbn/{isbn}.json");
            JsonObject? reponse = await _bookApi.HttpGet(url);
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
                return BookApiService.FormatResponse(jsonResponse);
            }
            return NoContent();
        }

        

        /*[HttpGet("bookkey/{key}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>?> GetByIsbn(string key)
        {
            Uri url = new Uri($"{API_URL}?q=isbn:{isbn}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                JsonObject? jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();
                if(jsonResponse == null)
                    return null;
                return BookApiService.FormatResponse(jsonResponse);
            }
            return NoContent();
        }*/
    }
}

