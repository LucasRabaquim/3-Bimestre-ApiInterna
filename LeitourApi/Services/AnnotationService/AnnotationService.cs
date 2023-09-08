using LeitourApi.Controllers;
using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LeitourApi.Services.AnnotationService;

public class AnnotationService : IAnnotationService{

    private readonly LeitourContext _context;
    public AnnotationService(LeitourContext context) => _context = context;

    public async Task<List<SavedBook>?> GetAllSaved(int id){
        if(_context.SavedBooks == null) 
            return null;
        List<SavedBook> savedBook = await _context.SavedBooks.
            Where(SavedBooks => SavedBooks.UserId == id).ToListAsync();
        return savedBook;
    }

    public async Task<SavedBook?> GetSavedById(int id){
        if(_context.SavedBooks == null) 
            return null;
        SavedBook? savedBook = await _context.SavedBooks.FindAsync(id);
        return savedBook;
    }

    public async Task SwitchPublic(SavedBook savedBook){
        savedBook.Public = !savedBook.Public;
        _context.Entry(savedBook).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<Annotation>? GetAnnotation(int id){
        if(_context.Annotations == null) 
            return null;
        var annotation = await _context.Annotations.Where(annotation => 
            annotation.AnnotationId == id).FirstOrDefaultAsync();
        return annotation;
    }
    public async Task<List<Annotation>>? GetAllAnnotations(int savedBookId){
        if(_context.Annotations == null) 
            return null;
        List<Annotation> list = await _context.Annotations.Where(annotation => annotation.SavedBookId == savedBookId).ToListAsync();
        return list;
    }

    


    public async Task AlterAnnotation(Annotation annotation){    
        annotation.AlteratedDate = DateTime.Now;
        _context.Entry(annotation).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnnotation(Annotation annotation){    
        _context.Annotations.Remove(annotation);
        await _context.SaveChangesAsync();
    }

    public async Task CreateAnnotation(Annotation annotation){
        await _context.Annotations.AddAsync(annotation);
        await _context.SaveChangesAsync();
    }

    public async Task SaveBook(SavedBook savedBook){
        await _context.SavedBooks.AddAsync(savedBook);
        await _context.SaveChangesAsync();
    }
    
    public async Task UnsaveBook(SavedBook savedBook){
        List<Annotation> list = await _context.Annotations.Where(annotation => annotation.SavedBookId == savedBook.SavedId).ToListAsync();

        foreach(Annotation annotation in list)
            await DeleteAnnotation(annotation);
        
        _context.SavedBooks.Remove(savedBook);
        await _context.SaveChangesAsync();
    }

}