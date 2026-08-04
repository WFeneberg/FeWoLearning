import { ComponentFixture, TestBed } from "@angular/core/testing";
import { CartLine, CartTotalsComponent } from "./cart-totals.component";

const PEN: CartLine = { name: "Pen", price: 2, qty: 3 };
const PAD: CartLine = { name: "Pad", price: 4, qty: 1 };

describe("CartTotalsComponent", () => {
  let fixture: ComponentFixture<CartTotalsComponent>;
  let component: CartTotalsComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartTotalsComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(CartTotalsComponent);
    component = fixture.componentInstance;
    // Deliberately no detectChanges() here: rendering would read the computeds, and the
    // laziness tests below need to observe a component nobody has read yet.
  });

  it("starts empty with zero totals", () => {
    expect(component.isEmpty()).toBe(true);
    expect(component.itemCount()).toBe(0);
    expect(component.subtotal()).toBe(0);
    expect(component.total()).toBe(0);
  });

  it("sums price times quantity", () => {
    component.lines.set([PEN, PAD]);

    expect(component.subtotal()).toBe(10);
  });

  it("counts quantities, not lines", () => {
    component.lines.set([PEN, PAD]);

    expect(component.itemCount()).toBe(4);
  });

  it("applies the tax rate to the subtotal", () => {
    component.lines.set([PEN, PAD]);

    expect(component.tax()).toBeCloseTo(2);
    expect(component.total()).toBeCloseTo(12);
  });

  it("follows a change to the tax rate", () => {
    component.lines.set([PEN, PAD]);
    component.taxRate.set(0.5);

    expect(component.tax()).toBeCloseTo(5);
    expect(component.total()).toBeCloseTo(15);
  });

  it("is no longer empty once a line is added", () => {
    component.addLine(PEN);

    expect(component.isEmpty()).toBe(false);
    expect(component.itemCount()).toBe(3);
  });

  it("appends without mutating the previous array", () => {
    const original = component.lines();
    component.addLine(PEN);

    expect(original).toHaveLength(0);
    expect(component.lines()).toHaveLength(1);
    expect(component.lines()).not.toBe(original);
  });

  it("recomputes after every append", () => {
    component.addLine(PEN);
    component.addLine(PAD);

    expect(component.subtotal()).toBe(10);
  });

  it("does not evaluate until something reads it", () => {
    component.lines.set([PEN, PAD]);

    // A computed is lazy: nothing has asked for the subtotal, so nothing ran.
    expect(component.subtotalEvaluations).toBe(0);

    // Reading it is what runs the body — exactly once.
    expect(component.subtotal()).toBe(10);
    expect(component.subtotalEvaluations).toBe(1);
  });

  it("memoises repeated reads", () => {
    component.lines.set([PEN, PAD]);

    expect(component.subtotal()).toBe(10);
    expect(component.subtotal()).toBe(10);
    expect(component.subtotal()).toBe(10);

    expect(component.subtotalEvaluations).toBe(1);
  });

  it("re-evaluates when its own dependency changes", () => {
    component.lines.set([PEN]);
    void component.subtotal();
    component.lines.set([PEN, PAD]);
    void component.subtotal();

    expect(component.subtotalEvaluations).toBe(2);
  });

  it("does not re-evaluate when an unrelated signal changes", () => {
    component.lines.set([PEN, PAD]);
    expect(component.total()).toBeCloseTo(12);
    expect(component.subtotalEvaluations).toBe(1);

    component.taxRate.set(0.5);

    // `total` must recompute, but `subtotal` never read taxRate, so it stays cached.
    expect(component.total()).toBeCloseTo(15);
    expect(component.subtotalEvaluations).toBe(1);
  });

  it("renders every total", () => {
    component.lines.set([PEN, PAD]);
    fixture.detectChanges();

    expect(query("p.count").textContent).toContain("4");
    expect(query("p.subtotal").textContent).toContain("10");
    expect(query("p.tax").textContent).toContain("2");
    expect(query("p.total").textContent).toContain("12");
  });

  it("re-renders when a line is added", () => {
    fixture.detectChanges();
    expect(query("p.total").textContent).toContain("0");

    component.addLine(PAD);
    fixture.detectChanges();

    expect(query("p.total").textContent).toContain("4.8");
  });
});
