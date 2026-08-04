import { Component } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { TabComponent, TabSetComponent } from "./tab-set.component";

@Component({
  selector: "app-tabs-host",
  standalone: true,
  imports: [TabSetComponent, TabComponent],
  template: `
    <app-tab-set>
      <app-tab label="first">First body</app-tab>
      <app-tab label="second">Second body</app-tab>
      <app-tab label="third">Third body</app-tab>
    </app-tab-set>
  `,
})
class TabsHostComponent {}

@Component({
  selector: "app-empty-tabs-host",
  standalone: true,
  imports: [TabSetComponent],
  template: `<app-tab-set />`,
})
class EmptyTabsHostComponent {}

describe("TabSetComponent", () => {
  let fixture: ComponentFixture<TabsHostComponent>;
  let tabSet: TabSetComponent;

  const buttons = (): HTMLButtonElement[] =>
    Array.from(
      fixture.nativeElement.querySelectorAll("button.tab-button") as NodeListOf<HTMLButtonElement>,
    );

  const bodies = (): string[] =>
    Array.from(
      fixture.nativeElement.querySelectorAll("div.tab-body") as NodeListOf<HTMLElement>,
    ).map((node) => node.textContent?.trim() ?? "");

  const text = (selector: string): string => {
    const found = fixture.nativeElement.querySelector(selector) as HTMLElement | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template implemented?`);
    }
    return found.textContent?.trim() ?? "";
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TabsHostComponent, EmptyTabsHostComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(TabsHostComponent);
    fixture.detectChanges();
    tabSet = fixture.debugElement.children[0].componentInstance as TabSetComponent;
  });

  it("finds every projected tab", () => {
    expect(tabSet.tabList()).toHaveLength(3);
    expect(tabSet.tabList().map((tab) => tab.label())).toEqual(["first", "second", "third"]);
  });

  it("finds the first projected tab", () => {
    expect(tabSet.firstTab()).toBeInstanceOf(TabComponent);
    expect(tabSet.firstTab()).toBe(tabSet.tabList()[0]);
  });

  it("renders one button per tab", () => {
    expect(buttons().map((button) => button.textContent?.trim())).toEqual([
      "first",
      "second",
      "third",
    ]);
  });

  it("shows no body until a tab is selected", () => {
    expect(bodies()).toEqual([]);
    expect(text("p.active")).toBe("");
  });

  it("activates a tab by label", () => {
    tabSet.select("second");
    fixture.detectChanges();

    expect(text("p.active")).toBe("second");
    expect(bodies()).toEqual(["Second body"]);
  });

  it("shows exactly one body at a time", () => {
    tabSet.select("first");
    tabSet.select("third");
    fixture.detectChanges();

    expect(bodies()).toEqual(["Third body"]);
    expect(tabSet.tabList().filter((tab) => tab.active())).toHaveLength(1);
  });

  it("ignores an unknown label", () => {
    tabSet.select("second");
    fixture.detectChanges();
    expect(text("p.active")).toBe("second");

    tabSet.select("nope");
    fixture.detectChanges();

    // A no-op, not a reset.
    expect(text("p.active")).toBe("second");
  });

  it("activates from a button click", () => {
    buttons()[1].click();
    fixture.detectChanges();

    expect(text("p.active")).toBe("second");
    expect(bodies()).toEqual(["Second body"]);
  });

  it("selects the first tab on request", () => {
    tabSet.selectFirst();
    fixture.detectChanges();

    expect(text("p.active")).toBe("first");
    expect(bodies()).toEqual(["First body"]);
  });

  it("drives children that know nothing about it", () => {
    tabSet.select("third");

    // The container reads and writes the child's own state; the child never looks up.
    expect(tabSet.tabList()[2].active()).toBe(true);
    expect(tabSet.tabList()[0].active()).toBe(false);
  });

  it("copes with no projected tabs at all", () => {
    const empty = TestBed.createComponent(EmptyTabsHostComponent);
    empty.detectChanges();
    const emptySet = empty.debugElement.children[0].componentInstance as TabSetComponent;

    expect(emptySet.tabList()).toEqual([]);
    expect(emptySet.firstTab()).toBeUndefined();
    expect(emptySet.activeLabel()).toBe("");
    expect(() => emptySet.selectFirst()).not.toThrow();
  });

  it("finds content, not view children", () => {
    // The tabs live in the *host's* template, not the tab set's — a viewChild query would
    // find nothing here, which is the distinction this exercise exists for.
    expect(tabSet.tabList()).toHaveLength(3);
    expect(fixture.nativeElement.querySelectorAll("div.panels app-tab")).toHaveLength(3);
  });
});
