<!--
  Exercise 003 — UserProfile component (beginner).
  Goal:   a reactive() user object with a nested address.city field.
  Drills: reactive() with nested objects, mutating nested properties,
          template rendering reacting to nested mutations.
-->
<script setup lang="ts">
import { reactive } from "vue";

interface Address {
  city: string;
}

interface User {
  name: string;
  address: Address;
}

const user = reactive<User>({
  name: "Ada",
  address: {
    city: "London",
  },
});

function moveTo(_city: string): void {
  throw new Error("TODO: implement moveTo");
}

defineExpose({ user, moveTo });
</script>

<template>
  <div>
    <p class="name">{{ user.name }}</p>
    <p class="city">{{ user.address.city }}</p>
  </div>
</template>
