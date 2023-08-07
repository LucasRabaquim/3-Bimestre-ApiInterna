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
        

        string bookaName = "";
        string API_KEY = "AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";
        string API_URL = "https://www.googleapis.com/books/v1/volumes"; //volumes?q=intitle:{bookName}&key=AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";//"";


        static HttpClient client = new HttpClient();

        [HttpGet("Title/{title}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>> GetByTitle(string title)
        {
            Uri url = new Uri($"{API_URL}?q={title}+intitle:{title}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return GoogleBooksService.formatResponse(jsonString);
            }
            return NoContent();
        }

        [HttpGet("Author/{name}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>> GetByAuthor(string name)
        {
            Uri url = new Uri($"{API_URL}?q={name}+inauthor:{name}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return GoogleBooksService.formatResponse(jsonString);
            }
            return NoContent();
        }
        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<IEnumerable<GoogleBooks>>> GetByIsbn(string isbn)
        {
            Uri url = new Uri($"{API_URL}?q=isbn:{isbn}&key={API_KEY}");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return GoogleBooksService.formatResponse(jsonString);
            }
            return NoContent();
        }
    }
}

