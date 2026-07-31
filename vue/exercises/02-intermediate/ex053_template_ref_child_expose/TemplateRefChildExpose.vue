<!--
  Exercise 053 — TemplateRefChildExpose component (intermediate).
  Goal:   hold a template ref to a child component instance and call a
          method the child exposed via `defineExpose`.
  Drills: template refs typed to a component instance
          (`InstanceType<typeof Child>`), calling exposed child methods,
          guarding against a ref that is not yet mounted.
-->
<script setup lang="ts">
import { ref } from "vue";
import ChildCounter from "./ChildCounter.vue";

const childRef = ref<InstanceType<typeof ChildCounter> | null>(null);

function resetChild(): void {
  throw new Error("TODO: call childRef.value.reset() (guard against null)");
}
</script>

<template>
  <div class="template-ref-child-expose">
    <ChildCounter ref="childRef" />
    <button type="button" class="reset-child" @click="resetChild">
      Reset Child
    </button>
  </div>
</template>
