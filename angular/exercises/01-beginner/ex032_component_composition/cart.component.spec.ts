import { ComponentFixture, TestBed } from "@angular/core/testing";
import { By } from "@angular/platform-browser";
import {
  CartComponent,
  CartRowComponent,
  PriceTagComponent,
} from "./cart.component";

describe("PriceTagComponent", () => {
  let fixture: ComponentFixture<PriceTagComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceTagComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(PriceTagComponent);
    fixture.componentRef.setInput("amount", 12.5);
    fixture.detectChanges();
  });

  const price = (): string =>
    (fixture.nativeElement.querySelector("span.price") as HTMLElement | null)?.textContent?.trim() ??
    "MISSING span.price";

  it("formats what it was given", () => {
    expect(price()).toBe("12.50 EUR");
  });

  it("pads to two decimals", () => {
    fixture.componentRef.setInput("amount", 4);
    fixture.detectChanges();

    expect(price()).toBe("4.00 EUR");
  });

  it("takes the currency from its input", () => {
    fixture.componentRef.setInput("currency", "USD");
    fixture.detectChanges();

    expect(price()).toBe("12.50 USD");
  });

  it("knows nothing about the cart", () => {
    // A plain number in, a string out — reusable for a line, a subtotal or a tax row.
    fixture.componentRef.setInput("amount", 999.999);
    fixture.detectChanges();

    expect(price()).toBe("1000.00 EUR");
  });
});

describe("CartRowComponent", () => {
  let fixture: ComponentFixture<CartRowComponent>;
  let component: CartRowComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartRowComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(CartRowComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("line", { sku: "pen", qty: 3, unitPrice: 2 });
    fixture.detectChanges();
  });

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  it("renders the line it was handed", () => {
    expect(text("span.sku")).toBe("pen");
    expect(text("span.qty")).toBe("×3");
  });

  it("computes the line total", () => {
    expect(component.lineTotal()).toBe(6);
  });

  it("passes the total down to its own child", () => {
    // The row does not format anything itself — it delegates to the leaf.
    expect(text("app-price-tag span.price")).toBe("6.00 EUR");
  });

  it("forwards the currency down another level", () => {
    fixture.componentRef.setInput("currency", "GBP");
    fixture.detectChanges();

    expect(text("app-price-tag span.price")).toBe("6.00 GBP");
  });

  it("really does nest the leaf component", () => {
    expect(fixture.debugElement.queryAll(By.directive(PriceTagComponent))).toHaveLength(1);
  });
});

describe("CartComponent", () => {
  let fixture: ComponentFixture<CartComponent>;
  let component: CartComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  const rows = (): HTMLElement[] =>
    Array.from(fixture.nativeElement.querySelectorAll("app-cart-row") as NodeListOf<HTMLElement>);

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  it("renders one row per line", () => {
    expect(rows()).toHaveLength(2);
    expect(text("p.count")).toBe("2 lines");
  });

  it("builds the whole three-level tree", () => {
    // Two rows, each with a price tag, plus the total's own price tag.
    expect(fixture.debugElement.queryAll(By.directive(CartRowComponent))).toHaveLength(2);
    expect(fixture.debugElement.queryAll(By.directive(PriceTagComponent))).toHaveLength(3);
  });

  it("passes each line down to its row", () => {
    const skus = rows().map((row) => row.querySelector("span.sku")?.textContent?.trim());

    expect(skus).toEqual(["pen", "pad"]);
  });

  it("reaches the leaf two levels down", () => {
    const prices = rows().map((row) =>
      row.querySelector("app-price-tag span.price")?.textContent?.trim(),
    );

    expect(prices).toEqual(["6.00 EUR", "4.50 EUR"]);
  });

  it("totals every line", () => {
    expect(component.total()).toBe(10.5);
    expect(text("p.total")).toContain("10.50 EUR");
  });

  it("pushes a currency change all the way down", () => {
    component.currency.set("USD");
    fixture.detectChanges();

    expect(text("p.total")).toContain("10.50 USD");
    expect(rows()[0].querySelector("app-price-tag span.price")?.textContent?.trim()).toBe(
      "6.00 USD",
    );
  });

  it("removes a line when a row asks", () => {
    component.remove("pen");
    fixture.detectChanges();

    expect(rows()).toHaveLength(1);
    expect(component.total()).toBe(4.5);
  });

  it("ignores an unknown sku", () => {
    component.remove("nope");

    expect(component.lines()).toHaveLength(2);
  });

  it("routes a click in a grandchild row up to the owner", () => {
    const removeButton = rows()[0].querySelector("button.remove") as HTMLButtonElement;
    removeButton.click();
    fixture.detectChanges();

    // The row emitted, the cart acted — the row never touched `lines` itself.
    expect(component.lines().map((line) => line.sku)).toEqual(["pad"]);
    expect(rows()).toHaveLength(1);
    expect(text("p.count")).toBe("1 lines");
  });

  it("keeps working after the tree shrinks", () => {
    (rows()[0].querySelector("button.remove") as HTMLButtonElement).click();
    fixture.detectChanges();
    (rows()[0].querySelector("button.remove") as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(component.lines()).toEqual([]);
    expect(rows()).toHaveLength(0);
    expect(text("p.total")).toContain("0.00 EUR");
  });
});
