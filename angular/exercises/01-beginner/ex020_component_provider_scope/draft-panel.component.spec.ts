import { ComponentFixture, TestBed } from "@angular/core/testing";
import {
  DraftPanelComponent,
  DraftStore,
  SaveCounter,
  ShellComponent,
} from "./draft-panel.component";

describe("component-level providers", () => {
  let fixture: ComponentFixture<DraftPanelComponent>;
  let component: DraftPanelComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  beforeEach(async () => {
    DraftStore.instances = 0;
    await TestBed.configureTestingModule({
      imports: [DraftPanelComponent, ShellComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(DraftPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("resolves the scoped service from the component's own injector", () => {
    expect(component.draft).toBeInstanceOf(DraftStore);
    expect(DraftStore.instances).toBe(1);
  });

  it("holds and reports the draft", () => {
    component.draft.write("hello");

    expect(component.draft.value()).toBe("hello");
  });

  it("starts clean and turns dirty", () => {
    expect(component.draft.isDirty()).toBe(false);

    component.draft.write("hello");

    expect(component.draft.isDirty()).toBe(true);
  });

  it("treats whitespace as clean", () => {
    component.draft.write("   \n ");

    expect(component.draft.isDirty()).toBe(false);
  });

  it("renders the draft and its state", () => {
    component.draft.write("hello");
    fixture.detectChanges();

    expect(query("p.draft").textContent).toContain("hello");
    expect(query("p.dirty").textContent).toContain("dirty");
  });

  it("clears its own draft on save", () => {
    component.draft.write("hello");
    component.save();

    expect(component.draft.value()).toBe("");
    expect(component.saves.count).toBe(1);
  });

  it("saves from the button", () => {
    component.draft.write("hello");
    query<HTMLButtonElement>("button.save").click();

    expect(component.draft.value()).toBe("");
    expect(component.saves.count).toBe(1);
  });

  it("gives each panel instance its own store", () => {
    const shell = TestBed.createComponent(ShellComponent);
    shell.detectChanges();

    const panels = shell.debugElement.children.map(
      (child) => child.componentInstance as DraftPanelComponent,
    );
    expect(panels).toHaveLength(2);

    expect(panels[0].draft).toBeInstanceOf(DraftStore);
    expect(panels[0].draft).not.toBe(panels[1].draft);
  });

  it("keeps one panel's edits out of the other", () => {
    const shell = TestBed.createComponent(ShellComponent);
    shell.detectChanges();

    const panels = shell.debugElement.children.map(
      (child) => child.componentInstance as DraftPanelComponent,
    );
    expect(panels).toHaveLength(2);

    panels[0].draft.write("mine");

    expect(panels[1].draft.value()).toBe("");
  });

  it("still shares the root-provided counter", () => {
    const shell = TestBed.createComponent(ShellComponent);
    shell.detectChanges();

    const panels = shell.debugElement.children.map(
      (child) => child.componentInstance as DraftPanelComponent,
    );

    // Different DraftStores, same SaveCounter — the whole point of the contrast.
    expect(panels[0].draft).not.toBe(panels[1].draft);
    expect(panels[0].saves).toBe(panels[1].saves);
    expect(panels[0].saves).toBe(TestBed.inject(SaveCounter));
  });

  it("counts saves from both panels on the shared counter", () => {
    const shell = TestBed.createComponent(ShellComponent);
    shell.detectChanges();

    const panels = shell.debugElement.children.map(
      (child) => child.componentInstance as DraftPanelComponent,
    );
    panels[0].draft.write("a");
    panels[0].save();
    panels[1].draft.write("b");
    panels[1].save();

    expect(TestBed.inject(SaveCounter).count).toBe(2);
  });

  it("builds one store per panel, not one per application", () => {
    DraftStore.instances = 0;

    const shell = TestBed.createComponent(ShellComponent);
    shell.detectChanges();

    expect(DraftStore.instances).toBe(2);
  });
});
