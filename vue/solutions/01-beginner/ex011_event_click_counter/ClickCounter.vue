<!-- Exercise 011 — ClickCounter component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const count = ref(0);

function increment(): void {
  count.value += 1;
}
</script>

<template>
  <button @click="increment">Count: {{ count }}</button>
</template>
