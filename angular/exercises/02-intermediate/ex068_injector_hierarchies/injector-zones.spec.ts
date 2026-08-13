import { ComponentFixture, TestBed } from "@angular/core/testing";
import { By } from "@angular/platform-browser";
import { InnerZoneComponent, LeafComponent, OuterZoneComponent } from "./injector-zones";

describe("injector hierarchies", () => {
  let fixture: ComponentFixture<OuterZoneComponent>;
  let outer: OuterZoneComponent;
  let inner: InnerZoneComponent;
  let leaf: LeafComponent;

  const textOf = (root: Element, selector: string): string => {
    const found = root.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [OuterZoneComponent] }).compileComponents();
    fixture = TestBed.createComponent(OuterZoneComponent);
    outer = fixture.componentInstance;
    fixture.detectChanges();
    inner = fixture.debugElement.query(By.directive(InnerZoneComponent)).componentInstance;
    leaf = fixture.debugElement.query(By.directive(LeafComponent)).componentInstance;
  });

  it("resolves each component's own element-injector provider", () => {
    expect(outer.own()).toBe("[outer]");
    expect(inner.own()).toBe("[inner]");
  });

  it("gives a leaf with no provider of its own the nearest ancestor's value", () => {
    // Inner provides its own "[inner]" — the walk stops there before ever reaching Outer or root.
    expect(leaf.resolved()).toBe("[inner]");
  });

  it("skipSelf reaches past a component's own provider to the next one up", () => {
    expect(inner.ancestor()).toBe("[outer]");
  });

  it("skipSelf from the outermost zone falls all the way to the environment injector", () => {
    expect(outer.ancestor()).toBe("[root]");
  });

  it("optional returns null for a token nobody provides, anywhere", () => {
    expect(leaf.flag()).toBeNull();
  });

  it("renders the resolved values in the template", () => {
    expect(textOf(fixture.nativeElement, ".own")).toBe("[outer]");

    const innerHost = fixture.debugElement.query(By.directive(InnerZoneComponent))
      .nativeElement as Element;
    expect(textOf(innerHost, ".own")).toBe("[inner]");
    expect(textOf(innerHost, ".ancestor")).toBe("[outer]");

    const leafHost = fixture.debugElement.query(By.directive(LeafComponent))
      .nativeElement as Element;
    expect(textOf(leafHost, ".resolved")).toBe("[inner]");
    expect(textOf(leafHost, ".flag")).toBe("none");
  });
});

describe("a leaf with no zone ancestor at all", () => {
  it("falls through every element injector to the environment injector's default", async () => {
    await TestBed.configureTestingModule({ imports: [LeafComponent] }).compileComponents();
    const fixture = TestBed.createComponent(LeafComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.resolved()).toBe("[root]");
  });
});
