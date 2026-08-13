import { ComponentFixture, TestBed } from "@angular/core/testing";
import { VirtualListComponent, computeVirtualWindow } from "./virtual-list.component";

describe("computeVirtualWindow (pure windowing math)", () => {
  it("computes the window for a scroll position at the very top", () => {
    const result = computeVirtualWindow({
      scrollTop: 0,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 50,
      overscan: 0,
    });

    expect(result).toEqual({
      startIndex: 0,
      endIndex: 5,
      topSpacerHeight: 0,
      bottomSpacerHeight: 900,
    });
  });

  it("computes the window somewhere in the middle of the list, with overscan", () => {
    const result = computeVirtualWindow({
      scrollTop: 205,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 50,
      overscan: 1,
    });

    expect(result).toEqual({
      startIndex: 9,
      endIndex: 16,
      topSpacerHeight: 180,
      bottomSpacerHeight: 680,
    });
  });

  it("clamps startIndex to 0 near the top even with overscan requested", () => {
    const result = computeVirtualWindow({
      scrollTop: 0,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 50,
      overscan: 3,
    });

    expect(result.startIndex).toBe(0);
    expect(result.endIndex).toBe(8);
    expect(result.topSpacerHeight).toBe(0);
    expect(result.bottomSpacerHeight).toBe(840);
  });

  it("clamps endIndex to totalItems near the bottom of the list", () => {
    const result = computeVirtualWindow({
      scrollTop: 160,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 10,
      overscan: 2,
    });

    expect(result).toEqual({
      startIndex: 6,
      endIndex: 10,
      topSpacerHeight: 120,
      bottomSpacerHeight: 0,
    });
  });

  it("defaults overscan to 2 when not provided", () => {
    const result = computeVirtualWindow({
      scrollTop: 0,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 50,
    });

    expect(result).toEqual({
      startIndex: 0,
      endIndex: 7,
      topSpacerHeight: 0,
      bottomSpacerHeight: 860,
    });
  });

  it("returns an empty window for an empty list", () => {
    const result = computeVirtualWindow({
      scrollTop: 0,
      viewportHeight: 100,
      itemHeight: 20,
      totalItems: 0,
    });

    expect(result).toEqual({
      startIndex: 0,
      endIndex: 0,
      topSpacerHeight: 0,
      bottomSpacerHeight: 0,
    });
  });
});

describe("VirtualListComponent (wiring the window to a real scroll event)", () => {
  let fixture: ComponentFixture<VirtualListComponent>;
  let component: VirtualListComponent;
  const items = Array.from({ length: 100 }, (_, i) => `item ${i}`);

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [VirtualListComponent] });
    fixture = TestBed.createComponent(VirtualListComponent);
    fixture.componentRef.setInput("items", items);
    fixture.componentRef.setInput("itemHeight", 20);
    fixture.componentRef.setInput("viewportHeight", 100);
    fixture.componentRef.setInput("overscan", 0);
    fixture.detectChanges();
    component = fixture.componentInstance;
  });

  it("starts windowed to the top of the list", () => {
    expect(component.virtualWindow().startIndex).toBe(0);
    expect(component.visibleItems()).toEqual(["item 0", "item 1", "item 2", "item 3", "item 4"]);
  });

  it("advances the window when the viewport is scrolled", () => {
    const viewport = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(".viewport")!;
    viewport.scrollTop = 205;
    viewport.dispatchEvent(new Event("scroll"));
    fixture.detectChanges();

    expect(component.virtualWindow()).toEqual(
      computeVirtualWindow({ scrollTop: 205, viewportHeight: 100, itemHeight: 20, totalItems: 100, overscan: 0 }),
    );
    expect(component.visibleItems()).toEqual(["item 10", "item 11", "item 12", "item 13", "item 14"]);
  });

  it("only renders row elements for the visible window, not the whole list", () => {
    const rows = (fixture.nativeElement as HTMLElement).querySelectorAll(".row");

    expect(rows.length).toBe(5);
  });

  it("reflects the spacer heights from the window in the rendered spacer divs", () => {
    const viewport = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(".viewport")!;
    viewport.scrollTop = 205;
    viewport.dispatchEvent(new Event("scroll"));
    fixture.detectChanges();

    const topSpacer = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(".top-spacer")!;
    const bottomSpacer = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(".bottom-spacer")!;

    expect(topSpacer.style.height).toBe(`${component.virtualWindow().topSpacerHeight}px`);
    expect(bottomSpacer.style.height).toBe(`${component.virtualWindow().bottomSpacerHeight}px`);
  });
});
