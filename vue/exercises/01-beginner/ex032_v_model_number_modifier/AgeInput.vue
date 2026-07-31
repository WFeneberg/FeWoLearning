<!--
  Exercise 032 — AgeInput component (beginner).
  Goal:   bind a numeric <input> with `v-model.number` to an `age` ref.
  Drills: v-model.number modifier, exposing internal refs for testing.
-->
<script setup lang="ts">
import { ref } from "vue";

const age = ref<number>(0);

defineExpose({ age });
</script>

<template>
  <input type="number" v-model="age" data-testid="age-input" />
</template>
