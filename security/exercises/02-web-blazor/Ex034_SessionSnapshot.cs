namespace FeWoLearning.Security.Exercises.WebBlazor;

// The payload Ex034_PersistentStateLeak persists across the prerender-to-
// interactive handoff: only what the next render actually needs to redraw
// the same view, never anything that authenticates or identifies the user.
// Plain data - nothing here throws, there is nothing to implement.
public sealed record Ex034_SessionSnapshot(string DisplayName, string LastViewedPage);
