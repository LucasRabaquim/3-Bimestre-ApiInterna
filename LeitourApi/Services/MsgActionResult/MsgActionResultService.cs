using LeitourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.MsgActionResult;

[ApiExplorerSettings(IgnoreApi = true)]
public class MsgActionResultService : ControllerBase
{
        public ActionResult MsgUserNotFound() => NotFound("O usuário não existe.");
        public ActionResult MsgInvalid() => BadRequest("Autenticação invalida, logue novamente.");

        public ObjectResult MsgInternalError(string obj,string acao) => StatusCode(StatusCodes.Status500InternalServerError, $"A {acao} de {obj} não foi bem sucedida.");  
        
        public ActionResult MsgPageNotFound() => NotFound("A página não foi encontrada");
        public ActionResult MsgWrongPassword() => BadRequest("Senha incorreta.");
        public ActionResult MsgPostNotFound() => NotFound("O post não foi encontrado");
       
        public ActionResult MsgInvalidPost() => BadRequest("Algo deu errado na atualização do post");
        public ActionResult MsgAlreadyExists() => BadRequest("Já existe usuário com esse email.");
        public ActionResult MsgAnnotationPrivated() => BadRequest("Estas anotações são privadas");
        public ActionResult MsgAnnotationNotFound() => BadRequest("As anotações não foram encontradas");
        public ActionResult MsgAnnotationCreated() => Ok("As anotações foram criadas");
        public ActionResult MsgAnnotationUpdated() => Ok("As anotações foram atualizadas");
        public ActionResult MsgAnnotationDeleted() => Ok("As anotações foram deletadas");
        public ActionResult MsgSavedBookNotFound() => BadRequest("O livro não está salvo");
        public ActionResult MsgAnnotationToPrivate() => Ok("Agora só você pode ver as anotações deste livro");
        public ActionResult MsgAnnotationToPublic() => Ok("Agora todos podem ver as anotações sobre esse livro");
        public ActionResult MsgBookNotReturned() => StatusCode(StatusCodes.Status500InternalServerError,"Falha ao pesquisar livro. Verifique a conexão de internet.");
        public ActionResult MsgBookNotFound() => NotFound("Não foi encontrado nenhum livro com o parâmetro pesquisado.");
        public ActionResult MsgNoSavedBook() => NotFound("Você não tem nenhum livro salvo.");
        public ActionResult MsgDebugValue(string message) => Ok(message);
        public ActionResult MsgBookUnsaved() => Ok("Você dessalvou o livro.");
}