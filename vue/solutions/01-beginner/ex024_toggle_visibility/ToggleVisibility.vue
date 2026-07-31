<!--
  Exercise 024 — ToggleVisibility component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const visible = ref(true);

function toggle() {
  visible.value = !visible.value;
}
</script>

<template>
  <div>
    <button @click="toggle">Toggle</button>
    <p v-if="visible" data-testid="message">Hello there!</p>
  </div>
</template>
