import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { CardComponent } from "./card.component";

@Component({
  selector: "app-full-card",
  standalone: true,
  imports: [CardComponent],
  template: `
    <app-card>
      <h3 card-title>The title</h3>
      <img src="pic.png" alt="a picture" />
      <p class="card-body">The body</p>
      <span card-footer>The footer</span>
      <em>Left over</em>
    </app-card>
  `,
})
class FullCardComponent {}

@Component({
  selector: "app-partial-card",
  standalone: true,
  imports: [CardComponent],
  template: `
    <app-card>
      <h3 card-title>Only a title</h3>
    </app-card>
  `,
})
class PartialCardComponent {}

@Component({
  selector: "app-nested-card",
  standalone: true,
  imports: [CardComponent],
  template: `
    <app-card>
      <div class="wrapper">
        <span card-footer>Buried footer</span>
      </div>
    </app-card>
  `,
})
class NestedCardComponent {}

describe("CardComponent", () => {
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

  it("routes an attribute-selected node", async () => {
    const fixture = await build(FullCardComponent);

    expect(text(fixture, "header.head")).toBe("The title");
  });

  it("routes an element-selected node", async () => {
    const fixture = await build(FullCardComponent);

    expect(fixture.nativeElement.querySelector("div.media img")).not.toBeNull();
  });

  it("routes a class-selected node", async () => {
    const fixture = await build(FullCardComponent);

    expect(text(fixture, "div.body")).toBe("The body");
  });

  it("routes a second attribute-selected node", async () => {
    const fixture = await build(FullCardComponent);

    expect(text(fixture, "footer.foot")).toBe("The footer");
  });

  it("sends everything else to the catch-all", async () => {
    const fixture = await build(FullCardComponent);

    expect(text(fixture, "div.rest")).toBe("Left over");
  });

  it("keeps each slot to its own content", async () => {
    const fixture = await build(FullCardComponent);

    expect(text(fixture, "header.head")).not.toContain("The body");
    expect(text(fixture, "div.rest")).not.toContain("The title");
  });

  it("leaves unfilled slots empty rather than borrowing", async () => {
    const fixture = await build(PartialCardComponent);

    expect(text(fixture, "header.head")).toBe("Only a title");
    expect(text(fixture, "div.body")).toBe("");
    expect(text(fixture, "footer.foot")).toBe("");
    expect(text(fixture, "div.rest")).toBe("");
  });

  it("does not reach into a nested node", async () => {
    const fixture = await build(NestedCardComponent);

    // Only top-level projected nodes are matched, so the buried footer travels with its wrapper
    // into the catch-all instead.
    expect(text(fixture, "footer.foot")).toBe("");
    expect(text(fixture, "div.rest")).toContain("Buried footer");
  });
});
