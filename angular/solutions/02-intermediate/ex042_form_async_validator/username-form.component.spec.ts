import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormControl, Validators } from "@angular/forms";
import {
  uniqueUsername,
  UsernameFormComponent,
  UsernameService,
} from "./username-form.component";

describe("uniqueUsername", () => {
  let service: UsernameService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsernameService);
  });

  const controlWith = (value: string): FormControl =>
    new FormControl(value, { asyncValidators: [uniqueUsername(service)] });

  it("asks the service about the value", () => {
    controlWith("ada");

    expect(service.queries).toEqual(["ada"]);
  });

  it("stays pending until answered", () => {
    const control = controlWith("ada");

    expect(control.status).toBe("PENDING");
    expect(control.errors).toBeNull();
  });

  it("becomes invalid for a taken name", () => {
    const control = controlWith("ada");

    service.resolve(true);

    expect(control.status).toBe("INVALID");
    expect(control.errors).toEqual({ taken: { name: "ada" } });
  });

  it("becomes valid for a free name", () => {
    const control = controlWith("zoe");

    service.resolve(false);

    expect(control.status).toBe("VALID");
    expect(control.errors).toBeNull();
  });

  it("settles rather than staying pending", () => {
    const control = controlWith("ada");

    service.resolve(false);

    // If the returned observable never completed, this would still be PENDING — the single
    // most common bug in an async validator, and it fails with no error to look at.
    expect(control.status).not.toBe("PENDING");
    expect(service.outstanding).toBe(0);
  });

  it("re-validates a new value", () => {
    const control = controlWith("ada");
    service.resolve(true);
    expect(control.status).toBe("INVALID");

    control.setValue("zoe");
    expect(control.status).toBe("PENDING");
    service.resolve(false);

    expect(control.status).toBe("VALID");
    expect(service.queries).toEqual(["ada", "zoe"]);
  });

  it("does not run while a synchronous validator is failing", () => {
    const control = new FormControl("", {
      validators: [Validators.required],
      asyncValidators: [uniqueUsername(service)],
    });

    // No point asking a server about "": Angular skips the async pass entirely.
    expect(control.status).toBe("INVALID");
    expect(control.errors).toEqual({ required: true });
    expect(service.queries).toEqual([]);
  });
});

describe("UsernameFormComponent", () => {
  let fixture: ComponentFixture<UsernameFormComponent>;
  let component: UsernameFormComponent;
  let service: UsernameService;

  const text = (selector: string): string =>
    (fixture.nativeElement.querySelector(selector) as HTMLElement | null)?.textContent?.trim() ??
    `MISSING ${selector}`;

  const submitButton = (): HTMLButtonElement => {
    const found = fixture.nativeElement.querySelector("button.submit") as HTMLButtonElement | null;
    if (found === null) {
      throw new Error('no element matched "button.submit" — is the template implemented?');
    }
    return found;
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsernameFormComponent],
    }).compileComponents();
    service = TestBed.inject(UsernameService);
    fixture = TestBed.createComponent(UsernameFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("starts required, with no query made", () => {
    expect(component.statusLabel()).toBe("required");
    expect(service.queries).toEqual([]);
    expect(component.canSubmit()).toBe(false);
  });

  it("shows the pending state", () => {
    component.username.setValue("ada");
    fixture.detectChanges();

    expect(component.statusLabel()).toBe("checking…");
    expect(text("p.status")).toBe("checking…");
  });

  it("refuses to submit while pending", () => {
    component.username.setValue("ada");
    fixture.detectChanges();

    // A button wired only to `invalid` would be enabled here, mid-flight.
    expect(component.canSubmit()).toBe(false);
    expect(submitButton().disabled).toBe(true);
  });

  it("reports a taken name", () => {
    component.username.setValue("ada");
    service.resolve(true);
    fixture.detectChanges();

    expect(component.statusLabel()).toBe("taken");
    expect(component.takenName()).toBe("ada");
    expect(component.canSubmit()).toBe(false);
  });

  it("reports a free name and allows submitting", () => {
    component.username.setValue("zoe");
    service.resolve(false);
    fixture.detectChanges();

    expect(component.statusLabel()).toBe("free");
    expect(component.takenName()).toBeNull();
    expect(component.canSubmit()).toBe(true);
    expect(submitButton().disabled).toBe(false);
  });

  it("goes back to required when emptied, without querying", () => {
    component.username.setValue("zoe");
    service.resolve(false);
    expect(component.canSubmit()).toBe(true);

    component.username.setValue("");
    fixture.detectChanges();

    expect(component.statusLabel()).toBe("required");
    expect(component.canSubmit()).toBe(false);
    // Still just the one query, from the earlier value.
    expect(service.queries).toEqual(["zoe"]);
  });

  it("re-checks each new value", () => {
    component.username.setValue("ada");
    service.resolve(true);
    component.username.setValue("zoe");
    service.resolve(false);
    fixture.detectChanges();

    expect(service.queries).toEqual(["ada", "zoe"]);
    expect(component.statusLabel()).toBe("free");
  });
});
