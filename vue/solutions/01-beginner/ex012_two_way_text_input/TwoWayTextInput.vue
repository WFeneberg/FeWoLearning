<!-- Exercise 012 — TwoWayTextInput component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const name = ref("");
</script>

<template>
  <div>
    <input type="text" v-model="name" data-testid="name-input" />
    <p data-testid="greeting">Hello, {{ name }}!</p>
  </div>
</template>
