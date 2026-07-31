<!-- Exercise 083 — ErrorBoundaryCapture component (reference solution). -->
<script setup lang="ts">
import { onErrorCaptured, ref } from "vue";

withDefaults(defineProps<{ fallbackMessage?: string }>(), {
  fallbackMessage: "Something went wrong.",
});

const capturedError = ref<Error | null>(null);

onErrorCaptured((err) => {
  capturedError.value = err instanceof Error ? err : new Error(String(err));
  // Returning false stops the error from propagating further up the
  // component tree (and suppresses Vue's default console logging for it),
  // so the rest of the app keeps working.
  return false;
});
</script>

<template>
  <div v-if="capturedError">{{ fallbackMessage }}</div>
  <slot v-else />
</template>
