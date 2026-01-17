import { test, expect } from '@playwright/test';

test.describe('Posts Management', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/login');
        await page.fill('input[type="text"]', 'admin');
        await page.fill('input[type="password"]', 'admin123');
        await page.click('button[type="submit"]');
        await page.waitForURL(/.*dashboard/);
        await page.click('a[href="/dashboard/posts"]');
    });

    test('should display posts list', async ({ page }) => {
        await expect(page.locator('h1')).toContainText('Bài viết');
        await expect(page.locator('table')).toBeVisible();
    });

    test('should have search functionality', async ({ page }) => {
        const searchInput = page.locator('input[placeholder*="Tìm kiếm"]');
        await expect(searchInput).toBeVisible();
        await searchInput.fill('Shopee');
        // Search should filter results
    });

    test('should navigate to new post page', async ({ page }) => {
        await page.click('text=Tạo bài viết');
        await expect(page).toHaveURL(/.*posts\/new/);
        await expect(page.locator('h1')).toContainText('Tạo bài viết mới');
    });

    test('should display new post form fields', async ({ page }) => {
        await page.click('text=Tạo bài viết');

        await expect(page.locator('input[placeholder*="Tiêu đề"]')).toBeVisible();
        await expect(page.locator('textarea')).toBeVisible();
        await expect(page.locator('text=Shopee')).toBeVisible();
        await expect(page.locator('text=Lazada')).toBeVisible();
        await expect(page.locator('text=TikTok')).toBeVisible();
    });
});
