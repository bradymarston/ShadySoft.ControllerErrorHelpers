using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ShadySoft.ControllerErrorHelpers.Extensions.DepedencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddControllerErrorHelpers(this IServiceCollection services)
        {
            return services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(ProblemDetailsGenerators.Generate(context.ModelState)));
        }
    }
}
