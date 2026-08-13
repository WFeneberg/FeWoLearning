import { ComponentFixture, TestBed, fakeAsync, flushMicrotasks, tick } from "@angular/core/testing";
import { SaveIndicatorComponent } from "./save-indicator.component";

describe("SaveIndicatorComponent (fakeAsync/tick/flushMicrotasks)", () => {
  let fixture: ComponentFixture<SaveIndicatorComponent>;
  let component: SaveIndicatorComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [SaveIndicatorComponent] });
    fixture = TestBed.createComponent(SaveIndicatorComponent);
    component = fixture.componentInstance;
  });

  it("moves to the saving state synchronously when save() is called", () => {
    component.save();

    expect(component.state()).toBe("saving");
  });

  it("resolves the microtask-based validation via flushMicrotasks, before any timer fires", fakeAsync(() => {
    component.save();
    expect(component.validated()).toBe(false);

    flushMicrotasks();

    expect(component.validated()).toBe(true);
    expect(component.state()).toBe("saving");
  }));

  it("does not flip to saved until the 2000ms timer has fully elapsed", fakeAsync(() => {
    component.save();

    tick(1999);
    expect(component.state()).toBe("saving");

    tick(1);
    expect(component.state()).toBe("saved");
  }));

  it("flushMicrotasks alone never fires the save timer", fakeAsync(() => {
    component.save();

    flushMicrotasks();

    expect(component.state()).toBe("saving");
  }));

  it("clicking Save eventually renders the saved state once ticked forward", fakeAsync(() => {
    fixture.detectChanges();
    const button = (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>(".save")!;

    button.click();
    tick(2000);
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).querySelector(".state")!.textContent).toContain("saved");
  }));
});
