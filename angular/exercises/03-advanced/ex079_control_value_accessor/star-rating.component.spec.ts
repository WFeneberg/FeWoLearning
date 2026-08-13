import { Component, viewChild } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { StarRatingComponent } from "./star-rating.component";

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, StarRatingComponent],
  template: `<app-star-rating [formControl]="ctrl" />`,
})
class HostComponent {
  readonly ctrl = new FormControl(3, { nonNullable: true });
  readonly rating = viewChild.required(StarRatingComponent);
}

describe("StarRatingComponent (ControlValueAccessor)", () => {
  let fixture: ComponentFixture<HostComponent>;
  let host: HostComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HostComponent] });
    fixture = TestBed.createComponent(HostComponent);
    host = fixture.componentInstance;
    fixture.detectChanges();
  });

  const stars = () =>
    Array.from((fixture.nativeElement as HTMLElement).querySelectorAll<HTMLButtonElement>(".star"));
  const filledCount = () => stars().filter((button) => button.classList.contains("filled")).length;

  it("renders the FormControl's initial value via writeValue", () => {
    expect(filledCount()).toBe(3);
  });

  it("re-renders when the FormControl is patched from outside the component", () => {
    host.ctrl.setValue(5);
    fixture.detectChanges();

    expect(filledCount()).toBe(5);
  });

  it("updates the FormControl's value when a star is clicked", () => {
    stars()[3].click(); // 4th star, 1-based rating of 4

    expect(host.ctrl.value).toBe(4);
  });

  it("updates the component's own displayed value on click, not just the form", () => {
    stars()[1].click(); // 2nd star, rating of 2
    fixture.detectChanges();

    expect(host.rating().value()).toBe(2);
    expect(filledCount()).toBe(2);
  });

  it("marks the control as touched once the user interacts with it", () => {
    expect(host.ctrl.touched).toBe(false);

    stars()[0].click();

    expect(host.ctrl.touched).toBe(true);
  });

  it("ignores clicks while the control is disabled", () => {
    host.ctrl.disable();
    fixture.detectChanges();

    stars()[4].click();

    expect(host.ctrl.value).toBe(3); // unchanged from the initial value
  });
});
