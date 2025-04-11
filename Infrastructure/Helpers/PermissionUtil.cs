using DotNetService.Constants.Permission;

namespace DotNetService.Infrastructure.Helpers
{
  public static class PermissionUtil
  {
    public static void ValidatePermission(string[] userPermissions, string[] requiredPermissions)
    {
      if (Array.FindIndex(requiredPermissions, s => s.Equals(PermissionConstant.DEPRECATED, StringComparison.OrdinalIgnoreCase)) >= 0)
      {
        throw new UnauthorizedAccessException();
      }

      if (Array.FindIndex(userPermissions, s => s.Equals(PermissionConstant.ALL, StringComparison.OrdinalIgnoreCase)) >= 0)
      {
        return;
      }

      // Validate if user has all required permissions
      var isValid = requiredPermissions.All(requiredPermission =>
        userPermissions.Any(
          userPermission => userPermission.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase)
        )
      );

      if (!isValid)
      {
        throw new UnauthorizedAccessException();
      }
    }
  }
}