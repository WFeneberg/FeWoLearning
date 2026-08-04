import { ComponentFixture, TestBed } from "@angular/core/testing";
import { StatusPanelComponent } from "./status-panel.component";

describe("StatusPanelComponent", () => {
  let fixture: ComponentFixture<StatusPanelComponent>;
  let component: StatusPanelComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  /** Unlike `query`, this is allowed to come back empty — absence is the assertion. */
  const maybe = (selector: string): Element | null =>
    fixture.nativeElement.querySelector(selector);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusPanelComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(StatusPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("shows the loading branch first", () => {
    expect(query("p.loading").textContent).toContain("Loading");
  });

  it("keeps the other branches out of the DOM entirely", () => {
    // The matching branch is rendered...
    expect(maybe("p.loading")).not.toBeNull();

    // ...and @if removes the rest. It does not hide them — there is nothing to find.
    expect(maybe("p.error")).toBeNull();
    expect(maybe("p.empty")).toBeNull();
    expect(maybe("p.ready")).toBeNull();
  });

  it("shows the error branch with its message", () => {
    component.status.set("error");
    component.message.set("Request failed");
    fixture.detectChanges();

    expect(query("p.error").textContent).toContain("Request failed");
    expect(maybe("p.loading")).toBeNull();
  });

  it("reports emptiness when ready with no items", () => {
    component.status.set("ready");

    expect(component.isEmpty()).toBe(true);
  });

  it("is not empty while still loading", () => {
    expect(component.isEmpty()).toBe(false);
  });

  it("is not empty once there are items", () => {
    component.status.set("ready");
    component.count.set(3);

    expect(component.isEmpty()).toBe(false);
  });

  it("shows the empty branch", () => {
    component.status.set("ready");
    fixture.detectChanges();

    expect(query("p.empty").textContent).toContain("Nothing here");
    expect(maybe("p.ready")).toBeNull();
  });

  it("falls through to the ready branch", () => {
    component.status.set("ready");
    component.count.set(3);
    fixture.detectChanges();

    expect(query("p.ready").textContent).toContain("3");
    expect(maybe("p.empty")).toBeNull();
    expect(maybe("p.loading")).toBeNull();
  });

  it("swaps branches when the state changes", () => {
    component.status.set("error");
    fixture.detectChanges();
    expect(maybe("p.error")).not.toBeNull();

    component.status.set("ready");
    component.count.set(1);
    fixture.detectChanges();

    expect(maybe("p.error")).toBeNull();
    expect(maybe("p.ready")).not.toBeNull();
  });

  it("shows the anonymous branch with no profile", () => {
    expect(query("p.anonymous").textContent).toContain("Signed out");
    expect(maybe("p.profile")).toBeNull();
  });

  it("binds the aliased profile", () => {
    component.profile.set({ name: "Ada", email: "ada@example.com" });
    fixture.detectChanges();

    const text = query("p.profile").textContent ?? "";
    expect(text).toContain("Ada");
    expect(text).toContain("ada@example.com");
    expect(maybe("p.anonymous")).toBeNull();
  });

  it("goes back to anonymous when the profile is cleared", () => {
    component.profile.set({ name: "Ada", email: "ada@example.com" });
    fixture.detectChanges();

    component.profile.set(null);
    fixture.detectChanges();

    expect(maybe("p.profile")).toBeNull();
    expect(maybe("p.anonymous")).not.toBeNull();
  });
});
