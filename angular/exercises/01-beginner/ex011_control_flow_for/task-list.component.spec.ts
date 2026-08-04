import { ComponentFixture, TestBed } from "@angular/core/testing";
import { Task, TaskListComponent } from "./task-list.component";

const task = (id: number, title: string, done = false): Task => ({ id, title, done });

const WRITE = task(1, "Write");
const REVIEW = task(2, "Review", true);
const SHIP = task(3, "Ship");

describe("TaskListComponent", () => {
  let fixture: ComponentFixture<TaskListComponent>;
  let component: TaskListComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const maybe = (selector: string): Element | null =>
    fixture.nativeElement.querySelector(selector);

  const rows = (): HTMLLIElement[] =>
    Array.from(fixture.nativeElement.querySelectorAll("li.task")) as HTMLLIElement[];

  const texts = (): string[] => rows().map((row) => row.textContent?.trim() ?? "");

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(TaskListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders the empty block when there is nothing to show", () => {
    expect(query("li.empty").textContent).toContain("No tasks");
    expect(rows()).toHaveLength(0);
  });

  it("renders one row per task", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    expect(rows()).toHaveLength(3);
    expect(maybe("li.empty")).toBeNull();
  });

  it("exposes $index and $count", () => {
    component.tasks.set([WRITE, REVIEW]);
    fixture.detectChanges();

    expect(texts()).toEqual(["0: Write (2)", "1: Review (2)"]);
  });

  it("marks the first and last rows", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    const [first, middle, last] = rows();

    expect(first.classList.contains("first")).toBe(true);
    expect(first.classList.contains("last")).toBe(false);
    expect(middle.classList.contains("first")).toBe(false);
    expect(middle.classList.contains("last")).toBe(false);
    expect(last.classList.contains("last")).toBe(true);
  });

  it("marks a single row as both first and last", () => {
    component.tasks.set([WRITE]);
    fixture.detectChanges();

    const [only] = rows();

    expect(only.classList.contains("first")).toBe(true);
    expect(only.classList.contains("last")).toBe(true);
  });

  it("exposes $even", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    expect(rows().map((row) => row.classList.contains("even"))).toEqual([true, false, true]);
  });

  it("counts the done tasks", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);

    expect(component.doneCount()).toBe(1);
  });

  it("renders the summary", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    expect(query("p.summary").textContent).toContain("1 of 3 done");
  });

  it("prepends without mutating the previous array", () => {
    component.tasks.set([REVIEW]);
    const original = component.tasks();

    component.prepend(WRITE);

    expect(original).toHaveLength(1);
    expect(component.tasks().map((t) => t.id)).toEqual([1, 2]);
  });

  it("re-renders after a prepend", () => {
    component.tasks.set([REVIEW]);
    component.prepend(WRITE);
    fixture.detectChanges();

    expect(texts()).toEqual(["0: Write (2)", "1: Review (2)"]);
  });

  it("reuses the existing DOM node for a tracked item", () => {
    component.tasks.set([REVIEW, SHIP]);
    fixture.detectChanges();

    const reviewRow = rows()[0];
    expect(reviewRow.textContent).toContain("Review");

    component.prepend(WRITE);
    fixture.detectChanges();

    // Review moved from index 0 to index 1. Tracked by id, Angular *moves* the node it
    // already built; tracking by $index would have thrown it away and rebuilt it.
    expect(rows()[1]).toBe(reviewRow);
    expect(rows()[1].textContent).toContain("Review");
  });

  it("keeps tracked nodes across a full reorder", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    const before = new Map(rows().map((row) => [row.getAttribute("data-id"), row]));
    // Guard the loop below: with no rows at all it would assert nothing.
    expect(before.size).toBe(3);

    component.tasks.set([SHIP, WRITE, REVIEW]);
    fixture.detectChanges();

    expect(texts()).toEqual(["0: Ship (3)", "1: Write (3)", "2: Review (3)"]);
    for (const row of rows()) {
      expect(row).toBe(before.get(row.getAttribute("data-id")));
    }
  });

  it("drops rows for removed tasks", () => {
    component.tasks.set([WRITE, REVIEW, SHIP]);
    fixture.detectChanges();

    component.tasks.set([SHIP]);
    fixture.detectChanges();

    expect(texts()).toEqual(["0: Ship (1)"]);
  });

  it("goes back to the empty block when the last task goes", () => {
    component.tasks.set([WRITE]);
    fixture.detectChanges();

    component.tasks.set([]);
    fixture.detectChanges();

    expect(rows()).toHaveLength(0);
    expect(maybe("li.empty")).not.toBeNull();
  });
});
