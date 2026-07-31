<!-- Exercise 012 — TwoWayTextInput component (beginner). -->
<!-- Goal:   bind a text input with v-model to a `name` ref shown elsewhere. -->
<!-- Drills: ref, v-model on <input>, template interpolation. -->
<script setup lang="ts">
import { ref } from "vue";

const name = ref("");

// TODO: bind the <input> below to `name` with v-model so the greeting
// updates as the user types.
throw new Error("TODO: implement v-model binding on the input");
</script>

<template>
  <div>
    <input type="text" data-testid="name-input" />
    <p data-testid="greeting">Hello, {{ name }}!</p>
  </div>
</template>
