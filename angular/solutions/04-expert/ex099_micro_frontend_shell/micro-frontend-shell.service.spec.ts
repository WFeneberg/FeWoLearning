import { TestBed } from "@angular/core/testing";
import { MicroFrontendShellService } from "./micro-frontend-shell.service";

describe("MicroFrontendShellService (mount/unmount isolation and teardown)", () => {
  let shell: MicroFrontendShellService;
  let containerA: HTMLDivElement;
  let containerB: HTMLDivElement;

  const click = (el: HTMLElement) => el.dispatchEvent(new MouseEvent("click", { bubbles: true }));

  beforeEach(() => {
    TestBed.configureTestingModule({});
    shell = TestBed.inject(MicroFrontendShellService);
    containerA = document.createElement("div");
    containerB = document.createElement("div");
  });

  it("gives two mounted apps their own independent CounterStore instances", () => {
    const storeA = shell.mount("a", containerA);
    const storeB = shell.mount("b", containerB);

    expect(storeA).not.toBe(storeB);

    storeA.increment();
    storeA.increment();

    expect(storeA.count()).toBe(2);
    expect(storeB.count()).toBe(0); // sibling untouched
  });

  it("tracks mounted ids in mount order", () => {
    shell.mount("a", containerA);
    shell.mount("b", containerB);

    expect(shell.mountedIds()).toEqual(["a", "b"]);
  });

  it("wires each mount's container so clicking it increments only that mount's store", () => {
    const storeA = shell.mount("a", containerA);
    const storeB = shell.mount("b", containerB);

    click(containerA);
    click(containerA);
    click(containerB);

    expect(storeA.count()).toBe(2);
    expect(storeB.count()).toBe(1);
  });

  it("throws a RangeError when mounting an id that is already mounted", () => {
    shell.mount("a", containerA);

    expect(() => shell.mount("a", containerA)).toThrow(RangeError);
  });

  it("throws a RangeError when unmounting an id that was never mounted", () => {
    expect(() => shell.unmount("missing")).toThrow(RangeError);
  });

  it("unmount destroys that mount's store (ngOnDestroy) and removes its click listener", () => {
    const storeA = shell.mount("a", containerA);

    shell.unmount("a");

    expect(storeA.destroyed()).toBe(true);

    click(containerA); // listener should be gone — must not silently keep incrementing
    expect(storeA.count()).toBe(0);
  });

  it("unmounting one app does not affect a sibling that stays mounted", () => {
    const storeA = shell.mount("a", containerA);
    const storeB = shell.mount("b", containerB);

    shell.unmount("a");

    expect(storeB.destroyed()).toBe(false);
    click(containerB);
    expect(storeB.count()).toBe(1);
    expect(shell.mountedIds()).toEqual(["b"]);
  });

  it("storeFor returns null once an id is unmounted, and the live store while mounted", () => {
    const storeA = shell.mount("a", containerA);

    expect(shell.storeFor("a")).toBe(storeA);

    shell.unmount("a");

    expect(shell.storeFor("a")).toBeNull();
  });

  it("allows re-mounting the same id again after it was unmounted, with a fresh store", () => {
    const firstStore = shell.mount("a", containerA);
    firstStore.increment();
    shell.unmount("a");

    const secondStore = shell.mount("a", containerA);

    expect(secondStore).not.toBe(firstStore);
    expect(secondStore.count()).toBe(0);
  });
});
