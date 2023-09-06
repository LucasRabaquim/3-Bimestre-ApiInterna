using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LeitourApi.Services.AnnotationService;

public class AnnotationService : IAnnotationService{

    private readonly LeitourContext _context;
    public AnnotationService(LeitourContext context){ 
        _context = context;
    }

    public async Task<List<Annotation>>? GetByAnnotationId(int savedBookId, int userId){
        if(_context.Annotations == null) 
            return null;
        var savedBook = await _context.SavedBooks.FindAsync(savedBookId);
        if (savedBook == null)
            return null;
        if(userId != savedBook.UserId && !savedBook.Public)
            return null;
        List<Annotation> list = await _context.Annotations.Where(annotation => annotation.SavedBookId == savedBook.SavedId).ToListAsync();
    
        return list;
    }

    public async Task<List<SavedBook>?> GetBySavedId(int id){
        if(_context.Annotations == null) 
            return null;
        List<SavedBook> savedBook = await _context.SavedBooks.
            Where(SavedBooks => SavedBooks.UserId == id).ToListAsync();
        return savedBook;
    }

    public async Task<SavedBook>? SwitchPublic(int id){
        if(_context.Annotations == null) 
            return null;
        SavedBook savedBook = await _context.SavedBooks.FindAsync(id);
        savedBook.AlteratedDate = DateTime.Now;
        savedBook.Public = !savedBook.Public;
        return savedBook;
    }

    public async void AlterAnnotation(Annotation annotation){    
        annotation.AlteratedDate = DateTime.Now;
        _context.Entry(annotation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async void DeleteAnnotation(Annotation annotation){    
        _context.Annotations.Remove(annotation);
        await _context.SaveChangesAsync();
    }

    public async void CreateAnnotation(Annotation annotation){
        _context.Annotations.Add(annotation);
        await _context.SaveChangesAsync();
    }

    public async Task<string> UnsaveBook(int savedBookId){
        if(_context.Annotations == null) 
            return null;
        var savedBook = await _context.SavedBooks.FindAsync(savedBookId);
        if (savedBook == null)
            return null;
       
        List<Annotation> list = await _context.Annotations.Where(annotation => annotation.SavedBookId == savedBook.SavedId).ToListAsync();

        foreach(Annotation annotation in list)
            DeleteAnnotation(annotation);
        
        _context.SavedBooks.Remove(savedBook);
        await _context.SaveChangesAsync();

        return "Success";
    }

}