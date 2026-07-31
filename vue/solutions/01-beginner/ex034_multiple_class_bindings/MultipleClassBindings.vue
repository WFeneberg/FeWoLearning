<!-- Exercise 034 — MultipleClassBindings component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const isActive = ref(false);
const size = ref<"small" | "large">("small");

function toggleActive() {
  isActive.value = !isActive.value;
}

function toggleSize() {
  size.value = size.value === "small" ? "large" : "small";
}
</script>

<template>
  <div>
    <div
      class="box"
      :class="[{ active: isActive }, `size-${size}`]"
      data-testid="box"
    >
      Box
    </div>
    <button data-testid="toggle-active" @click="toggleActive">Toggle Active</button>
    <button data-testid="toggle-size" @click="toggleSize">Toggle Size</button>
  </div>
</template>
