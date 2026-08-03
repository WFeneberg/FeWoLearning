<!-- Exercise 015 — RadioGroupModel component (beginner). -->
<!-- Goal:   bind a group of radio inputs with v-model to a `choice` ref. -->
<!-- Drills: ref, v-model on <input type="radio">, shared name attribute. -->
<script setup lang="ts">
import { ref } from "vue";

const fruits = ["apple", "banana", "cherry"] as const;

const choice = ref<string>("apple");

// TODO: bind each radio input below to `choice` with v-model (and give
// them a shared `name` so only one can be selected at a time) so that
// clicking a radio updates the status text.
throw new Error("TODO: implement v-model binding on the radio group");
</script>

<template>
  <div>
    <label v-for="fruit in fruits" :key="fruit" :data-testid="`label-${fruit}`">
      <input type="radio" name="fruit" :value="fruit" :data-testid="`radio-${fruit}`" />
      {{ fruit }}
    </label>
    <p data-testid="choice">{{ choice }}</p>
  </div>
</template>
