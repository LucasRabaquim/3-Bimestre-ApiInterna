using LeitourApi.Models;

public interface IAnnotationService{
    public Task<List<SavedBook>?> GetAllSaved(int id);
    public Task<SavedBook?> GetSavedById(int id);

    public Task SwitchPublic(SavedBook savedBook);

    public Task<Annotation>? GetAnnotation(int id);
    public Task<List<Annotation>>? GetAllAnnotations(int savedBookId);
    public Task AlterAnnotation(Annotation annotation);

    public Task DeleteAnnotation(Annotation annotation);

    public Task CreateAnnotation(Annotation annotation);

    public Task SaveBook(SavedBook savedBook);
    public Task UnsaveBook(SavedBook savedBook);
}