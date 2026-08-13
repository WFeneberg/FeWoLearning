import { ChangeDetectionStrategy, Component, signal } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { VolumeDialComponent } from "./volume-dial.component";

/** A parent that binds both ways, which is what model() exists to support. */
@Component({
  selector: "app-host",
  standalone: true,
  imports: [VolumeDialComponent],
  // Explicit: Angular 22.1.1's JIT compiler compiles an omitted `changeDetection` as OnPush
  // instead of the intended CheckAlways default (an emitted-definition bug, not a signals
  // opt-in). hostLevel/hostLabel are plain fields, not signals, so without this the second
  // `host.detectChanges()` below would never push a parent-side write down into the child.
  changeDetection: ChangeDetectionStrategy.Default,
  template: `<app-volume-dial [(level)]="hostLevel" [(label)]="hostLabel" />`,
})
class HostComponent {
  hostLevel = 50;
  hostLabel = "Speaker";
}

describe("VolumeDialComponent", () => {
  let fixture: ComponentFixture<VolumeDialComponent>;
  let component: VolumeDialComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VolumeDialComponent, HostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(VolumeDialComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput("label", "Speaker");
    fixture.detectChanges();
  });

  it("starts at the default level", () => {
    expect(component.level()).toBe(50);
    expect(query("p.level").textContent).toContain("Speaker: 50");
  });

  it("raises and lowers the level", () => {
    component.up();
    expect(component.level()).toBe(60);

    component.down();
    component.down();
    expect(component.level()).toBe(40);
  });

  it("stops at the ceiling", () => {
    component.level.set(95);
    component.up();

    expect(component.level()).toBe(100);

    component.up();
    expect(component.level()).toBe(100);
  });

  it("stops at the floor", () => {
    component.level.set(5);
    component.down();

    expect(component.level()).toBe(0);

    component.down();
    expect(component.level()).toBe(0);
  });

  it("steps from the buttons", () => {
    query<HTMLButtonElement>("button.up").click();
    fixture.detectChanges();

    expect(component.level()).toBe(60);
    expect(query("p.level").textContent).toContain("60");
  });

  it("mutes without losing the level", () => {
    component.toggleMute();

    expect(component.muted()).toBe(true);
    expect(component.level()).toBe(50);
    expect(component.effective()).toBe(0);
  });

  it("unmutes back to the remembered level", () => {
    component.toggleMute();
    component.toggleMute();

    expect(component.muted()).toBe(false);
    expect(component.effective()).toBe(50);
  });

  it("renders the mute state", () => {
    expect(query("p.muted").textContent).toContain("live");

    query<HTMLButtonElement>("button.mute").click();
    fixture.detectChanges();

    expect(query("p.muted").textContent).toContain("muted");
  });

  it("accepts a write from the parent", () => {
    fixture.componentRef.setInput("level", 30);
    fixture.detectChanges();

    // A model is an input too, so the parent can push a value down.
    expect(component.level()).toBe(30);
    expect(query("p.level").textContent).toContain("30");
  });

  it("requires the label", () => {
    const bare = TestBed.createComponent(VolumeDialComponent);

    // Like input.required(), but its own error code: a required *model* is NG0952,
    // while a required *input* is NG0950.
    expect(() => bare.componentInstance.label()).toThrow(/NG0952/);
  });

  it("writes the label back out to the parent", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const dial: VolumeDialComponent = host.debugElement.children[0].componentInstance;
    expect(dial.label()).toBe("Speaker");

    // A model's implicit output is the field name plus "Change", which is exactly what
    // [(label)] subscribes to — so a plain set() inside the child reaches the parent.
    dial.label.set("Headphones");
    host.detectChanges();

    expect(host.componentInstance.hostLabel).toBe("Headphones");
  });

  it("keeps a two-way bound parent in step", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const dial: VolumeDialComponent = host.debugElement.children[0].componentInstance;
    expect(dial.level()).toBe(50);

    dial.up();
    host.detectChanges();

    // The child wrote its own model; [(level)] carried it back up with no wiring.
    expect(host.componentInstance.hostLevel).toBe(60);
  });

  it("lets the parent drive the child through the same binding", () => {
    const host = TestBed.createComponent(HostComponent);
    host.detectChanges();

    const dial: VolumeDialComponent = host.debugElement.children[0].componentInstance;
    host.componentInstance.hostLevel = 20;
    host.detectChanges();

    expect(dial.level()).toBe(20);
  });

  it("does not expose the plain signal to the outside", () => {
    // `muted` is deliberately a local signal, not a model: no input, no output.
    const asOutput = component as unknown as { mutedChange?: unknown };
    expect(asOutput.mutedChange).toBeUndefined();

    component.toggleMute();
    expect(component.muted()).toBe(true);
  });
});
