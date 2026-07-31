<!-- Exercise 089 — RouterDynamicParams component (reference solution). -->
<script setup lang="ts">
import {
  computed,
  h,
  inject,
  provide,
  ref,
  type Component,
  type InjectionKey,
  type Ref,
} from "vue";

export interface RouteParams {
  [key: string]: string;
}

export interface RouteLocation {
  path: string;
  params: RouteParams;
}

interface RouteRecord {
  path: string;
  component: Component;
}

const ROUTE_KEY: InjectionKey<Ref<RouteLocation>> = Symbol("route");

export function useRoute(): Ref<RouteLocation> {
  const route = inject(ROUTE_KEY);
  if (!route) {
    throw new Error("useRoute() was called outside of a RouterDynamicParams tree");
  }
  return route;
}

function matchRoute(pattern: string, path: string): RouteParams | null {
  const patternParts = pattern.split("/").filter(Boolean);
  const pathParts = path.split("/").filter(Boolean);
  if (patternParts.length !== pathParts.length) return null;

  const params: RouteParams = {};
  for (let i = 0; i < patternParts.length; i += 1) {
    const patternPart = patternParts[i]!;
    const pathPart = pathParts[i]!;
    if (patternPart.startsWith(":")) {
      params[patternPart.slice(1)] = pathPart;
    } else if (patternPart !== pathPart) {
      return null;
    }
  }
  return params;
}

const UserProfile: Component = {
  name: "UserProfile",
  setup() {
    const route = useRoute();
    return () => h("p", { class: "user-profile" }, `User #${route.value.params.id}`);
  },
};

const NotFound: Component = {
  name: "NotFound",
  setup() {
    return () => h("p", { class: "not-found" }, "Not found");
  },
};

const routes: RouteRecord[] = [{ path: "/users/:id", component: UserProfile }];

const currentPath = ref("/");

const matched = computed((): { component: Component; params: RouteParams } => {
  for (const record of routes) {
    const params = matchRoute(record.path, currentPath.value);
    if (params) return { component: record.component, params };
  }
  return { component: NotFound, params: {} };
});

const route = computed<RouteLocation>(() => ({
  path: currentPath.value,
  params: matched.value.params,
}));

provide(ROUTE_KEY, route as unknown as Ref<RouteLocation>);

const matchedComponent = computed((): Component => matched.value.component);

function navigate(path: string): void {
  currentPath.value = path;
}

defineExpose({ route, navigate });
</script>

<template>
  <component :is="matchedComponent" />
</template>
