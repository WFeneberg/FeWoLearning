namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 005 — ModelBindingOverposting (web-aspnet).
// Goal:   Apply a caller-supplied JSON body onto an existing user profile,
//         updating only DisplayName and Email - Id and IsAdministrator must
//         stay exactly as they were, no matter what the request body claims,
//         because a client can put any field name it likes in a JSON body.
// Drills: mass assignment, BindNever, explicit DTO projection.
// Passes: attack facts   - a body containing "isAdministrator": true leaves
//                          IsAdministrator false; a body containing "id": 999
//                          leaves Id unchanged;
//         use facts      - a body containing displayName and email updates
//                          exactly those two fields; a body containing only
//                          displayName leaves Email unchanged.
public sealed class Ex005_UserProfile
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsAdministrator { get; set; }
}

public static class Ex005_ModelBindingOverposting
{
    public static Ex005_UserProfile Apply(Ex005_UserProfile existing, string requestJson) =>
        throw new NotImplementedException(
            "TODO: Ex005 - project only DisplayName and Email from requestJson onto a copy of existing; Id and IsAdministrator must never be settable from the request body");
}
