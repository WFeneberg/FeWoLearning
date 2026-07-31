<!--
  Exercise 061 — Suspense with error handling (reference solution).
-->
<script setup lang="ts">
import { defineComponent, h, onErrorCaptured, ref } from "vue";

const props = defineProps<{ shouldFail: boolean }>();

const errorMessage = ref("");

onErrorCaptured((err) => {
  errorMessage.value = err instanceof Error ? err.message : String(err);
  return false;
});

const AsyncChild = defineComponent({
  async setup() {
    await Promise.resolve();
    if (props.shouldFail) {
      throw new Error("Failed to load data");
    }
    return () => h("div", { class: "content" }, "Loaded!");
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
