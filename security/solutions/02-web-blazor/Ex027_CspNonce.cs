namespace FeWoLearning.Security.Exercises.WebBlazor;

// The cascaded value Ex027_CspNonceFlow reads: the per-request nonce a CSP
// header (row 003) issued, so a component two levels deep in the tree never
// has to be handed it as an ordinary [Parameter] threaded through every
// ancestor. Plain data - nothing here throws, there is nothing to implement.
public sealed record Ex027_CspNonce(string Value);
