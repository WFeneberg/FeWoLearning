import { TestBed } from "@angular/core/testing";
import { SessionBannerComponent, stableSessionKey } from "./session-banner.component";

describe("stableSessionKey (pure, deterministic — no ambient clock/randomness)", () => {
  it("returns the same key for the same inputs, called twice", () => {
    const first = stableSessionKey("user-42", "2026-01-01T10:00:00.000Z");
    const second = stableSessionKey("user-42", "2026-01-01T10:00:00.000Z");

    expect(first).toBe(second);
  });

  it("is a zero-padded 3-digit string", () => {
    expect(stableSessionKey("abc", "2026-01-01T00:00:00.000Z")).toMatch(/^\d{3}$/);
  });

  it("differs for different inputs, so it is not just a hardcoded constant", () => {
    const a = stableSessionKey("user-1", "2026-01-01T10:00:00.000Z");
    const b = stableSessionKey("user-2", "2026-01-01T10:00:00.000Z");

    expect(a).not.toBe(b);
  });
});

describe("SessionBannerComponent (hydration-stable initial render)", () => {
  const SESSION_ID = "user-42";
  const STARTED_AT = "2026-03-01T14:05:00.000Z";

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [SessionBannerComponent] }).compileComponents();
  });

  function render(sessionId: string, startedAt: string) {
    const fixture = TestBed.createComponent(SessionBannerComponent);
    fixture.componentRef.setInput("sessionId", sessionId);
    fixture.componentRef.setInput("startedAt", startedAt);
    fixture.detectChanges();
    return fixture;
  }

  it("derives an HH:MM UTC label purely from the startedAt input", () => {
    const fixture = render(SESSION_ID, STARTED_AT);

    expect(fixture.componentInstance.startedAtLabel()).toBe("14:05 UTC");
  });

  it("derives sessionKey via the pure stableSessionKey helper, not some independent logic", () => {
    const fixture = render(SESSION_ID, STARTED_AT);

    expect(fixture.componentInstance.sessionKey()).toBe(stableSessionKey(SESSION_ID, STARTED_AT));
  });

  it("renders byte-identical output from two independent constructions — simulating a server render and the client's hydration render from the same inputs", () => {
    const serverRender = render(SESSION_ID, STARTED_AT);
    const clientRender = render(SESSION_ID, STARTED_AT);

    expect(serverRender.nativeElement.textContent).toBe(clientRender.nativeElement.textContent);
    expect(serverRender.componentInstance.sessionKey()).toBe(clientRender.componentInstance.sessionKey());
    expect(serverRender.componentInstance.startedAtLabel()).toBe(
      clientRender.componentInstance.startedAtLabel(),
    );
  });

  it("produces different output for genuinely different inputs (not hardcoded)", () => {
    const fixtureA = render(SESSION_ID, STARTED_AT);
    const fixtureB = render("someone-else", "2026-03-01T09:30:00.000Z");

    expect(fixtureB.nativeElement.textContent).not.toBe(fixtureA.nativeElement.textContent);
  });
});
