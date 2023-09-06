

using LeitourApi.Models;
using System.Text.Json;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using static System.Collections.IEnumerable;
using LeitourApi.Services.BookApiService;

namespace LeitourApi.Services.BookApiService;


public class BookApiService
{   
    private static string API_URL = "https://openlibrary.org/";
    private static string QUERY_PARAM = "q";
    private static string MODE = "mode";
    private static string LIMIT = "limit";
    private static string OFFSET = "offset";

    static HttpClient client = new HttpClient();
    const int limit = 10;

    public async Task<JsonObject?> HttpGet(Uri url){
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            JsonObject? jsonResponse = await response.Content.ReadFromJsonAsync<JsonObject>();
            return jsonResponse;
        }
        return null;
    }

    public static List<GoogleBooks> FormatResponse(JsonObject response)
    {
        JsonArray jsonBookArray = response["items"].AsArray();
        List<GoogleBooks> Books = new();
        foreach (JsonObject jsonItems in jsonBookArray)
        {
            GoogleBooks book = new();
            book.Key = jsonItems["id"].AsValue().ToString();
            JsonObject jsonVolumeInfo = jsonItems["volumeInfo"].AsObject();
            book.Title = (string) jsonVolumeInfo["title"];
            book.Subtitle = (string) jsonVolumeInfo["subtitle"];

            book.Authors = "null";
            
            book.Publisher = (string) jsonVolumeInfo["publisher"];
            book.PublishedDate = (string) jsonVolumeInfo["publishedDate"];
            book.Description = (string) jsonVolumeInfo["description"];

            JsonArray isbn = jsonVolumeInfo["industryIdentifiers"].AsArray();
         //   int[] isbnArray = {Convert.ToInt16(isbn[0]["identifier"]),Convert.ToInt16(isbn[1]["identifier"])};
            book.Isbn10 = 10;//isbnArray.Min();
            book.Isbn13 = 13;//isbnArray.Max();

            book.Pages = (int) jsonVolumeInfo["pageCount"].AsValue();

            book.Categories = "null";


            //try { book.Categories = (string)jsonVolumeInfo["categories"].AsArray().AsValue(); }
            //catch{ book.Categories = "null";}
            
            book.Language = (string) jsonVolumeInfo["language"];
            try { book.Cover = (string)jsonVolumeInfo["imageLinks"].AsObject()["thumbnail"].AsValue(); }
            catch{ book.Cover = "null";}
            Books.Add(book);
        }
        return Books;
    }
}