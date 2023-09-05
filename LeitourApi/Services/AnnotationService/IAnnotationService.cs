using LeitourApi.Models;

public interface IAnnotationService{
    public Task<List<Annotation>>? GetByAnnotationId(int savedBookId, int userId);
    public Task<List<SavedBook>?> GetBySavedId(int id);

    public Task<SavedBook>? SwitchPublic(int id);

    public void AlterAnnotation(Annotation annotation);

    public void DeleteAnnotation(Annotation annotation);

    public void CreateAnnotation(Annotation annotation);

    public Task<string> UnsaveBook(int savedBookId);
}