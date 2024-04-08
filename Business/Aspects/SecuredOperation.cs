using Castle.DynamicProxy;
using Core.Extensions;
using Core.Models;
using Core.Utilities.Interceptors;
using Core.Utilities.IOC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Aspects
{
    public class SecuredOperation(params OperationClaimEnum[] roles) : MethodInterception
    {
        private List<OperationClaimEnum> _roles = [OperationClaimEnum.admin, .. roles];
        private IHttpContextAccessor _httpContextAccessor = ServiceTool.ServiceProvider!.GetService<IHttpContextAccessor>()!;

        protected override void OnBefore(IInvocation invocation)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidProgramException();

            var roleClaims = httpContext.User.ClaimRoles();
            foreach (var role in _roles)
                if (roleClaims?.Contains(role) is true)
                    return;

            throw new UnauthorizedAccessException();
        }
    }
}
