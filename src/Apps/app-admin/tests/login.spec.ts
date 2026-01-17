import { test, expect } from '@playwright/test';

test.describe('Admin Login', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
    });

    test('should display login page', async ({ page }) => {
        await expect(page.locator('h1')).toContainText('Mã Siu Admin');
        await expect(page.locator('input[type="text"]')).toBeVisible();
        await expect(page.locator('input[type="password"]')).toBeVisible();
    });

    test('should show error with invalid credentials', async ({ page }) => {
        await page.fill('input[type="text"]', 'wronguser');
        await page.fill('input[type="password"]', 'wrongpass');
        await page.click('button[type="submit"]');

        await expect(page.locator('text=Tên đăng nhập hoặc mật khẩu không đúng')).toBeVisible();
    });

    test('should login successfully with valid credentials', async ({ page }) => {
        await page.fill('input[type="text"]', 'admin');
        await page.fill('input[type="password"]', 'admin123');
        await page.click('button[type="submit"]');

        await expect(page).toHaveURL(/.*dashboard/);
        await expect(page.locator('h1')).toContainText('Tổng quan');
    });

    test('should toggle password visibility', async ({ page }) => {
        const passwordInput = page.locator('input[type="password"]');
        const toggleButton = page.locator('button:has(svg)').last();

        await expect(passwordInput).toHaveAttribute('type', 'password');
        await toggleButton.click();
        await expect(page.locator('input').nth(1)).toHaveAttribute('type', 'text');
    });
});
