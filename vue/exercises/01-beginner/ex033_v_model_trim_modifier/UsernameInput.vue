<!--
  Exercise 033 — UsernameInput component (beginner).
  Goal:   bind a text <input> with `v-model.trim` to a `username` ref.
  Drills: v-model.trim modifier, exposing internal refs for testing.
-->
<script setup lang="ts">
import { ref } from "vue";

const username = ref("");

// TODO: bind the <input> below to `username` using the `.trim` modifier
// so leading/trailing whitespace is stripped, then expose `username`.
throw new Error("TODO: implement v-model.trim binding on the input");
</script>

<template>
  <input type="text" data-testid="username-input" />
</template>
