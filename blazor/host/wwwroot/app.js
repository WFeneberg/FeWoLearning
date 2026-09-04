// Real module backing Ex052_JsInteropModule's dynamic import
// (JS.InvokeAsync<IJSObjectReference>("import", "./app.js")). Its own top-level
// statement runs once, the moment the browser actually imports it - not something a
// simple HTTP GET of the host page can observe, since the import only happens once
// the InteractiveServer circuit is live (see Ex052.razor for why prerender is
// disabled on the component rather than the page).
window.__ex052ModuleLoaded = true;
