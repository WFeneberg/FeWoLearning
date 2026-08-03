<!--
  Exercise 032 — AgeInput component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const age = ref<number>(0);

defineExpose({ age });
</script>

<template>
  <!-- .number runs the typed string through parseFloat before assigning it to
       `age`. Without the modifier this text input would store "42", a string. -->
  <input type="text" v-model.number="age" data-testid="age-input" />
</template>
