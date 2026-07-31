<!--
  Exercise 024 — ToggleVisibility component (beginner).
  Goal:   a button that toggles a `visible` ref controlling a v-if block.
  Drills: ref, v-if, click handlers.
-->
<script setup lang="ts">
import { ref } from "vue";

const visible = ref(true);

function toggle() {
  throw new Error("TODO: implement toggle");
}
</script>

<template>
  <div>
    <button @click="toggle">Toggle</button>
    <p v-if="visible" data-testid="message">Hello there!</p>
  </div>
</template>
