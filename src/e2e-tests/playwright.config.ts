import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  projects: [
    {
      name: 'Google Chrome',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    headless: true,
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    baseURL: process.env.BASE_URL || 'http://localhost:8012',
  },
  webServer: {
    command: "cd ../backend/SQLisHard && dotnet run --urls=http://localhost:8012 --environment=Test",
    url: "http://localhost:8012",
    reuseExistingServer: false
  }
}); 