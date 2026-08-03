// Exercise 099 — Micro-frontend mount (reference solution).
import { createApp, h, type App, type Component } from "vue";

export interface MountOptions {
  props?: Record<string, unknown>;
  provides?: Record<string | symbol, unknown>;
  configure?: (app: App) => void;
}

export interface MicroFrontendHandle {
  app: App;
  container: Element;
  readonly isMounted: boolean;
  unmount: () => void;
}

let liveMounts = 0;

export function activeMountCount(): number {
  return liveMounts;
}

export function mountMicroFrontend(
  component: Component,
  container: Element,
  options: MountOptions = {},
): MicroFrontendHandle {
  if (!(container instanceof Element)) {
    throw new TypeError("mountMicroFrontend: container must be an Element");
  }

  // One app per mount is the whole point: app-level provides, plugins and config
  // stay scoped to this instance instead of being shared globally.
  const app = createApp({
    render: () => h(component, options.props ?? {}),
  });

  // Reflect.ownKeys so symbol keys survive — plain Object.keys would drop them.
  for (const key of Reflect.ownKeys(options.provides ?? {})) {
    app.provide(key as string | symbol, (options.provides as Record<string | symbol, unknown>)[key as string]);
  }

  options.configure?.(app);

  app.mount(container);
  liveMounts += 1;

  let mounted = true;

  return {
    app,
    container,
    get isMounted() {
      return mounted;
    },
    unmount() {
      // Guard so a second call neither throws nor double-decrements the counter.
      if (!mounted) return;
      mounted = false;
      liveMounts -= 1;
      app.unmount();
      container.innerHTML = "";
    },
  };
}
