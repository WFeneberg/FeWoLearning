<!-- Exercise 082 — SearchInput component (advanced).
     Goal:   render a text input that emits a `search` event with the current
             query, but only after typing pauses for `delay` milliseconds —
             wrap the input handler in a debounce composable so rapid
             keystrokes collapse into a single trailing emission.
     Drills: composables that manage timers, debouncing user input,
             defineProps with defaults, defineEmits, cleanup on unmount. -->
<script setup lang="ts">
import { ref } from "vue";

const props = withDefaults(defineProps<{ delay?: number }>(), { delay: 300 });
const emit = defineEmits<{ search: [query: string] }>();

const query = ref("");

// A tiny debounce composable: wraps `fn` so it only runs `delay` ms after the
// most recent call, resetting the timer on every new call in between.
function useDebouncedFn<Args extends unknown[]>(
  _fn: (...args: Args) => void,
  _delay: number,
): (...args: Args) => void {
  throw new Error("TODO: implement useDebouncedFn");
}

const debouncedSearch = useDebouncedFn((value: string) => {
  emit("search", value);
}, props.delay);

function onInput(event: Event): void {
  query.value = (event.target as HTMLInputElement).value;
  debouncedSearch(query.value);
}
</script>

<template>
  <input data-testid="input" type="text" :value="query" @input="onInput" />
</template>
