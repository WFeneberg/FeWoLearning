// Exercise 099 — Micro-frontend mount (expert).
// Goal:   mount independent Vue applications into arbitrary DOM containers, each
//         with its own app instance, its own provides and its own plugins, and
//         each tearing down cleanly. This is the shell side of a micro-frontend:
//         several apps coexist on one page without sharing global state.
// Drills: createApp per mount (not one shared app), app-level provide, plugin
//         installation, unmount and container cleanup, idempotent teardown.
import { type App, type Component } from "vue";

export interface MountOptions {
  /** Props handed to the root component. */
  props?: Record<string, unknown>;
  /** App-level provides. Must not leak into any other mounted app. */
  provides?: Record<string | symbol, unknown>;
  /** Escape hatch for plugin installation etc., called before mount. */
  configure?: (app: App) => void;
}

export interface MicroFrontendHandle {
  /** The underlying app instance, so callers can inspect or extend it. */
  app: App;
  /** The container this instance rendered into. */
  container: Element;
  /** True until `unmount()` has run. */
  readonly isMounted: boolean;
  /**
   * Unmounts the app and empties the container. Calling it more than once must be
   * harmless — a second call does nothing rather than throwing.
   */
  unmount: () => void;
}

/** How many handles are currently mounted. Used by the shell (and by tests). */
export function activeMountCount(): number {
  throw new Error("TODO: implement activeMountCount");
}

/**
 * Mounts `component` into `container` as its own application.
 *
 * Requirements:
 *  - one `createApp` per call, so two mounts of the same component share nothing;
 *  - apply `provides` at app level (`app.provide`) so injections resolve per app;
 *  - call `configure(app)` before mounting, if given;
 *  - throw a TypeError when `container` is not an Element;
 *  - `unmount()` empties the container and is idempotent;
 *  - `activeMountCount()` reflects the number of live handles.
 */
export function mountMicroFrontend(
  _component: Component,
  _container: Element,
  _options?: MountOptions,
): MicroFrontendHandle {
  throw new Error("TODO: implement mountMicroFrontend");
}
