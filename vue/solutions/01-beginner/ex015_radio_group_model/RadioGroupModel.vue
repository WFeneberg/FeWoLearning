<!-- Exercise 015 — RadioGroupModel component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

export const fruits = ["apple", "banana", "cherry"] as const;

const choice = ref<string>("apple");
</script>

<template>
  <div>
    <label v-for="fruit in fruits" :key="fruit" :data-testid="`label-${fruit}`">
      <input
        v-model="choice"
        type="radio"
        name="fruit"
        :value="fruit"
        :data-testid="`radio-${fruit}`"
      />
      {{ fruit }}
    </label>
    <p data-testid="choice">{{ choice }}</p>
  </div>
</template>
