import { ComponentFixture, TestBed } from "@angular/core/testing";
import { CounterComponent } from "./counter.component";

describe("CounterComponent", () => {
  let fixture: ComponentFixture<CounterComponent>;
  let component: CounterComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CounterComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(CounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("renders the current count", () => {
    component.increment();
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain("1");
  });

  it("never goes below zero", () => {
    component.decrement();
    expect(component.count()).toBe(0);
  });

  it("increments via the + button", () => {
    const buttons: HTMLButtonElement[] = fixture.nativeElement.querySelectorAll("button");
    const plus = Array.from(buttons).find((b) => b.textContent?.includes("+"));
    plus?.click();
    fixture.detectChanges();
    expect(component.count()).toBe(1);
  });
});
