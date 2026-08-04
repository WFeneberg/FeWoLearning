import { Component, signal } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";

// Exercise 037 — FormControl basics (reference solution).
@Component({
  selector: "app-nickname-field",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <input class="nickname" [formControl]="nickname" />
    <p class="echo">{{ nickname.value }}</p>
    <p class="changes">{{ changes().length }}</p>
  `,
})
export class NicknameFieldComponent {
  // nonNullable does two things: the type is `string` rather than `string | null`, and
  // reset() returns to "" instead of null.
  readonly nickname = new FormControl("", { nonNullable: true });

  readonly changes = signal<readonly string[]>([]);

  startRecording(): void {
    // valueChanges carries changes only — subscribing does not replay the current value.
    this.nickname.valueChanges.subscribe((value) => {
      this.changes.update((seen) => [...seen, value]);
    });
  }

  rename(next: string): void {
    this.nickname.setValue(next.trim());
  }

  renameQuietly(next: string): void {
    // The escape hatch for a listener that writes back: without it, a valueChanges handler
    // calling setValue would loop.
    this.nickname.setValue(next.trim(), { emitEvent: false });
  }

  clear(): void {
    this.nickname.reset();
  }

  hasValue(): boolean {
    return this.nickname.value.trim() !== "";
  }
}
