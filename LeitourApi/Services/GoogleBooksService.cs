

using LeitourApi.Models;
using System.Text.Json;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LeitourApi.Services;

public class GoogleBooksService
{
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


    public static List<GoogleBooks> formatResponse(string response)
    {
        var jsonObject = JsonNode.Parse(response);
        JsonArray jsonBookArray = jsonObject["items"].AsArray();
        List<GoogleBooks> Books = new List<GoogleBooks> { };
        foreach (JsonObject? jsonItems in jsonBookArray)
        {
            GoogleBooks book = new GoogleBooks();
            book.Key = jsonItems["id"].AsValue().ToString();

            JsonObject jsonVolumeInfo = jsonItems["volumeInfo"].AsObject();

            book.Title = (string)jsonVolumeInfo["title"];
            book.Subtitle = (string)jsonVolumeInfo["subtitle"];
            book.Authors = "null";
            book.Publisher = (string)jsonVolumeInfo["publisher"];
            book.PublishedDate = (string)jsonVolumeInfo["publishedDate"];
            book.Description = (string) jsonVolumeInfo["description"];
            book.Isbn10 = 10;
            book.Pages = 0;//(int) jsonVolumeInfo[pageCount"];
            book.Categories = "null";
            book.Language = (string)jsonVolumeInfo["language"];
            book.Cover = "null";
            Books.Add(book);
        }
        return Books;
    }

   /* public JsonAttribute verifyAtrr(JsonObject obj, string attr)
    {
        return ((JsonObject) obj[attr] != null);
    }*/
}