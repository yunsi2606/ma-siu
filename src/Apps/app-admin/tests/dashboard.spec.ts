import { test, expect } from '@playwright/test';

test.describe('Admin Dashboard', () => {
    // Login before each test
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="text"]', 'admin');
        await page.fill('input[type="password"]', 'admin123');
        await page.click('button[type="submit"]');
        await page.waitForURL(/.*dashboard/);
    });

    test('should display dashboard with stats', async ({ page }) => {
        await expect(page.locator('h1')).toContainText('Tổng quan');
        await expect(page.locator('text=Tổng bài viết')).toBeVisible();
        await expect(page.locator('text=Voucher hoạt động')).toBeVisible();
    });

    test('should navigate to posts page', async ({ page }) => {
        await page.click('a[href="/dashboard/posts"]');
        await expect(page).toHaveURL(/.*posts/);
        await expect(page.locator('h1')).toContainText('Bài viết');
    });

    test('should navigate to vouchers page', async ({ page }) => {
        await page.click('a[href="/dashboard/vouchers"]');
        await expect(page).toHaveURL(/.*vouchers/);
        await expect(page.locator('h1')).toContainText('Voucher');
    });

    test('should navigate to analytics page', async ({ page }) => {
        await page.click('a[href="/dashboard/analytics"]');
        await expect(page).toHaveURL(/.*analytics/);
        await expect(page.locator('h1')).toContainText('Thống kê');
    });

    test('should logout successfully', async ({ page }) => {
        await page.click('text=Đăng xuất');
        await expect(page).toHaveURL(/.*login/);
    });
});
