import { provideZonelessChangeDetection } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ZonelessCounterComponent } from "./zoneless-counter.component";

describe("ZonelessCounterComponent", () => {
  let fixture: ComponentFixture<ZonelessCounterComponent>;
  let component: ZonelessCounterComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ZonelessCounterComponent],
      providers: [provideZonelessChangeDetection()],
    });
    fixture = TestBed.createComponent(ZonelessCounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("increment() writes to the count signal, and a manual detectChanges reflects it", () => {
    component.increment();
    fixture.detectChanges();

    expect(component.count()).toBe(1);
    expect(fixture.nativeElement.querySelector(".count").textContent).toBe("1");
  });

  it("increment() can be called repeatedly and stays consistent with the rendered DOM", () => {
    component.increment();
    component.increment();
    component.increment();
    fixture.detectChanges();

    expect(component.count()).toBe(3);
    expect(fixture.nativeElement.querySelector(".count").textContent).toBe("3");
  });

  it("bumpLegacyCount() mutates the plain field AND notifies Angular, so a later detectChanges shows it", () => {
    component.bumpLegacyCount();
    fixture.detectChanges();

    expect(component.legacyCount).toBe(1);
    expect(fixture.nativeElement.querySelector(".legacy").textContent).toBe("1");
  });

  it("bumpLegacyCount() keeps notifying on every call, not just the first", () => {
    component.bumpLegacyCount();
    fixture.detectChanges();
    component.bumpLegacyCount();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector(".legacy").textContent).toBe("2");
  });

  it("a template-bound click is picked up automatically under autoDetectChanges, with no manual detectChanges call", async () => {
    fixture.autoDetectChanges();
    const button = fixture.nativeElement.querySelector(".inc") as HTMLButtonElement;

    button.click();
    await fixture.whenStable();

    expect(fixture.nativeElement.querySelector(".count").textContent).toBe("1");
  });
});
