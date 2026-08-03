<!--
  Exercise 032 — AgeInput component (beginner).
  Goal:   bind the text <input> below to `age` with `v-model.number`, so what
          the user types is stored as a number rather than a string.
  Drills: the v-model .number modifier, exposing internal refs for testing.
  Note:   the input is deliberately type="text". On type="number" Vue already
          casts to a number on its own, so the modifier would make no
          observable difference and there would be nothing to practise.
-->
<script setup lang="ts">
import { ref } from "vue";

const age = ref<number>(0);

defineExpose({ age });

// TODO: add `v-model.number="age"` to the input in the template below.
throw new Error("TODO: bind the input with v-model.number");
</script>

<template>
  <input type="text" data-testid="age-input" />
</template>
