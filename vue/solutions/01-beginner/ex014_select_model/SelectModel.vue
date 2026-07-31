<!-- Exercise 014 — SelectModel component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const fruits = ["apple", "banana", "cherry"];
const selected = ref(fruits[0]);
</script>

<template>
  <div>
    <select v-model="selected" data-testid="fruit-select">
      <option v-for="fruit in fruits" :key="fruit" :value="fruit">
        {{ fruit }}
      </option>
    </select>
    <p data-testid="status">Selected: {{ selected }}</p>
  </div>
</template>
