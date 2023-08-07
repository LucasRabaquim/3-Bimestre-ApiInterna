using System;
using System.Collections.Generic;

namespace LeitourApi.Models;

public partial class GoogleBooks
{
    public string Key { get; set; }
    public string Title {get;set;}
    public string Subtitle {get;set;}
    public string Authors {get;set;}
    public string Publisher {get;set;}
    public string PublishedDate {get;set;}
    public string Description {get;set;}
    public int Isbn10 {get;set;}
    public int Isbn13 {get;set;}
    public int Pages {get;set;}
    public string Categories {get;set;}
    public string Language {get;set;} 
    public string Cover {get;set;}

    public GoogleBooks(){}
}