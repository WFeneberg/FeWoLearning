import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TaggedItem, TaggedListComponent } from "./tagged-list.component";

describe("TaggedListComponent (track correctness and DOM reuse)", () => {
  let fixture: ComponentFixture<TaggedListComponent>;

  const initialItems: readonly TaggedItem[] = [
    { id: 1, label: "one" },
    { id: 2, label: "two" },
    { id: 3, label: "three" },
  ];

  function rowFor(id: number): HTMLLIElement {
    return fixture.nativeElement.querySelector(`li[data-id="${id}"]`) as HTMLLIElement;
  }

  function inputFor(id: number): HTMLInputElement {
    return rowFor(id).querySelector("input") as HTMLInputElement;
  }

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [TaggedListComponent] });
    fixture = TestBed.createComponent(TaggedListComponent);
    fixture.componentRef.setInput("items", initialItems);
    fixture.detectChanges();
  });

  it("renders one <li> per item with the correct ids and labels", () => {
    expect(fixture.nativeElement.querySelectorAll("li").length).toBe(3);
    expect(rowFor(2).textContent).toContain("two");
  });

  it("keeps the same DOM node (and whatever it was holding) for an item that just moved position", () => {
    const nodeBefore = rowFor(2);
    inputFor(2).value = "typed-by-user";

    // same objects, reversed order - a reorder, not a data replacement
    fixture.componentRef.setInput("items", [...initialItems].reverse());
    fixture.detectChanges();

    const nodeAfter = rowFor(2);
    expect(nodeAfter).toBe(nodeBefore);
    expect(inputFor(2).value).toBe("typed-by-user");
  });

  it("keeps the same DOM node when the array is replaced by brand-new objects sharing the same ids", () => {
    const nodeBefore = rowFor(2);
    inputFor(2).value = "typed-by-user";

    // brand new object instances (new references), but same logical entities/ids
    const refreshed: TaggedItem[] = initialItems.map((item) => ({ ...item }));
    fixture.componentRef.setInput("items", refreshed);
    fixture.detectChanges();

    const nodeAfter = rowFor(2);
    expect(nodeAfter).toBe(nodeBefore);
    expect(inputFor(2).value).toBe("typed-by-user");
  });

  it("still creates a fresh node for a genuinely new id, and drops the node for a removed id", () => {
    fixture.componentRef.setInput("items", [
      { id: 1, label: "one" },
      { id: 4, label: "four" },
    ]);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('li[data-id="2"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('li[data-id="3"]')).toBeNull();
    expect(rowFor(4).textContent).toContain("four");
  });
});
