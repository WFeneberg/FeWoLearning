// jest-preset-angular v14 deprecates the old `import "jest-preset-angular/setup-jest"`
// entry point in favour of this explicit call.
import { setupZoneTestEnv } from "jest-preset-angular/setup-env/zone";

setupZoneTestEnv();
