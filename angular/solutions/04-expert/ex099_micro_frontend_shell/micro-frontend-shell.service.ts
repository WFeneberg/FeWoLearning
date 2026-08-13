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

// Exercise 099 — a micro-frontend shell: independent mount/unmount with real DI isolation
// (reference solution).

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

  mount(id: string, container: HTMLElement): CounterStore {
    if (this.mounts.has(id)) {
      throw new RangeError(`micro-app already mounted: ${id}`);
    }

    // A child EnvironmentInjector, not a providedIn:'root' service — each mount's CounterStore is
    // its own instance, never shared with a sibling mount of the same "app".
    const injector = createEnvironmentInjector(
      [CounterStore],
      this.platformInjector,
      `microapp:${id}`,
    );
    const store = injector.get(CounterStore);

    const onClick = () => store.increment();
    container.addEventListener("click", onClick);

    // DestroyRef scoped to THIS injector: the callback fires exactly when injector.destroy() runs,
    // so unmount() doesn't need to know a listener was ever wired up.
    runInInjectionContext(injector, () => {
      inject(DestroyRef).onDestroy(() => container.removeEventListener("click", onClick));
    });

    this.mounts.set(id, injector);
    this.mountedIds.set([...this.mounts.keys()]);
    return store;
  }

  unmount(id: string): void {
    const injector = this.mounts.get(id);
    if (!injector) {
      throw new RangeError(`no micro-app mounted with id: ${id}`);
    }

    injector.destroy(); // runs CounterStore's ngOnDestroy AND the DestroyRef listener cleanup
    this.mounts.delete(id);
    this.mountedIds.set([...this.mounts.keys()]);
  }

  storeFor(id: string): CounterStore | null {
    const injector = this.mounts.get(id);
    return injector ? injector.get(CounterStore) : null;
  }
}
