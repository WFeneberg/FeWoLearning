import { ComponentFixture, TestBed } from "@angular/core/testing";
import { NicknameFieldComponent } from "./nickname-field.component";

describe("NicknameFieldComponent", () => {
  let fixture: ComponentFixture<NicknameFieldComponent>;
  let component: NicknameFieldComponent;

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  const input = (): HTMLInputElement => {
    const found = fixture.nativeElement.querySelector("input.nickname") as HTMLInputElement | null;
    if (found === null) {
      throw new Error('no element matched "input.nickname" — is the template implemented?');
    }
    return found;
  };

  const typeInto = (value: string): void => {
    const element = input();
    element.value = value;
    element.dispatchEvent(new Event("input"));
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NicknameFieldComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(NicknameFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts empty", () => {
    expect(component.nickname.value).toBe("");
    expect(component.hasValue()).toBe(false);
  });

  it("holds a value set through the control", () => {
    component.rename("ada");

    expect(component.nickname.value).toBe("ada");
    expect(component.hasValue()).toBe(true);
  });

  it("trims what it is given", () => {
    component.rename("  ada  ");

    expect(component.nickname.value).toBe("ada");
  });

  it("treats whitespace as empty", () => {
    component.rename("   ");

    expect(component.nickname.value).toBe("");
    expect(component.hasValue()).toBe(false);
  });

  it("pushes the control's value into the input element", () => {
    component.rename("grace");
    fixture.detectChanges();

    expect(input().value).toBe("grace");
    expect(text("p.echo")).toBe("grace");
  });

  it("takes a typed value from the input element", () => {
    typeInto("hopper");

    expect(component.nickname.value).toBe("hopper");
  });

  it("records changes on the stream", () => {
    component.startRecording();

    component.rename("a");
    component.rename("ab");

    expect(component.changes()).toEqual(["a", "ab"]);
  });

  it("records nothing before anything changes", () => {
    component.startRecording();

    // Subscribing does not replay the current value — valueChanges only carries changes.
    expect(component.changes()).toEqual([]);

    component.rename("a");
    expect(component.changes()).toEqual(["a"]);
  });

  it("records a value typed into the element too", () => {
    component.startRecording();

    typeInto("hopper");

    expect(component.changes()).toEqual(["hopper"]);
  });

  it("stays silent for a quiet write", () => {
    component.startRecording();

    component.renameQuietly("secret");

    // The value moved...
    expect(component.nickname.value).toBe("secret");
    // ...but nothing was notified, which is what breaks a setValue feedback loop.
    expect(component.changes()).toEqual([]);
  });

  it("renders a quiet write once change detection runs", () => {
    component.renameQuietly("secret");
    fixture.detectChanges();

    expect(input().value).toBe("secret");
  });

  it("resumes emitting after a quiet write", () => {
    component.startRecording();
    component.renameQuietly("secret");

    component.rename("loud");

    expect(component.changes()).toEqual(["loud"]);
  });

  it("resets to the initial value, not to null", () => {
    component.rename("ada");
    component.clear();

    // nonNullable is what makes this "" rather than null.
    expect(component.nickname.value).toBe("");
    expect(component.hasValue()).toBe(false);
  });

  it("keeps the control's own value while disabled", () => {
    component.rename("ada");
    component.nickname.disable();

    expect(component.nickname.disabled).toBe(true);
    // Disabled means "not submitted", not "emptied".
    expect(component.nickname.value).toBe("ada");
  });

  it("disables the input element too", () => {
    component.nickname.disable();
    fixture.detectChanges();

    expect(input().disabled).toBe(true);
  });

  it("re-enables", () => {
    component.nickname.disable();
    component.nickname.enable();
    fixture.detectChanges();

    expect(component.nickname.disabled).toBe(false);
    expect(input().disabled).toBe(false);
  });

  it("tracks its own dirty flag", () => {
    expect(component.nickname.dirty).toBe(false);

    // A programmatic setValue does not dirty a control; user interaction does.
    component.rename("ada");
    expect(component.nickname.dirty).toBe(false);

    typeInto("hopper");
    expect(component.nickname.dirty).toBe(true);
  });

  it("renders how many changes it has seen", () => {
    component.startRecording();
    component.rename("a");
    component.rename("b");
    fixture.detectChanges();

    expect(text("p.changes")).toBe("2");
  });
});
