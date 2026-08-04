import { ComponentFixture, TestBed } from "@angular/core/testing";
import { BadgeComponent, PanelComponent } from "./panel.component";

describe("PanelComponent", () => {
  let fixture: ComponentFixture<PanelComponent>;
  let component: PanelComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PanelComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(PanelComponent);
    component = fixture.componentInstance;
  });

  it("resolves without waiting for change detection", () => {
    // No detectChanges() yet. A signal query is lazy but the view already exists, so
    // unlike the old @ViewChild decorator there is nothing to wait for.
    expect(component.nameBox()).toBeDefined();
    expect(component.badges()).toHaveLength(2);
  });

  it("comes back undefined for something that is not rendered", () => {
    fixture.detectChanges();

    // The sibling query does find its element...
    expect(component.nameBox()).toBeDefined();

    // ...so this undefined is about the target sitting inside a false @if, rather than
    // about queries not working at all.
    expect(component.extraBox()).toBeUndefined();
  });

  it("finds it once it is rendered", () => {
    component.expanded.set(true);
    fixture.detectChanges();

    expect(component.extraBox()).toBeDefined();
    expect(component.extraBox()?.nativeElement).toBe(query("input.extra"));
  });

  it("loses it again when it is removed", () => {
    component.expanded.set(true);
    fixture.detectChanges();
    expect(component.extraBox()).toBeDefined();

    component.expanded.set(false);
    fixture.detectChanges();

    expect(component.extraBox()).toBeUndefined();
  });

  it("hands back the very element the template rendered", () => {
    fixture.detectChanges();

    expect(component.nameBox()?.nativeElement).toBe(query("input.name"));
  });

  it("wraps the DOM node in an ElementRef", () => {
    fixture.detectChanges();

    // Not the node itself: `.nativeElement` is the unwrap step people forget.
    expect(component.nameBox()?.nativeElement.tagName).toBe("INPUT");
  });

  it("reads through the query", () => {
    fixture.detectChanges();
    query<HTMLInputElement>("input.name").value = "Ada";

    expect(component.typedLength()).toBe(3);
  });

  it("reports zero for an empty input", () => {
    expect(component.typedLength()).toBe(0);

    fixture.detectChanges();
    query<HTMLInputElement>("input.name").value = "ab";

    expect(component.typedLength()).toBe(2);
  });

  it("renders the length through the query", () => {
    fixture.detectChanges();
    query<HTMLInputElement>("input.name").value = "Grace";
    fixture.detectChanges();

    expect(query("p.typed").textContent?.trim()).toBe("5");
  });

  it("finds the first child component instance", () => {
    fixture.detectChanges();

    expect(component.badge()).toBeInstanceOf(BadgeComponent);
  });

  it("hands back the instance, not the element", () => {
    fixture.detectChanges();

    // A component query gives you the class, so its methods are available.
    expect(component.shoutFirst()).toBe("BADGE");
  });

  it("finds every match with viewChildren", () => {
    fixture.detectChanges();

    expect(component.badges()).toHaveLength(2);
    expect(component.badgeCount()).toBe(2);
  });

  it("renders the badge count", () => {
    fixture.detectChanges();

    expect(query("p.count").textContent?.trim()).toBe("2");
  });

  it("returns the first of several for a single-result query", () => {
    fixture.detectChanges();

    expect(component.badge()).toBeInstanceOf(BadgeComponent);
    expect(component.badge()).toBe(component.badges()[0]);
    expect(component.badge()).not.toBe(component.badges()[1]);
  });

  it("drives every child through the query", () => {
    fixture.detectChanges();

    component.labelAll("tag");
    fixture.detectChanges();

    const rendered = Array.from(
      fixture.nativeElement.querySelectorAll("app-badge span.text") as NodeListOf<HTMLElement>,
    ).map((node) => node.textContent?.trim());

    expect(rendered).toEqual(["tag 1", "tag 2"]);
  });

  it("shouts the relabelled first badge", () => {
    fixture.detectChanges();
    component.labelAll("tag");

    expect(component.shoutFirst()).toBe("TAG 1");
  });
});
