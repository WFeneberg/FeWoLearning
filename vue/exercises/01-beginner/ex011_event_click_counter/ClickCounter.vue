<!-- Exercise 011 — ClickCounter component (beginner). -->
<!-- Goal:   increment a `count` ref on button click via `@click`. -->
<!-- Drills: v-on click handling, template interpolation, ref binding. -->
<script setup lang="ts">
import { ref } from "vue";

const count = ref(0);

function increment(): void {
  throw new Error("TODO: implement increment on click");
}
</script>

<template>
  <button @click="increment">Count: {{ count }}</button>
</template>
