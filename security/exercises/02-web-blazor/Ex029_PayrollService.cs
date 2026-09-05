using System.Security.Claims;

namespace FeWoLearning.Security.Exercises.WebBlazor;

// Exercise 029 — ClientAuthIsNotEnforcement (web-blazor).
// Goal:   Refuse a payroll approval unless the caller genuinely holds the
//         "approver" role - independent of whatever the client's UI chose to
//         show or hide. TryApprove is the server-side gate: it must reach
//         the same answer whether or not any component ever asked first.
// Drills: UI trimming is not authorization, server-side enforcement.
// Passes: attack facts - calling TryApprove directly, bypassing any
//                        component entirely, with a non-approver principal
//                        returns false; with an anonymous principal it also
//                        returns false;
//         use facts     - calling TryApprove with an approver principal
//                        returns true (paired with the component's own use
//                        facts - see Ex029_ClientAuthIsNotEnforcement.razor -
//                        so hiding the button and enforcing the rule are
//                        both demanded, never just one of the two).
public sealed class Ex029_PayrollService
{
    public bool TryApprove(ClaimsPrincipal caller, int requestId, out string? denial) =>
        throw new NotImplementedException(
            "TODO: Ex029 - false (with a denial reason) unless caller is authenticated and in the \"approver\" role");
}
