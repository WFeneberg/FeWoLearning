import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ProductBadgeComponent } from "./product-badge.component";

describe("ProductBadgeComponent", () => {
  let fixture: ComponentFixture<ProductBadgeComponent>;
  let component: ProductBadgeComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductBadgeComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ProductBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("interpolates the label", () => {
    expect(query("h2.label").textContent?.trim()).toBe("Widget");
  });

  it("re-renders the label when it changes", () => {
    component.label = "Gadget";
    fixture.detectChanges();

    expect(query("h2.label").textContent?.trim()).toBe("Gadget");
  });

  it("builds the stock label from the count", () => {
    expect(component.stockLabel()).toBe("3 left");
  });

  it("uses the singular form for one item", () => {
    component.stock = 1;

    expect(component.stockLabel()).toBe("1 left");
  });

  it("reports sold out regardless of the count", () => {
    component.soldOut = true;

    expect(component.stockLabel()).toBe("Sold out");
  });

  it("interpolates the stock label into the template", () => {
    expect(query("p.stock").textContent?.trim()).toBe("3 left");

    component.soldOut = true;
    fixture.detectChanges();

    expect(query("p.stock").textContent?.trim()).toBe("Sold out");
  });

  it("binds disabled as a DOM property", () => {
    const button = query<HTMLButtonElement>("button.buy");
    expect(button.disabled).toBe(false);

    component.soldOut = true;
    fixture.detectChanges();

    // The property is what disables the button — not a "disabled" string somewhere.
    expect(button.disabled).toBe(true);
  });

  it("binds href on the details link", () => {
    expect(query("a.details").getAttribute("href")).toBe("/products/widget");
  });

  it("binds data-tone as an attribute", () => {
    expect(query("button.buy").getAttribute("data-tone")).toBe("info");
  });

  it("updates an attribute binding when the value changes", () => {
    component.tone = "warning";
    fixture.detectChanges();

    expect(query("button.buy").getAttribute("data-tone")).toBe("warning");
  });

  it("renders the badge attribute when set", () => {
    expect(query("a.details").getAttribute("data-badge")).toBe("new");
  });

  it("removes the attribute entirely when the binding is null", () => {
    component.badge = null;
    fixture.detectChanges();

    const link = query("a.details");
    // Not the string "null", and not an empty attribute — gone.
    expect(link.hasAttribute("data-badge")).toBe(false);
  });
});
