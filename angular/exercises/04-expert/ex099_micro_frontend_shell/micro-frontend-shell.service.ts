import {
  DestroyRef,
  EnvironmentInjector,
  Injectable,
  OnDestroy,
  createEnvironmentInjector,
  inject,
  runInInjectionContext,
  signal,
} from "@angular/core";

// Exercise 099 — a micro-frontend shell: independent mount/unmount with real DI isolation (expert).
// Goal:   let a shell mount several independent "micro-app" instances side by side and tear any ONE
//         of them down cleanly — without a real, separately-bootstrapped Angular application per
//         instance (fragile enough in a real browser; two `bootstrapApplication()` calls sharing one
//         jsdom `document` in a Jest run is not a foundation worth building an exercise on).
// Drills: `EnvironmentInjector` / `createEnvironmentInjector` as the actual isolation primitive real
//         micro-frontend shells build on, `DestroyRef.onDestroy` for teardown that a mount registers
//         for itself, and `ngOnDestroy` as proof an injector's own providers were really torn down.
// Passes: when `npx jest exercises/04-expert/ex099_micro_frontend_shell` is green.
//
// The isolation contract this shell exists to enforce: mounting the "same" micro-app twice must NOT
// mean two mounts silently sharing one instance of its state. A `providedIn: 'root'` service would
// do exactly that — one singleton, shared by every mount, exactly the bug a real shell cannot ship
// with. The fix is the same one exercise 020 used at component scope, one level up: each mount gets
// its own CHILD `EnvironmentInjector`, created fresh via `createEnvironmentInjector`, with its own
// `CounterStore` provider — so two mounts of the same micro-app get two independent instances, and
// nothing one of them does is visible from the other.
//
// Real teardown means more than the shell forgetting a reference it was holding. It means the
// mounted app's own providers get to run `ngOnDestroy`, AND whatever that app wired up directly on
// the DOM (here: a click listener on its own container) gets a chance to unwind too — a leaked
// listener is a leaked micro-app, invisible in every test that only checks "is the object still
// there?". This shell asks each mount's OWN injector to register that cleanup via `DestroyRef`,
// rather than `unmount()` hand-rolling it — a mount can register work to unwind without the shell
// needing to know that work exists. `EnvironmentInjector.destroy()` is what fires all of it: the
// `ngOnDestroy` hooks of providers created in that injector, AND any `DestroyRef.onDestroy`
// callbacks registered within it.

/** A tiny "micro-app": one signal of state, and proof (`destroyed`) that it was torn down. */
@Injectable()
export class CounterStore implements OnDestroy {
  readonly count = signal(0);
  readonly destroyed = signal(false);

  increment(): void {
    this.count.update((value) => value + 1);
  }

  ngOnDestroy(): void {
    this.destroyed.set(true);
  }
}

@Injectable({ providedIn: "root" })
export class MicroFrontendShellService {
  private readonly platformInjector = inject(EnvironmentInjector);
  private readonly mounts = new Map<string, EnvironmentInjector>();

  readonly mountedIds = signal<readonly string[]>([]);

  /**
   * TODO: implement mount.
   *   - Throw a RangeError if `id` is already mounted — mount ids must be unique while active.
   *   - Create a fresh EnvironmentInjector via `createEnvironmentInjector([CounterStore],
   *     this.platformInjector, ...)` so this mount gets its OWN CounterStore instance.
   *   - Get that instance with `injector.get(CounterStore)`.
   *   - Add a "click" listener on `container` that calls the mount's `store.increment()`.
   *   - Register — via `runInInjectionContext(injector, () => inject(DestroyRef).onDestroy(...))` —
   *     that the listener is removed again when THIS injector is destroyed. `unmount()` must not
   *     need to know the listener exists.
   *   - Record the mount, update mountedIds (in insertion order), and return the CounterStore.
   */
  mount(id: string, container: HTMLElement): CounterStore {
    throw new Error("TODO: implement mount");
  }

  /**
   * TODO: implement unmount.
   *   - Throw a RangeError if `id` is not currently mounted.
   *   - Destroy that mount's injector — this must run its DestroyRef callback (removing the click
   *     listener) AND CounterStore's ngOnDestroy (setting `destroyed` true).
   *   - Remove it from the registry and update mountedIds.
   *   - Must have NO effect on any other mounted id.
   */
  unmount(id: string): void {
    throw new Error("TODO: implement unmount");
  }

  /** The CounterStore for a currently-mounted id, or null if nothing is mounted there. */
  storeFor(id: string): CounterStore | null {
    throw new Error("TODO: implement storeFor");
  }
}
