import { EffectRef } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { ThemeStore } from "./theme-effects";

describe("ThemeStore effects", () => {
  let store: ThemeStore;

  /** Effects need an injection context to be created in. */
  const inContext = <T>(build: () => T): T => TestBed.runInInjectionContext(build);

  /** Effects are scheduled, not synchronous — this is what actually runs them. */
  const flush = (): void => TestBed.flushEffects();

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(ThemeStore);
  });

  it("runs once on creation", () => {
    inContext(() => store.watchTheme());

    // Nothing yet: creating an effect schedules it rather than running it.
    expect(store.written).toEqual([]);

    flush();

    expect(store.written).toEqual(["theme:light"]);
  });

  it("re-runs when its dependency changes", () => {
    inContext(() => store.watchTheme());
    flush();

    store.theme.set("dark");
    flush();

    expect(store.written).toEqual(["theme:light", "theme:dark"]);
  });

  it("does not re-run for an unrelated change", () => {
    inContext(() => store.watchTheme());
    flush();
    expect(store.written).toEqual(["theme:light"]);

    store.fontSize.set(20);
    flush();

    // It never read fontSize, so it never subscribed to it.
    expect(store.written).toEqual(["theme:light"]);
  });

  it("does not re-run for a set to the same value", () => {
    inContext(() => store.watchTheme());
    flush();

    store.theme.set("light");
    flush();

    expect(store.written).toEqual(["theme:light"]);
  });

  it("tracks every signal it reads, with no dependency list", () => {
    inContext(() => store.watchBoth());
    flush();
    expect(store.written).toEqual(["both:light/14"]);

    store.fontSize.set(16);
    flush();
    store.theme.set("dark");
    flush();

    expect(store.written).toEqual(["both:light/14", "both:light/16", "both:dark/16"]);
  });

  it("stops when destroyed", () => {
    const ref = inContext(() => store.watchTheme()) as EffectRef;
    flush();

    ref.destroy();
    store.theme.set("dark");
    flush();

    expect(store.written).toEqual(["theme:light"]);
  });

  it("runs a cleanup before each re-run", () => {
    inContext(() => store.watchWithCleanup());
    flush();
    expect(store.written).toEqual(["open:light"]);
    expect(store.cleanups).toEqual([]);

    store.theme.set("dark");
    flush();

    // The previous run is torn down first, and it remembers its own value.
    expect(store.cleanups).toEqual(["close:light"]);
    expect(store.written).toEqual(["open:light", "open:dark"]);
  });

  it("runs the cleanup on destruction too", () => {
    const ref = inContext(() => store.watchWithCleanup()) as EffectRef;
    flush();

    ref.destroy();

    expect(store.cleanups).toEqual(["close:light"]);
  });

  it("cleans up each run exactly once", () => {
    inContext(() => store.watchWithCleanup());
    flush();
    store.theme.set("dark");
    flush();
    store.theme.set("light");
    flush();

    expect(store.cleanups).toEqual(["close:light", "close:dark"]);
    expect(store.written).toEqual(["open:light", "open:dark", "open:light"]);
  });

  it("ignores an untracked read", () => {
    inContext(() => store.watchThemeIgnoringSize());
    flush();
    expect(store.written).toEqual(["themeOnly:light/14"]);

    store.fontSize.set(20);
    flush();

    // Read, current, and not subscribed to.
    expect(store.written).toEqual(["themeOnly:light/14"]);
  });

  it("picks the untracked value up on the next real run", () => {
    inContext(() => store.watchThemeIgnoringSize());
    flush();
    store.fontSize.set(20);
    flush();

    store.theme.set("dark");
    flush();

    expect(store.written).toEqual(["themeOnly:light/14", "themeOnly:dark/20"]);
  });

  it("refuses to be created outside an injection context", () => {
    // It has a lifetime, so it needs an injector to be tied to.
    expect(() => store.watchTheme()).toThrow(/NG0203/);
  });
});
