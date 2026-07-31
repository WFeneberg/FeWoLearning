<!--
  Exercise 033 — UsernameInput component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const username = ref("");

defineExpose({ username });
</script>

<template>
  <input type="text" v-model.trim="username" data-testid="username-input" />
</template>
