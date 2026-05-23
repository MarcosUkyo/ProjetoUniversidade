using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjetoUniversidade.Autenticacao;

namespace ProjetoUniversidade.Filters
{
    /// <summary>
    /// Verifica se o usuário está logado via sessão.
    /// Opcionalmente restringe a um ou mais roles (CSV): RoleAnyOf = "Admin,Biblioteca"
    /// </summary>
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        /// <summary>Roles permitidos, separados por vírgula. Null = qualquer role logado.</summary>
        public string? RoleAnyOf { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session  = context.HttpContext.Session;
            var userId   = session.GetInt32(SessionKeys.UserId);
            var userRole = session.GetString(SessionKeys.UserRole);

            // Não autenticado → redireciona para o login
            if (userId == null)
            {
                var returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToActionResult("Login", "Auth",
                    new { returnUrl });
                return;
            }

            // Verifica role, se exigido
            if (!string.IsNullOrWhiteSpace(RoleAnyOf))
            {
                var allowed = RoleAnyOf.Split(',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

                if (!allowed.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult(
                        "AcessoNegado", "Auth", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
