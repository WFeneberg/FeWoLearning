import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import {
  CanComponentDeactivate,
  DiscardConfirmation,
  NoteEditorComponent,
  unsavedChangesGuard,
} from "./unsaved-changes-guard";

describe("NoteEditorComponent dirty tracking", () => {
  let fixture: ComponentFixture<NoteEditorComponent>;
  let component: NoteEditorComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [NoteEditorComponent] }).compileComponents();
    fixture = TestBed.createComponent(NoteEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts clean", () => {
    expect(component.isDirty()).toBe(false);
    expect(component.canDeactivate()).toBe(true);
  });

  it("becomes dirty after an edit", () => {
    component.text.set("hello");

    expect(component.isDirty()).toBe(true);
    expect(component.canDeactivate()).toBe(false);
  });

  it("is clean again after saving", () => {
    component.text.set("hello");
    component.save();

    expect(component.isDirty()).toBe(false);
  });

  it("goes dirty again after a save if edited further", () => {
    component.text.set("hello");
    component.save();
    component.text.set("hello world");

    expect(component.isDirty()).toBe(true);
  });

  it("updates from the textarea in the template", () => {
    const textarea = fixture.nativeElement.querySelector("textarea.note") as HTMLTextAreaElement;
    textarea.value = "typed";
    textarea.dispatchEvent(new Event("input"));

    expect(component.text()).toBe("typed");
    expect(component.isDirty()).toBe(true);
  });

  it("saves from the button in the template", () => {
    component.text.set("typed");

    (fixture.nativeElement.querySelector("button.save") as HTMLButtonElement).click();

    expect(component.isDirty()).toBe(false);
  });
});

describe("unsavedChangesGuard", () => {
  let confirmation: { confirm: jest.Mock };

  beforeEach(() => {
    confirmation = { confirm: jest.fn() };
    TestBed.configureTestingModule({
      providers: [{ provide: DiscardConfirmation, useValue: confirmation }],
    });
  });

  const runGuard = (component: CanComponentDeactivate) =>
    TestBed.runInInjectionContext(() =>
      unsavedChangesGuard(
        component,
        {} as ActivatedRouteSnapshot,
        {} as RouterStateSnapshot,
        {} as RouterStateSnapshot,
      ),
    );

  it("lets a clean component go without asking", () => {
    const clean: CanComponentDeactivate = { canDeactivate: () => true };

    expect(runGuard(clean)).toBe(true);
    expect(confirmation.confirm).not.toHaveBeenCalled();
  });

  it("asks for confirmation when dirty, and honours a yes", () => {
    confirmation.confirm.mockReturnValue(true);
    const dirty: CanComponentDeactivate = { canDeactivate: () => false };

    expect(runGuard(dirty)).toBe(true);
    expect(confirmation.confirm).toHaveBeenCalledTimes(1);
  });

  it("honours a no", () => {
    confirmation.confirm.mockReturnValue(false);
    const dirty: CanComponentDeactivate = { canDeactivate: () => false };

    expect(runGuard(dirty)).toBe(false);
  });

  it("drives the guard with a real, dirty NoteEditorComponent", async () => {
    confirmation.confirm.mockReturnValue(false);
    await TestBed.configureTestingModule({ imports: [NoteEditorComponent] }).compileComponents();
    const fixture = TestBed.createComponent(NoteEditorComponent);
    fixture.componentInstance.text.set("unsaved work");
    fixture.detectChanges();

    expect(runGuard(fixture.componentInstance)).toBe(false);
  });
});
