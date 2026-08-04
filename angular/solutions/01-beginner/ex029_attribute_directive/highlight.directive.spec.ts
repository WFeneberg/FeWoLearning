import { ComponentFixture, TestBed } from "@angular/core/testing";
import { By } from "@angular/platform-browser";
import { HighlightDirective, HighlightHostComponent } from "./highlight.directive";

describe("HighlightDirective", () => {
  let fixture: ComponentFixture<HighlightHostComponent>;

  const element = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}"`);
    }
    return found;
  };

  /** The directive instance attached to a given element. */
  const directiveOn = (selector: string): HighlightDirective => {
    const node = fixture.debugElement
      .queryAll(By.directive(HighlightDirective))
      .find((candidate) => candidate.nativeElement === element(selector));
    if (node === undefined) {
      throw new Error(`no HighlightDirective attached to "${selector}"`);
    }
    return node.injector.get(HighlightDirective);
  };

  const hover = (selector: string, type: "mouseenter" | "mouseleave"): void => {
    element(selector).dispatchEvent(new MouseEvent(type));
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HighlightHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(HighlightHostComponent);
    fixture.detectChanges();
  });

  it("attaches to every element carrying the attribute", () => {
    expect(fixture.debugElement.queryAll(By.directive(HighlightDirective))).toHaveLength(3);

    // ...and each instance really did receive its own host node.
    expect(directiveOn("p.plain").hostTag()).toBe("P");
    expect(directiveOn("button.btn").hostTag()).toBe("BUTTON");
  });

  it("keeps its default when the attribute has no value, and takes one when given", () => {
    // A bare `appHighlight` arrives as the empty string, not as "no value bound", so a
    // transform is what turns it back into the fallback colour...
    expect(directiveOn("p.plain").appHighlight()).toBe("yellow");

    // ...while `appHighlight="lime"` both applies it and passes a value.
    expect(directiveOn("p.lime").appHighlight()).toBe("lime");
    expect(directiveOn("button.btn").appHighlight()).toBe("pink");
  });

  it("paints nothing until the pointer arrives", () => {
    expect(element("p.plain").style.backgroundColor).toBe("");

    // The listener does work, so the blank above is "not yet" rather than "never".
    hover("p.plain", "mouseenter");
    expect(element("p.plain").style.backgroundColor).toBe("yellow");
  });

  it("paints the host element on enter", () => {
    hover("p.plain", "mouseenter");

    expect(element("p.plain").style.backgroundColor).toBe("yellow");
  });

  it("uses the colour it was given", () => {
    hover("p.lime", "mouseenter");

    expect(element("p.lime").style.backgroundColor).toBe("lime");
  });

  it("clears the paint on leave", () => {
    hover("p.plain", "mouseenter");
    expect(element("p.plain").style.backgroundColor).toBe("yellow");

    hover("p.plain", "mouseleave");

    expect(element("p.plain").style.backgroundColor).toBe("");
  });

  it("counts entries but not exits", () => {
    hover("p.plain", "mouseenter");
    hover("p.plain", "mouseleave");
    hover("p.plain", "mouseenter");

    expect(directiveOn("p.plain").entries()).toBe(2);
  });

  it("keeps each attachment independent", () => {
    hover("p.lime", "mouseenter");

    expect(element("p.lime").style.backgroundColor).toBe("lime");
    // One directive class, three separate instances with their own host and state.
    expect(element("p.plain").style.backgroundColor).toBe("");
    expect(directiveOn("p.plain").entries()).toBe(0);
    expect(directiveOn("p.lime").entries()).toBe(1);
  });

  it("works on any element, not just a paragraph", () => {
    hover("button.btn", "mouseenter");

    expect(element("button.btn").style.backgroundColor).toBe("pink");
    expect(directiveOn("button.btn").hostTag()).toBe("BUTTON");
  });

  it("reports the host element it was attached to", () => {
    expect(directiveOn("p.plain").hostTag()).toBe("P");
    expect(directiveOn("button.btn").hostTag()).toBe("BUTTON");
  });
});
