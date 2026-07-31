<!--
  Exercise 053 — ChildCounter component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const count = ref(0);

function increment(): void {
  count.value += 1;
}

function reset(): void {
  count.value = 0;
}

defineExpose({
  reset,
});
</script>

<template>
  <div class="child-counter">
    <p class="child-count">Count: {{ count }}</p>
    <button type="button" class="child-increment" @click="increment">
      Increment
    </button>
  </div>
</template>
