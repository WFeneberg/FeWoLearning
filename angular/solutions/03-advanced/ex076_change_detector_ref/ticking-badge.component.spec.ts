import { Component, viewChild } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TickingBadgeComponent } from "./ticking-badge.component";

@Component({
  standalone: true,
  imports: [TickingBadgeComponent],
  template: `<app-ticking-badge />`,
})
class HostComponent {
  readonly badge = viewChild.required(TickingBadgeComponent);
}

describe("TickingBadgeComponent (ChangeDetectorRef)", () => {
  let fixture: ComponentFixture<HostComponent>;
  let badge: TickingBadgeComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HostComponent] });
    fixture = TestBed.createComponent(HostComponent);
    fixture.detectChanges();
    badge = fixture.componentInstance.badge();
  });

  const text = () => (fixture.nativeElement.querySelector(".badge") as HTMLElement).textContent;

  it("renders the initial label, then updates once setLabel marks it for check", () => {
    expect(text()).toBe("0");

    badge.setLabel("1");
    fixture.detectChanges();

    expect(text()).toBe("1");
  });

  it("does not refresh on a raw field mutation that bypasses setLabel", () => {
    badge.setLabel("1");
    fixture.detectChanges();
    expect(text()).toBe("1");

    badge.label = "2"; // direct mutation — no markForCheck, so OnPush never learns about it
    fixture.detectChanges();

    expect(text()).toBe("1");
  });

  it("stops refreshing once paused, even though setLabel still calls markForCheck", () => {
    badge.setLabel("1");
    fixture.detectChanges();
    badge.pause();

    badge.setLabel("2");
    fixture.detectChanges();

    expect(text()).toBe("1");
  });

  it("can still be forced to render manually while paused", () => {
    badge.pause();
    badge.setLabel("3");
    badge.renderNow();

    expect(text()).toBe("3");
  });
});
