import { Component, signal } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { PanelComponent } from "./panel.component";

@Component({
  selector: "app-with-content",
  standalone: true,
  imports: [PanelComponent],
  template: `
    <app-panel [heading]="heading()">
      <p class="projected">{{ message() }}</p>
    </app-panel>
  `,
})
class WithContentComponent {
  readonly heading = signal("Details");
  readonly message = signal("from the parent");
}

@Component({
  selector: "app-without-content",
  standalone: true,
  imports: [PanelComponent],
  template: `<app-panel />`,
})
class WithoutContentComponent {}

describe("PanelComponent", () => {
  const build = async <T>(type: new () => T): Promise<ComponentFixture<T>> => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({ imports: [type as never] }).compileComponents();
    const fixture = TestBed.createComponent(type);
    fixture.detectChanges();
    return fixture;
  };

  const text = (fixture: ComponentFixture<unknown>, selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  it("renders its own frame", async () => {
    const fixture = await build(WithoutContentComponent);

    expect(text(fixture, "section.panel")).not.toBe("");
    expect(text(fixture, "h3.heading")).toBe("Panel");
  });

  it("takes the heading from its input", async () => {
    const fixture = await build(WithContentComponent);

    expect(text(fixture, "h3.heading")).toBe("Details");
  });

  it("places the caller's content in its body", async () => {
    const fixture = await build(WithContentComponent);

    expect(text(fixture, "div.body p.projected")).toBe("from the parent");
  });

  it("falls back when nothing is projected", async () => {
    const fixture = await build(WithoutContentComponent);

    expect(text(fixture, "div.body")).toBe("nothing here yet");
  });

  it("drops the fallback once content is supplied", async () => {
    const fixture = await build(WithContentComponent);

    expect(text(fixture, "div.body")).not.toContain("nothing here yet");
  });

  it("resolves projected bindings against the parent", async () => {
    const fixture = await build(WithContentComponent);
    const host = fixture.componentInstance as WithContentComponent;

    host.message.set("changed");
    fixture.detectChanges();

    // The panel never sees `message` — the parent renders it and the panel only hosts the DOM.
    expect(text(fixture, "p.projected")).toBe("changed");
  });

  it("keeps the frame and the content independent", async () => {
    const fixture = await build(WithContentComponent);
    const host = fixture.componentInstance as WithContentComponent;

    host.heading.set("Renamed");
    fixture.detectChanges();

    expect(text(fixture, "h3.heading")).toBe("Renamed");
    expect(text(fixture, "p.projected")).toBe("from the parent");
  });

  it("can be reused with different content", async () => {
    const withContent = await build(WithContentComponent);
    expect(text(withContent, "p.projected")).toBe("from the parent");

    const without = await build(WithoutContentComponent);
    // Same component, two shapes — which is the whole point of projecting.
    expect(without.nativeElement.querySelector("p.projected")).toBeNull();
    expect(text(without, "div.body")).toBe("nothing here yet");
  });
});
