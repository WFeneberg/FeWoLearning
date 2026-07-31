<!--
  Exercise 061 — Suspense with error handling (intermediate).
  Goal:   wrap an async component in <Suspense>, and use onErrorCaptured
          to render a fallback error message when the async setup's
          promise rejects (instead of an unhandled rejection / blank page).
  Drills: <Suspense> default/fallback slots, async setup(), onErrorCaptured,
          returning `false` from onErrorCaptured to stop propagation.
-->
<script setup lang="ts">
import { defineComponent, h, onErrorCaptured, ref } from "vue";

const props = defineProps<{ shouldFail: boolean }>();

const errorMessage = ref("");

onErrorCaptured((err) => {
  errorMessage.value = err instanceof Error ? err.message : String(err);
  return false;
});

// TODO: make this async child resolve to "Loaded!" when `props.shouldFail`
// is false, and reject with new Error("Failed to load data") when it is
// true. As written it always throws, which is what makes this a stub.
const AsyncChild = defineComponent({
  async setup() {
    await Promise.resolve();
    throw new Error("TODO: implement SuspenseErrorFallback");
  },
});
</script>

<template>
  <div>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <Suspense v-else>
      <template #default>
        <AsyncChild />
      </template>
      <template #fallback>
        <p class="loading">Loading...</p>
      </template>
    </Suspense>
  </div>
</template>
