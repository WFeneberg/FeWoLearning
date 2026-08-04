import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TemperatureGaugeComponent } from "./temperature-gauge.component";

/** A host binding the inputs by their public names, the way a template would. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [TemperatureGaugeComponent],
  template: `<app-temperature-gauge label="Kitchen" celsius="21.456" digits="2" compact />`,
})
class HostComponent {}

describe("TemperatureGaugeComponent", () => {
  let fixture: ComponentFixture<TemperatureGaugeComponent>;
  let component: TemperatureGaugeComponent;

  const query = <T extends Element>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  const setInput = (name: string, value: unknown): void => {
    fixture.componentRef.setInput(name, value);
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemperatureGaugeComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(TemperatureGaugeComponent);
    component = fixture.componentInstance;
    // The required input has to be bound before anything reads it.
    fixture.componentRef.setInput("label", "Kitchen");
    fixture.detectChanges();
  });

  it("reads a bound required input", () => {
    expect(component.label()).toBe("Kitchen");
    expect(query("h3.label").textContent?.trim()).toBe("Kitchen");
  });

  it("throws when a required input is read before it is bound", () => {
    const bare = TestBed.createComponent(TemperatureGaugeComponent);

    // NG0950 — the loud failure a decorator-based required input does not give you.
    // Matched on the code, not just "it threw": an unimplemented stub throws too.
    expect(() => bare.componentInstance.label()).toThrow(/NG0950/);
  });

  it("keeps the declared defaults", () => {
    expect(component.celsius()).toBe(0);
    expect(component.unit()).toBe("C");
    expect(component.precision()).toBe(1);
    expect(component.compact()).toBe(false);
  });

  it("formats the default reading", () => {
    expect(component.reading()).toBe("0.0 °C");
    expect(query("p.reading").textContent?.trim()).toBe("0.0 °C");
  });

  it("coerces the celsius input to a number", () => {
    setInput("celsius", "21.456");

    expect(component.celsius()).toBeCloseTo(21.456);
    expect(typeof component.celsius()).toBe("number");
  });

  it("rounds to the requested precision", () => {
    setInput("celsius", "21.456");
    setInput("digits", "2");

    expect(component.reading()).toBe("21.46 °C");
  });

  it("exposes precision under its alias", () => {
    setInput("digits", "3");

    expect(component.precision()).toBe(3);
  });

  it("converts to fahrenheit", () => {
    setInput("celsius", 100);
    setInput("unit", "F");
    setInput("digits", "1");

    expect(component.reading()).toBe("212.0 °F");
  });

  it("drops the space in compact mode", () => {
    setInput("celsius", "21.456");
    setInput("digits", "2");
    setInput("compact", "");

    expect(component.compact()).toBe(true);
    expect(component.reading()).toBe("21.46°C");
    expect(query("p.mode").textContent?.trim()).toBe("compact");
  });

  it("re-renders when an input changes, with no ngOnChanges in sight", () => {
    setInput("celsius", 5);

    expect(query("p.reading").textContent?.trim()).toBe("5.0 °C");

    setInput("celsius", 6);

    expect(query("p.reading").textContent?.trim()).toBe("6.0 °C");
  });

  it("exposes inputs as read-only signals", () => {
    // A real signal input reads like a signal...
    expect(component.celsius()).toBe(0);

    // ...but the parent owns the value, so there is nothing to write through.
    const asWritable = component.celsius as unknown as { set?: unknown; update?: unknown };
    expect(asWritable.set).toBeUndefined();
    expect(asWritable.update).toBeUndefined();
  });

  it("wires up from a host template", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const gauge: TemperatureGaugeComponent = host.debugElement.children[0].componentInstance;

    expect(gauge.label()).toBe("Kitchen");
    expect(gauge.precision()).toBe(2);
    expect(gauge.compact()).toBe(true);
    expect(gauge.reading()).toBe("21.46°C");
  });
});
