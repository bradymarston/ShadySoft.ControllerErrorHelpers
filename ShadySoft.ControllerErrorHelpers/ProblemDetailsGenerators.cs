using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadySoft.ControllerErrorHelpers
{
    internal static class ProblemDetailsGenerators
    {
        public static ProblemDetails Generate(ModelStateDictionary state)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "ValidationError",
                Title = "One or more validation errors occurred."
            };

            problemDetails.AddErrors(state.Select(p => new ErrorItem()
            {
                Key = string.IsNullOrEmpty(p.Key) ? "" : char.ToLower(p.Key[0]) + p.Key.Substring(1),
                Descriptions = p.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            }));

            return problemDetails;
        }

        public static ProblemDetails Generate(IdentityResult result)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "IdentityError",
                Title = "One or more errors occurred in the identity system."
            };

            problemDetails.AddErrors(result.Errors.Select(e => new ErrorItem()
            {
                Key = e.Code,
                Descriptions = new string[] { e.Description }
            }));

            return problemDetails;
        }

        public static ProblemDetails Generate(Microsoft.AspNetCore.Identity.SignInResult result)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "SignInError",
                Title = "One or more errors during sign-in."
            };

            var reason = "InvalidSignIn";

            if (result.RequiresTwoFactor)
                reason = "TwoFactor";

            if (result.IsLockedOut)
                reason = "LockedOut";

            if (result.IsNotAllowed)
                reason = "CredentialUnconfirmed";

            problemDetails.AddExtension("reason", reason);

            if (reason == "InvalidSignIn")
                problemDetails.AddErrors(new List<ErrorItem>()
                {
                    new ErrorItem()
                    {
                        Key = "Credentials",
                        Descriptions = new string[] { "Invalid email address or password." }
                    }
                });

            return problemDetails;
        }

        private static void AddErrors(this ProblemDetails problemDetails, IEnumerable<ErrorItem> errors)
        {
            var errorExtension = new Dictionary<string, string[]>();

            if (errors.Count() > 0)
                foreach (var error in errors)
                {
                    errorExtension.Add(error.Key, error.Descriptions);
                }

            problemDetails.AddExtension("errors", errorExtension);
        }

        private static void AddExtension(this ProblemDetails problemDetails, string key, object data)
        {
            problemDetails.Extensions.Add(key, data);
        }

        private class ErrorItem
        {
            public string Key { get; set; } = "";
            public string[] Descriptions { get; set; } = { };
        }
    }
}
