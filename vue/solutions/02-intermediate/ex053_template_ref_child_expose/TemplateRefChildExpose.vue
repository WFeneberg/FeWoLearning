<!--
  Exercise 053 — TemplateRefChildExpose component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";
import ChildCounter from "./ChildCounter.vue";

const childRef = ref<InstanceType<typeof ChildCounter> | null>(null);

function resetChild(): void {
  childRef.value?.reset();
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
