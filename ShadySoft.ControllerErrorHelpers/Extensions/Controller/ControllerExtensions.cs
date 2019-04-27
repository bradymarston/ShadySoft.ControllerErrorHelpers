using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ShadySoft.ControllerErrorHelpers.Extensions.Controller
{
    public static class ControllerExtensions
    {
        public static IActionResult SignInFailure(this ControllerBase controller, Microsoft.AspNetCore.Identity.SignInResult result)
        {
            var problemDetails = ProblemDetailsGenerators.Generate(result);

            if ((string)problemDetails.Extensions["reason"] == "InvalidSignIn")
            {
                return controller.BadRequest(problemDetails);
            }

            return controller.Unauthorized(problemDetails);
        }

        public static IActionResult IdentityFailure(this ControllerBase controller, IdentityResult result)
        {
            return controller.BadRequest(ProblemDetailsGenerators.Generate(result));
        }

        public static IActionResult IdentityFailure(this ControllerBase controller, string code, string description)
        {
            var result = IdentityResult.Failed(new IdentityError() { Code = code, Description = description });

            return controller.BadRequest(ProblemDetailsGenerators.Generate(result));
        }

        public static IActionResult BadModel(this ControllerBase controller, string key, string errorMessage)
        {
            controller.ModelState.AddModelError(key, errorMessage);
            return controller.BadModel();
        }

        public static IActionResult BadModel(this ControllerBase controller)
        {
            return controller.BadRequest(ProblemDetailsGenerators.Generate(controller.ModelState));
        }
    }
}