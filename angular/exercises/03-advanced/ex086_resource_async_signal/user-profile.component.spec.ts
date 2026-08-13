import { ComponentFixture, TestBed } from "@angular/core/testing";
import { UserProfileComponent } from "./user-profile.component";

describe("UserProfileComponent (resource())", () => {
  let fixture: ComponentFixture<UserProfileComponent>;
  let component: UserProfileComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [UserProfileComponent] });
    fixture = TestBed.createComponent(UserProfileComponent);
    component = fixture.componentInstance;
  });

  it("loads the initial user once the loader settles", async () => {
    fixture.detectChanges();
    await fixture.whenStable();

    expect(component.userResource.hasValue()).toBe(true);
    expect(component.userResource.value()?.name).toBe("User 1");
    expect(component.userResource.isLoading()).toBe(false);
  });

  it("re-runs the loader when userId changes, via nextUser()", async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    expect(component.userResource.value()?.name).toBe("User 1");

    component.nextUser();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(component.userResource.value()?.name).toBe("User 2");
  });

  it("surfaces a rejected loader as the error signal, not a thrown value()", async () => {
    component.userId.set(0);
    fixture.detectChanges();
    await fixture.whenStable();

    expect(component.userResource.error()?.message).toBe("No user with id 0");
    expect(component.userResource.hasValue()).toBe(false);
  });

  it("renders the loaded user's name in the DOM", async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).querySelector(".name")?.textContent).toContain(
      "User 1",
    );
  });

  it("clicking Next user advances to the next id and re-renders", async () => {
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const button = (fixture.nativeElement as HTMLElement).querySelector<HTMLButtonElement>(".next")!;
    button.click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).querySelector(".name")?.textContent).toContain(
      "User 2",
    );
  });
});
