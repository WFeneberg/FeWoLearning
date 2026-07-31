// Exercise 062 — defineAsyncComponent with retry/error options (reference solution).
import { defineAsyncComponent, h, type AsyncComponentLoader, type Component } from "vue";

export const LoadingComponent: Component = {
  name: "AsyncLoading",
  render: () => h("div", { class: "async-loading" }, "Loading..."),
};

export const ErrorComponent: Component = {
  name: "AsyncError",
  render: () => h("div", { class: "async-error" }, "Failed to load component"),
};

export function createRetryAsyncComponent(loader: AsyncComponentLoader): Component {
  return defineAsyncComponent({
    loader,
    loadingComponent: LoadingComponent,
    errorComponent: ErrorComponent,
    delay: 0,
    timeout: 10000,
    onError(error, retry, fail, attempts) {
      // Allow exactly one retry: on the first failure (attempts === 1) try
      // again, otherwise give up and let the error component render.
      console.log("ONERROR attempts", attempts);
      if (attempts <= 1) {
        retry();
      } else {
        fail();
      }
    },
  });
}
