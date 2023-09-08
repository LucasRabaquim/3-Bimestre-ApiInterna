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
using Newtonsoft.Json.Linq;

namespace LeitourApi.Controllers
{
    [Route("api/SearchBy/")]
    [ApiController]
    public class BookApiController : ControllerBase
    {

        private static string API_URL = "https://openlibrary.org/";
        //volumes?q=intitle:{bookName}&key=AIzaSyAz_H70Ju10k16gGDt-V-wQnYll-q7q7LY";//"";
        static readonly HttpClient client = new();

        public readonly BookApiService _bookApi;
        public readonly MsgActionResultService _httpMessage;

        public BookApiController(BookApiService bookApi,MsgActionResultService httpMessage){
            _bookApi = bookApi;
            _httpMessage = httpMessage;    
        }

        [HttpGet("Title/{title}")]
        public async Task<ActionResult<IEnumerable<BookApi>>?> GetByTitle(string title)
        {
            Uri url = new($"{API_URL}search.json?q={title}&limit=10");
            JObject response = await _bookApi.HttpGet(url);
            if((int) response["Code"] == StatusCodes.Status500InternalServerError)
                return _httpMessage.MsgBookNotReturned();
            if((int) response["Code"]  == StatusCodes.Status404NotFound)
                return _httpMessage.MsgBookNotFound();
            //return _httpMessage.MsgDebugValue(response.ToString());
            List<BookApi> books = await _bookApi.FormatResponse(response);
            return books;
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<IEnumerable<BookApi>>?> GetByIsbn(string isbn)
        {
            Uri url = new($"{API_URL}search.json?isbn={isbn}&limit=10");
            JObject response = await _bookApi.HttpGet(url);
            if((int) response["Code"] == StatusCodes.Status500InternalServerError)
                return _httpMessage.MsgBookNotReturned();
            if((int) response["Code"]  == StatusCodes.Status404NotFound)
                return _httpMessage.MsgBookNotFound();
            //return _httpMessage.MsgDebugValue(response.ToString());
            List<BookApi> books = await _bookApi.FormatResponse(response);
            return books;
        }
    }
}