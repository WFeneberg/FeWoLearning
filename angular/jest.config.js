module.exports = {
  preset: "jest-preset-angular",
  setupFilesAfterEnv: ["<rootDir>/setup-jest.ts"],
  testMatch: ["<rootDir>/exercises/**/*.spec.ts"],
  moduleFileExtensions: ["ts", "html", "js", "json"],
};
