<!--
  Exercise 003 — UserProfile component (reference solution).
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

function moveTo(city: string): void {
  user.address.city = city;
}

defineExpose({ user, moveTo });
</script>

<template>
  <div>
    <p class="name">{{ user.name }}</p>
    <p class="city">{{ user.address.city }}</p>
  </div>
</template>
