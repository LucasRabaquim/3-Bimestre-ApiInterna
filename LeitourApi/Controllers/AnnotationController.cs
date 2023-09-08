using Microsoft.AspNetCore.Mvc;
using LeitourApi.Models;
using LeitourApi.Services;
using LeitourApi.Services.UserService;
using LeitourApi.Services.AnnotationService;
using LeitourApi.Services.MsgActionResult;
using Microsoft.AspNetCore.WebUtilities;

namespace LeitourApi.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class AnnotationsController : ControllerBase
    {
        public readonly IUserService _userService;
        public readonly IAnnotationService _AnnotationService;
        public readonly MsgActionResultService _msgService;

        public AnnotationsController(IUserService userService, IAnnotationService AnnotationService, MsgActionResultService msgService)
        {
            _userService = userService;
            _AnnotationService = AnnotationService;
            _msgService = msgService;
        }

        [HttpGet("SavedBooks")]
        public async Task<ActionResult<List<SavedBook>>>? GetUserBooks([FromHeader] string token)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetAllSaved(userId);
            return (saved == null) ? _msgService.MsgNoSavedBook() : saved;
        }

        [HttpGet("SavedBooks/{id}")]
        public async Task<ActionResult<SavedBook>?> GetBySavedId([FromHeader] string token, int id)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetSavedById(id);
            return (saved == null) ? _msgService.MsgSavedBookNotFound() : saved;
        }

        [HttpPut("SavedBooks/{id}")]
        public async Task<ActionResult<SavedBook>>? SwitchPublic([FromHeader] string token, int id)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetSavedById(id);
            if(saved == null) 
                return _msgService.MsgSavedBookNotFound();
            await _AnnotationService.SwitchPublic(saved);
            return !saved.Public ? _msgService.MsgAnnotationToPublic() : _msgService.MsgAnnotationToPrivate();
        }

        [HttpPost("SavedBooks")]
        public async Task<ActionResult<SavedBook>?> SaveBook([FromHeader] string token, SavedBook newSaved)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            newSaved.UserId = userId;
            await _AnnotationService.SaveBook(newSaved);
            return CreatedAtAction("SaveBook",newSaved);
        }

        [HttpDelete("SavedBooks/{id}")]
        public async Task<ActionResult<SavedBook>>? UnsaveBook([FromHeader] string token, int id)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetSavedById(id);
            if(saved == null) 
            return _msgService.MsgSavedBookNotFound();
            await _AnnotationService.UnsaveBook(saved);
            return _msgService.MsgAnnotationDeleted();
        }


        [HttpGet("SavedBooks/annotation/{id}/")]
        public async Task<ActionResult<Annotation>> GetAnnotation([FromHeader] string token, int id)
        {
            var annotation = await _AnnotationService.GetAnnotation(id);
            if(annotation == null)
                return _msgService.MsgAnnotationNotFound();
            var saved = await _AnnotationService.GetSavedById(annotation.SavedBookId);
    
            if(!saved.Public){
                int userId = TokenService.DecodeToken(token);
                User? user = await _userService.GetById(userId);
                if (user == null)
                    return _msgService.MsgInvalid();
                if(saved.UserId != userId)
                    return _msgService.MsgAnnotationPrivated();
            }
            
            return annotation;
        }

        [HttpGet("SavedBooks/{savedId}/annotation")]
        public async Task<ActionResult<IEnumerable<Annotation>>> GetAllAnnotations([FromHeader] string token,int savedId)
        {
            var saved = await _AnnotationService.GetSavedById(savedId);
            if (saved == null)
                return _msgService.MsgSavedBookNotFound();
                
            if(!saved.Public){
                int userId = TokenService.DecodeToken(token);
                User? user = await _userService.GetById(userId);
                if (user == null)
                    return _msgService.MsgInvalid();
                if(saved.UserId != userId)
                    return _msgService.MsgAnnotationPrivated();
            }
        
            var annotation = await _AnnotationService.GetAllAnnotations(savedId);
            if(annotation == null)
                return _msgService.MsgAnnotationNotFound();
            return annotation;
        }
        

        [HttpPost("SavedBooks/annotation")]
        public async Task<ActionResult<Annotation>> CreateAnnotation([FromHeader] string token, Annotation annotation)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();

            var saved = await _AnnotationService.GetSavedById(annotation.SavedBookId);
            if (saved == null)
                return _msgService.MsgSavedBookNotFound();
            
            await _AnnotationService.CreateAnnotation(annotation);

            return CreatedAtAction("CreateAnnotation", annotation);
        }

        [HttpPut("SavedBooks/annotation/{id}")]
        public async Task<ActionResult<Annotation>>? UpdateAnnotation([FromHeader] string token, int id, [FromBody] Annotation newAnnotation)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetSavedById(newAnnotation.SavedBookId);
            if (saved == null)
                return _msgService.MsgSavedBookNotFound();
            if(userId != saved.UserId)
                return _msgService.MsgInvalid();
            var annotation = await _AnnotationService.GetAnnotation(id);
            if(annotation == null)
                return _msgService.MsgAnnotationNotFound();
            await _AnnotationService.AlterAnnotation(newAnnotation);
            return _msgService.MsgAnnotationUpdated();

        }

        [HttpDelete("SavedBooks/{savedId}/annotation/{id}")]
        public async Task<ActionResult<Annotation>>? DeleteAnnotation([FromHeader] string token, int savedId, int id)
        {
            int userId = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(userId);
            if (user == null)
                return _msgService.MsgInvalid();
            var saved = await _AnnotationService.GetSavedById(savedId);
            if (saved == null)
                return _msgService.MsgSavedBookNotFound();
            if(userId != saved.UserId)
                return _msgService.MsgInvalid();
            var annotation = await _AnnotationService.GetAnnotation(id);
            if(annotation == null)
                return _msgService.MsgAnnotationNotFound();
            await _AnnotationService.DeleteAnnotation(annotation);
            return _msgService.MsgAnnotationDeleted();
        }
    }
}