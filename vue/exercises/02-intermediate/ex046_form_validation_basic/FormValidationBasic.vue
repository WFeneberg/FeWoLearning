<!-- Exercise 046 — FormValidationBasic component (intermediate). -->
<!-- Goal:   basic form validation with a required-field error message. -->
<!-- Drills: v-model, form submit handling, conditional rendering, computed validation state. -->
<script setup lang="ts">
import { ref } from "vue";

const email = ref("");
const error = ref("");

function handleSubmit(): void {
  throw new Error("TODO: implement handleSubmit (set error when email is empty)");
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
