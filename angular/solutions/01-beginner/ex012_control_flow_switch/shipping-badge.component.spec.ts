import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Carrier, ShippingBadgeComponent, ShippingState } from "./shipping-badge.component";

describe("ShippingBadgeComponent", () => {
  let fixture: ComponentFixture<ShippingBadgeComponent>;
  let component: ShippingBadgeComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const maybe = (selector: string): Element | null =>
    fixture.nativeElement.querySelector(selector);

  const render = (state: ShippingState, carrier: Carrier = "post"): void => {
    component.state.set(state);
    component.carrier.set(carrier);
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShippingBadgeComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ShippingBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders the pending case first", () => {
    expect(query("p.pending").textContent).toContain("Not shipped yet");
  });

  it("renders exactly one case", () => {
    expect(maybe("p.pending")).not.toBeNull();
    // No fall-through: the following cases are simply not in the DOM.
    expect(maybe("p.transit")).toBeNull();
    expect(maybe("p.delivered")).toBeNull();
    expect(maybe("p.unknown")).toBeNull();
  });

  it("renders the transit case with an estimate", () => {
    render("transit", "post");

    expect(query("p.transit").textContent).toContain("2 days");
    expect(maybe("p.pending")).toBeNull();
  });

  it("renders the delivered case", () => {
    render("delivered");

    expect(query("p.delivered").textContent).toContain("Delivered");
    expect(maybe("p.transit")).toBeNull();
  });

  it("falls back to the default case", () => {
    render("lost");

    expect(query("p.unknown").textContent).toContain("lost");
    expect(maybe("p.pending")).toBeNull();
    expect(maybe("p.delivered")).toBeNull();
  });

  it("swaps cases as the state moves", () => {
    render("transit");
    expect(maybe("p.transit")).not.toBeNull();

    render("delivered");

    expect(maybe("p.transit")).toBeNull();
    expect(maybe("p.delivered")).not.toBeNull();
  });

  it("estimates zero days once delivered", () => {
    render("delivered", "courier");

    expect(component.etaDays()).toBe(0);
  });

  it("estimates per carrier while in transit", () => {
    component.state.set("transit");

    component.carrier.set("post");
    expect(component.etaDays()).toBe(2);

    component.carrier.set("courier");
    expect(component.etaDays()).toBe(1);

    component.carrier.set("pickup");
    expect(component.etaDays()).toBe(0);
  });

  it("refuses to estimate when there is nothing to estimate", () => {
    component.state.set("pending");
    expect(() => component.etaDays()).toThrow(RangeError);

    component.state.set("lost");
    expect(() => component.etaDays()).toThrow(RangeError);
  });

  it("renders the courier estimate in the template", () => {
    render("transit", "courier");

    expect(query("p.transit").textContent).toContain("1 days");
  });

  it("renders the matching carrier", () => {
    render("pending", "post");
    expect(query("span.post").textContent).toContain("Post");

    render("pending", "courier");
    expect(query("span.courier").textContent).toContain("Courier");
    expect(maybe("span.post")).toBeNull();
  });

  it("renders nothing when a switch has no matching case and no default", () => {
    render("pending", "pickup");

    // A @switch with no @default and no match is simply empty — not an error.
    expect(maybe("span.post")).toBeNull();
    expect(maybe("span.courier")).toBeNull();
    // The other switch still rendered, so this is not just an unimplemented template.
    expect(maybe("p.pending")).not.toBeNull();
  });
});
