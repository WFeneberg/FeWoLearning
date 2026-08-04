import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ActivatedRoute, convertToParamMap } from "@angular/router";
import { ProductDetailComponent } from "./product-detail.component";

/**
 * A stand-in ActivatedRoute carrying just a snapshot.
 *
 * convertToParamMap is the router's own helper, so the ParamMap behaves exactly as a real one —
 * including getAll for a repeated key.
 */
const routeWith = (
  params: Record<string, string>,
  queryParams: Record<string, string | string[]> = {},
): unknown => ({
  snapshot: {
    paramMap: convertToParamMap(params),
    queryParamMap: convertToParamMap(queryParams),
  },
});

describe("ProductDetailComponent", () => {
  const build = async (
    params: Record<string, string>,
    queryParams: Record<string, string | string[]> = {},
  ): Promise<ComponentFixture<ProductDetailComponent>> => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [ProductDetailComponent],
      providers: [{ provide: ActivatedRoute, useValue: routeWith(params, queryParams) }],
    }).compileComponents();
    const fixture = TestBed.createComponent(ProductDetailComponent);
    fixture.detectChanges();
    return fixture;
  };

  const text = (fixture: ComponentFixture<unknown>, selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  it("reads the id parameter", async () => {
    const fixture = await build({ id: "42" });

    expect(fixture.componentInstance.productId()).toBe(42);
  });

  it("returns the raw value as a string", async () => {
    const fixture = await build({ id: "42" });

    // Everything in a paramMap is a string, whatever the URL looked like.
    expect(fixture.componentInstance.rawId()).toBe("42");
    expect(typeof fixture.componentInstance.rawId()).toBe("string");
  });

  it("returns null rather than undefined for a missing parameter", async () => {
    const fixture = await build({});

    expect(fixture.componentInstance.rawId()).toBeNull();
  });

  it("refuses a missing id rather than reporting NaN", async () => {
    const fixture = await build({});

    expect(() => fixture.componentInstance.productId()).toThrow(RangeError);
  });

  it("refuses an unparseable id", async () => {
    const fixture = await build({ id: "banana" });

    expect(() => fixture.componentInstance.productId()).toThrow(RangeError);
  });

  it("renders the id", async () => {
    const fixture = await build({ id: "7" });

    expect(text(fixture, "p.id")).toBe("7");
  });

  it("titles a known product", async () => {
    const fixture = await build({ id: "7" });

    expect(fixture.componentInstance.title()).toBe("Product 7");
    expect(text(fixture, "h2.title")).toBe("Product 7");
  });

  it("titles an unusable id without throwing", async () => {
    const fixture = await build({ id: "banana" });

    // The template must render, so title() has to absorb what productId() rejects.
    expect(fixture.componentInstance.title()).toBe("Unknown product");
    expect(text(fixture, "h2.title")).toBe("Unknown product");
  });

  it("reads a query parameter", async () => {
    const fixture = await build({ id: "1" }, { page: "3" });

    expect(fixture.componentInstance.page()).toBe(3);
    expect(text(fixture, "p.page")).toBe("3");
  });

  it("defaults the page when absent", async () => {
    const fixture = await build({ id: "1" });

    expect(fixture.componentInstance.page()).toBe(1);
  });

  it("defaults the page when unparseable", async () => {
    const fixture = await build({ id: "1" }, { page: "later" });

    expect(fixture.componentInstance.page()).toBe(1);
  });

  it("keeps every value of a repeated query parameter", async () => {
    const fixture = await build({ id: "1" }, { tag: ["a", "b", "c"] });

    // `get` would have returned only "a" and silently dropped the rest.
    expect(fixture.componentInstance.tags()).toEqual(["a", "b", "c"]);
    expect(text(fixture, "p.tags")).toBe("a,b,c");
  });

  it("handles a single tag", async () => {
    const fixture = await build({ id: "1" }, { tag: "only" });

    expect(fixture.componentInstance.tags()).toEqual(["only"]);
  });

  it("handles no tags", async () => {
    const fixture = await build({ id: "1" });

    expect(fixture.componentInstance.tags()).toEqual([]);
  });

  it("reports presence separately from value", async () => {
    const fixture = await build({ id: "1" }, { q: "" });

    // Present but empty is still present.
    expect(fixture.componentInstance.hasQueryParam("q")).toBe(true);
    expect(fixture.componentInstance.hasQueryParam("missing")).toBe(false);
  });

  it("reads everything from one snapshot", async () => {
    const fixture = await build({ id: "9" }, { page: "2", tag: ["x", "y"] });

    expect(text(fixture, "p.id")).toBe("9");
    expect(text(fixture, "p.page")).toBe("2");
    expect(text(fixture, "p.tags")).toBe("x,y");
  });
});
