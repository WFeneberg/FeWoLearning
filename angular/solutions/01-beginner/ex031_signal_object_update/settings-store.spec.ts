import { TestBed } from "@angular/core/testing";
import { DEFAULT_SETTINGS, SettingsStore } from "./settings-store";

describe("SettingsStore", () => {
  let store: SettingsStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(SettingsStore);
  });

  it("starts at the defaults and moves off them", () => {
    expect(store.settings()).toEqual(DEFAULT_SETTINGS);
    expect(store.isDefault()).toBe(true);
    expect(store.summary()).toBe("Anonymous · light · 25");

    store.toggleTheme();

    expect(store.isDefault()).toBe(false);
    expect(store.summary()).toBe("Anonymous · dark · 25");
  });

  it("toggles the theme", () => {
    store.toggleTheme();
    expect(store.settings().theme).toBe("dark");

    store.toggleTheme();
    expect(store.settings().theme).toBe("light");
  });

  it("replaces the object rather than mutating it", () => {
    const before = store.settings();

    store.toggleTheme();

    expect(store.settings()).not.toBe(before);
    // The old object is intact for anything still holding it.
    expect(before.theme).toBe("light");
  });

  it("leaves the other fields alone", () => {
    store.toggleTheme();

    expect(store.settings().pageSize).toBe(25);
    expect(store.settings().notifications).toBe(true);
  });

  it("keeps untouched nested objects as the same reference", () => {
    const profileBefore = store.settings().profile;

    store.toggleTheme();

    // A shallow copy: `profile` was not on the path being changed, so it is not rebuilt.
    expect(store.settings().profile).toBe(profileBefore);
  });

  it("sets the page size", () => {
    store.setPageSize(50);

    expect(store.settings().pageSize).toBe(50);
  });

  it("rejects a page size out of range", () => {
    expect(() => store.setPageSize(0)).toThrow(RangeError);
    expect(() => store.setPageSize(101)).toThrow(RangeError);
    expect(store.settings().pageSize).toBe(25);
  });

  it("patches several fields at once", () => {
    store.patch({ theme: "dark", pageSize: 10 });

    expect(store.settings().theme).toBe("dark");
    expect(store.settings().pageSize).toBe(10);
    expect(store.settings().notifications).toBe(true);
  });

  it("patches nothing when given nothing", () => {
    const before = store.settings();

    store.patch({});

    expect(store.settings()).toEqual(before);
  });

  it("renames the nested profile", () => {
    store.renameProfile("Ada");

    expect(store.settings().profile.name).toBe("Ada");
    expect(store.settings().profile.email).toBe("");
  });

  it("rebuilds every level down to the changed field", () => {
    const before = store.settings();
    const profileBefore = before.profile;

    store.renameProfile("Ada");

    // Both the root and the nested object are new; spreading only the root would have
    // left `profile` pointing at the old one and the change invisible.
    expect(store.settings()).not.toBe(before);
    expect(store.settings().profile).not.toBe(profileBefore);
    expect(profileBefore.name).toBe("Anonymous");
  });

  it("refuses a blank profile name", () => {
    expect(() => store.renameProfile("  ")).toThrow(RangeError);
    expect(store.settings().profile.name).toBe("Anonymous");
  });

  it("recomputes the summary from a nested change", () => {
    expect(store.summary()).toBe("Anonymous · light · 25");

    store.renameProfile("Ada");

    expect(store.summary()).toBe("Ada · light · 25");
  });

  it("resets back to the defaults", () => {
    store.toggleTheme();
    store.renameProfile("Ada");
    expect(store.isDefault()).toBe(false);

    store.reset();

    expect(store.settings()).toEqual(DEFAULT_SETTINGS);
    expect(store.isDefault()).toBe(true);
  });

  it("never holds the shared defaults object itself", () => {
    // Otherwise setThemeByMutating() below would corrupt the constant for every other
    // reader in the process — a mutation bug that outlives the store that caused it.
    expect(store.settings()).not.toBe(DEFAULT_SETTINGS);

    store.setThemeByMutating("dark");

    expect(DEFAULT_SETTINGS.theme).toBe("light");
  });

  it("recomputes only when the reference changes", () => {
    expect(store.summary()).toBe("Anonymous · light · 25");
    const before = store.recomputes;

    store.toggleTheme();

    expect(store.summary()).toBe("Anonymous · dark · 25");
    expect(store.recomputes).toBe(before + 1);
  });

  it("does not notice a mutated object", () => {
    expect(store.summary()).toBe("Anonymous · light · 25");
    const before = store.recomputes;

    store.setThemeByMutating("dark");

    // The value is genuinely there...
    expect(store.settings().theme).toBe("dark");

    // ...and nothing downstream was told, because the reference never moved.
    expect(store.summary()).toBe("Anonymous · light · 25");
    expect(store.recomputes).toBe(before);
  });

  it("surfaces the smuggled value on the next real update", () => {
    // Read first: a computed with nothing cached would just see the mutated object and
    // look correct, so the staleness needs an existing cached value to hide behind.
    expect(store.summary()).toBe("Anonymous · light · 25");

    store.setThemeByMutating("dark");
    expect(store.summary()).toBe("Anonymous · light · 25");

    store.setPageSize(10);

    // "dark" appears now, from a change that had nothing to do with the theme.
    expect(store.summary()).toBe("Anonymous · dark · 10");
  });
});
