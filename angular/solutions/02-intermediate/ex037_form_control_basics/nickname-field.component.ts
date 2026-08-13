import { ChangeDetectionStrategy, Component, signal } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";

// Exercise 037 — FormControl basics (reference solution).
//
// changeDetection is explicit here because Angular 22.1.1's JIT compiler compiles an
// omitted `changeDetection` decorator property as OnPush rather than the intended
// CheckAlways default (see @angular/compiler's compileComponentFromMetadata). Reactive
// forms push value changes through RxJS/zone patching, not the signal graph, so
// `{{ nickname.value }}` needs CheckAlways to be re-read after a plain setValue().
@Component({
  selector: "app-nickname-field",
  standalone: true,
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.Default,
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
