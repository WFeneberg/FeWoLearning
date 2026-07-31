// Exercise 062 — defineAsyncComponent with retry/error options (intermediate).
// Goal:   configure defineAsyncComponent so a failed load is retried once
//         before falling back to an error component.
// Drills: defineAsyncComponent options, the onError(error, retry, fail, attempts)
//         hook, loading/error components, async component lifecycle.
import { defineAsyncComponent, h, type AsyncComponentLoader, type Component } from "vue";

// Shown while the loader's promise is pending.
export const LoadingComponent: Component = {
  name: "AsyncLoading",
  render: () => h("div", { class: "async-loading" }, "Loading..."),
};

// Shown once the loader has failed and no more retries are allowed.
export const ErrorComponent: Component = {
  name: "AsyncError",
  render: () => h("div", { class: "async-error" }, "Failed to load component"),
};

/**
 * Wrap `loader` in a defineAsyncComponent config that retries a failed load
 * exactly once before giving up and rendering `ErrorComponent`.
 */
export function createRetryAsyncComponent(_loader: AsyncComponentLoader): Component {
  throw new Error("TODO: implement createRetryAsyncComponent");
}
