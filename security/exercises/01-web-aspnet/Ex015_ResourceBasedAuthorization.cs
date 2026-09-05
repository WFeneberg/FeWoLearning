using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 015 — ResourceBasedAuthorization (web-aspnet).
// Goal:   Authorize an action against a specific resource instance rather than
//         a role alone - reading and deleting a document are separate
//         decisions that both key off who owns it, and an administrator role
//         must not be a blanket override for either.
// Drills: IAuthorizationService on a resource instance, ownership checks.
// Passes: attack facts   - a principal that is not the document's owner is
//                          denied both read (PolicyName) and delete
//                          (DeletePolicyName); an anonymous principal is
//                          denied;
//         use facts      - the owner is allowed to read and to delete, and a
//                          principal holding an admin role is allowed to read
//                          but is still denied delete.
public sealed record Ex015_Document(int Id, string OwnerId, string Body);

public static class Ex015_ResourceBasedAuthorization
{
    public const string PolicyName = "DocumentOwner";
    public const string DeletePolicyName = "DocumentOwnerDelete";

    public static void AddServices(IServiceCollection services) =>
        throw new NotImplementedException(
            "TODO: Ex015 - register a DocumentOwner policy (owner or admin may read) and a DocumentOwnerDelete policy (owner only, never admin) evaluated against the Ex015_Document resource");
}
