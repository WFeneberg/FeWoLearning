<!-- Exercise 046 — FormValidationBasic component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const email = ref("");
const error = ref("");

function handleSubmit(): void {
  if (email.value.trim() === "") {
    error.value = "Email is required";
    return;
  }
  error.value = "";
}
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <label for="email">Email</label>
    <input id="email" v-model="email" type="text" />
    <button type="submit">Submit</button>
    <p v-if="error" role="alert" class="error">{{ error }}</p>
  </form>
</template>
