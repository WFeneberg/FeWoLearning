// FeWoLearning — Kotlin exercises.
//
// Same layout convention as the java/ track: each exercise's stub and its
// sibling JUnit test live together under exercises/<tier>/exNNN_slug/, so the
// whole `exercises/` tree is registered as the single `test` source set below
// (the `test` source set already depends on `main`, and `main` here is empty).
// `solutions/` is never referenced by any source set, so it is never compiled
// as part of this build; verify a solution by overlaying it onto its stub in
// a throwaway copy instead (see the root CLAUDE.md's "Adding or completing
// exercises" workflow).
//
// Kotlin plugin/coroutines versions below are believed-current as of authoring
// but could NOT be verified — there is no JDK/Gradle/Kotlin on this machine.
// Bump them if Gradle reports they don't exist once a real toolchain is used.
plugins {
    kotlin("jvm") version "2.0.21"
}

repositories {
    mavenCentral()
}

kotlin {
    jvmToolchain(21)
}

sourceSets {
    main {
        kotlin.srcDirs(emptyList<String>())
    }
    test {
        kotlin.srcDir("exercises")
    }
}

dependencies {
    testImplementation(platform("org.junit:junit-bom:5.11.0"))
    testImplementation("org.junit.jupiter:junit-jupiter")
    testRuntimeOnly("org.junit.platform:junit-platform-launcher")

    // Needed by stub signatures throughout the intermediate/advanced/expert
    // tiers (CoroutineScope, Flow, Channel, Mutex, ...), not just by tests.
    testImplementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.9.0")
    // runTest / StandardTestDispatcher / virtual time — used to test
    // delay()-based logic (retry backoff, debounce, ex069) without real waits.
    testImplementation("org.jetbrains.kotlinx:kotlinx-coroutines-test:1.9.0")
}

tasks.test {
    useJUnitPlatform()
}
