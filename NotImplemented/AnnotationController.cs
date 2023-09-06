using Microsoft.AspNetCore.Mvc;
using LeitourApi.Models;
using LeitourApi.Services;
using LeitourApi.Services.UserService;
using LeitourApi.Services.AnnotationService;
using LeitourApi.Services.MsgActionResult;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnotationsController : ControllerBase
    {
        private readonly LeitourContext _context;

        public readonly IUserService _userService;
        public readonly IAnnotationService _AnnotationService;
        public readonly MsgActionResultService _msgService;
        public AnnotationsController(LeitourContext context, IUserService userService, IAnnotationService AnnotationService, MsgActionResultService msgService){
            _context = context;
            _userService = userService;
            _AnnotationService = AnnotationService;
            _msgService = msgService;
        }
        
    [HttpGet("/SavedBooks")]
    public async Task<ActionResult<List<SavedBook>>? GetUserBooks([FromHeader] string token){

        int userId = TokenService.DecodeToken(token);
        if(_userService.GetById(userId) == null)
            return _msgService.MsgInvalid();
            return (post == null) ? _msgService.MsgPostNotFound() : post;
    }
    public async  Task<List<SavedBook>?> GetBySavedId(int id);

    public async  Task<SavedBook>? SwitchPublic(int id);

    public async  void AlterAnnotation(Annotation annotation);

    public async  void DeleteAnnotation(Annotation annotation);

    public async  void CreateAnnotation(Annotation annotation);

    public async  Task<string> UnsaveBook(int savedBookId);
    }
}
