import { ComponentFixture, TestBed } from "@angular/core/testing";
import { HighlightableDirective, PanelComponent } from "./panel.component";

describe("PanelComponent (hostDirectives composition)", () => {
  let fixture: ComponentFixture<PanelComponent>;
  let panelEl: HTMLElement;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [PanelComponent] });
    fixture = TestBed.createComponent(PanelComponent);
    fixture.detectChanges();
    panelEl = fixture.nativeElement as HTMLElement;
  });

  it("composes HighlightableDirective onto the panel's own host element", () => {
    expect(() => fixture.debugElement.injector.get(HighlightableDirective)).not.toThrow();
  });

  it("highlights the host element on hover via the composed directive", () => {
    expect(panelEl.classList.contains("highlighted")).toBe(false);

    panelEl.dispatchEvent(new MouseEvent("mouseenter"));
    fixture.detectChanges();

    expect(panelEl.classList.contains("highlighted")).toBe(true);
  });

  it("stops highlighting once the pointer leaves, via the same composed directive", () => {
    panelEl.dispatchEvent(new MouseEvent("mouseenter"));
    fixture.detectChanges();
    expect(panelEl.classList.contains("highlighted")).toBe(true);

    panelEl.dispatchEvent(new MouseEvent("mouseleave"));
    fixture.detectChanges();

    expect(panelEl.classList.contains("highlighted")).toBe(false);
  });

  it("toggling the header delegates to the composed ExpandableDirective, expanding the host", () => {
    expect(panelEl.classList.contains("expanded")).toBe(false);

    panelEl.querySelector<HTMLButtonElement>(".header")!.click();
    fixture.detectChanges();

    expect(panelEl.classList.contains("expanded")).toBe(true);
  });

  it("toggles back to collapsed on a second click, through the same delegation", () => {
    const header = panelEl.querySelector<HTMLButtonElement>(".header")!;

    header.click();
    fixture.detectChanges();
    expect(panelEl.classList.contains("expanded")).toBe(true);

    header.click();
    fixture.detectChanges();

    expect(panelEl.classList.contains("expanded")).toBe(false);
  });
});
