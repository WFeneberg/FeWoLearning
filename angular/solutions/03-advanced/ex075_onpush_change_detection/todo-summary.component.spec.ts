import { Component, signal } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Todo, TodoSummaryComponent } from "./todo-summary.component";

@Component({
  standalone: true,
  imports: [TodoSummaryComponent],
  template: `<app-todo-summary [items]="items()" />`,
})
class HostComponent {
  readonly items = signal<Todo[]>([{ id: 1, text: "a", done: false }]);
}

describe("TodoSummaryComponent (OnPush)", () => {
  let fixture: ComponentFixture<HostComponent>;
  let host: HostComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HostComponent] });
    fixture = TestBed.createComponent(HostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const countText = () =>
    (fixture.nativeElement.querySelector(".count") as HTMLElement).textContent;

  it("renders the initial summary", () => {
    expect(countText()).toBe("1 items, 0 done");
  });

  it("does not reflect an in-place mutation of the same array reference", () => {
    host.items().push({ id: 2, text: "b", done: true });
    fixture.detectChanges();

    expect(countText()).toBe("1 items, 0 done");
  });

  it("reflects a new array reference", () => {
    host.items.set([...host.items(), { id: 2, text: "b", done: true }]);
    fixture.detectChanges();

    expect(countText()).toBe("2 items, 1 done");
  });

  it("still reacts to its own internal signal writes (a click) under OnPush", () => {
    const state = () =>
      (fixture.nativeElement.querySelector(".collapsed-state") as HTMLElement).textContent;
    expect(state()).toBe("expanded");

    (fixture.nativeElement.querySelector("button.toggle-collapsed") as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(state()).toBe("collapsed");
  });
});
