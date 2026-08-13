import { Component, computed, input } from "@angular/core";

// Exercise 094 — SSR-safe rendering: hydration-stable initial state (reference solution).

export function stableSessionKey(sessionId: string, startedAt: string): string {
  const combined = `${sessionId}|${startedAt}`;
  let sum = 0;
  for (let i = 0; i < combined.length; i++) {
    sum += combined.charCodeAt(i);
  }
  return String(sum % 997).padStart(3, "0");
}

@Component({
  selector: "app-session-banner",
  standalone: true,
  template: `<p class="banner">Session {{ sessionKey() }} started {{ startedAtLabel() }}</p>`,
})
export class SessionBannerComponent {
  readonly sessionId = input.required<string>();
  readonly startedAt = input.required<string>();

  readonly startedAtLabel = computed((): string => {
    // Parsing a GIVEN string is deterministic — unlike `new Date()` with no args, this never reads
    // the wall clock, so server and client agree.
    const date = new Date(this.startedAt());
    const hh = String(date.getUTCHours()).padStart(2, "0");
    const mm = String(date.getUTCMinutes()).padStart(2, "0");
    return `${hh}:${mm} UTC`;
  });

  readonly sessionKey = computed((): string => stableSessionKey(this.sessionId(), this.startedAt()));
}
