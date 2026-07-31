<!--
  Exercise 032 — AgeInput component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const age = ref<number>(0);

defineExpose({ age });
</script>

<template>
  <input type="number" v-model.number="age" data-testid="age-input" />
</template>
