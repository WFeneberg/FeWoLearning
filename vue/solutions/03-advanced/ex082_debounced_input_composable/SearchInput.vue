<!-- Exercise 082 — SearchInput component (reference solution). -->
<script setup lang="ts">
import { onUnmounted, ref } from "vue";

const props = withDefaults(defineProps<{ delay?: number }>(), { delay: 300 });
const emit = defineEmits<{ search: [query: string] }>();

const query = ref("");

// A tiny debounce composable: wraps `fn` so it only runs `delay` ms after the
// most recent call, resetting the timer on every new call in between.
function useDebouncedFn<Args extends unknown[]>(
  fn: (...args: Args) => void,
  delay: number,
): (...args: Args) => void {
  let timer: ReturnType<typeof setTimeout> | undefined;

  onUnmounted(() => {
    if (timer !== undefined) clearTimeout(timer);
  });

  return (...args: Args) => {
    if (timer !== undefined) clearTimeout(timer);
    timer = setTimeout(() => {
      timer = undefined;
      fn(...args);
    }, delay);
  };
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
