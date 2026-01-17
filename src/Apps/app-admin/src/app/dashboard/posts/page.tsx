'use client'

import { useState } from 'react'
import { Plus, Search, MoreHorizontal, Eye, Edit2, Trash2, ExternalLink } from 'lucide-react'
import Link from 'next/link'

// Mock data
const posts = [
    { id: '1', title: 'Deal Shopee 50% Flash Sale', platform: 'Shopee', status: 'active', views: 1234, clicks: 456, createdAt: '2026-01-17' },
    { id: '2', title: 'Mã giảm giá Lazada 30K', platform: 'Lazada', status: 'active', views: 890, clicks: 234, createdAt: '2026-01-16' },
    { id: '3', title: 'TikTok Shop siêu sale', platform: 'TikTok', status: 'draft', views: 0, clicks: 0, createdAt: '2026-01-15' },
    { id: '4', title: 'Combo deal cuối tuần', platform: 'Shopee', status: 'expired', views: 2345, clicks: 789, createdAt: '2026-01-10' },
]

const platformColors: Record<string, string> = {
    Shopee: 'text-orange-400 bg-orange-500/10',
    Lazada: 'text-blue-400 bg-blue-500/10',
    TikTok: 'text-pink-400 bg-pink-500/10',
}

const statusColors: Record<string, string> = {
    active: 'text-emerald-400 bg-emerald-500/10',
    draft: 'text-slate-400 bg-slate-500/10',
    expired: 'text-rose-400 bg-rose-500/10',
}

export default function PostsPage() {
    const [searchQuery, setSearchQuery] = useState('')

    return (
        <div className="animate-fade-in">
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-3xl font-bold text-white mb-2">Bài viết</h1>
                    <p className="text-slate-400">Quản lý các bài đăng deal và voucher</p>
                </div>
                <Link
                    href="/dashboard/posts/new"
                    className="flex items-center gap-2 px-5 py-3 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl font-medium hover:opacity-90 transition-opacity"
                >
                    <Plus size={20} />
                    Tạo bài viết
                </Link>
            </div>

            {/* Search & Filters */}
            <div className="glass-card rounded-xl p-4 mb-6">
                <div className="flex items-center gap-4">
                    <div className="flex-1 relative">
                        <Search size={20} className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
                        <input
                            type="text"
                            placeholder="Tìm kiếm bài viết..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="w-full pl-12 pr-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                        />
                    </div>
                    <select className="px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white focus:border-blue-500 transition-colors">
                        <option value="">Tất cả platform</option>
                        <option value="shopee">Shopee</option>
                        <option value="lazada">Lazada</option>
                        <option value="tiktok">TikTok</option>
                    </select>
                    <select className="px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white focus:border-blue-500 transition-colors">
                        <option value="">Tất cả trạng thái</option>
                        <option value="active">Đang hoạt động</option>
                        <option value="draft">Bản nháp</option>
                        <option value="expired">Hết hạn</option>
                    </select>
                </div>
            </div>

            {/* Posts Table */}
            <div className="glass-card rounded-xl overflow-hidden">
                <table className="w-full">
                    <thead>
                        <tr className="border-b border-slate-700/50">
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Bài viết</th>
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Platform</th>
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Trạng thái</th>
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Lượt xem</th>
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Clicks</th>
                            <th className="text-left px-6 py-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Ngày tạo</th>
                            <th className="px-6 py-4"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {posts.map((post, index) => (
                            <tr
                                key={post.id}
                                className="border-b border-slate-700/30 hover:bg-slate-800/30 transition-colors animate-fade-in"
                                style={{ animationDelay: `${index * 50}ms` }}
                            >
                                <td className="px-6 py-4">
                                    <p className="font-medium text-white">{post.title}</p>
                                </td>
                                <td className="px-6 py-4">
                                    <span className={`px-3 py-1 rounded-lg text-sm font-medium ${platformColors[post.platform]}`}>
                                        {post.platform}
                                    </span>
                                </td>
                                <td className="px-6 py-4">
                                    <span className={`px-3 py-1 rounded-lg text-sm font-medium ${statusColors[post.status]}`}>
                                        {post.status === 'active' ? 'Hoạt động' : post.status === 'draft' ? 'Nháp' : 'Hết hạn'}
                                    </span>
                                </td>
                                <td className="px-6 py-4 text-slate-300">{post.views.toLocaleString()}</td>
                                <td className="px-6 py-4 text-slate-300">{post.clicks.toLocaleString()}</td>
                                <td className="px-6 py-4 text-slate-400">{post.createdAt}</td>
                                <td className="px-6 py-4">
                                    <div className="flex items-center gap-2">
                                        <button className="p-2 hover:bg-slate-700/50 rounded-lg transition-colors" title="Xem">
                                            <Eye size={16} className="text-slate-400" />
                                        </button>
                                        <button className="p-2 hover:bg-slate-700/50 rounded-lg transition-colors" title="Sửa">
                                            <Edit2 size={16} className="text-slate-400" />
                                        </button>
                                        <button className="p-2 hover:bg-rose-500/20 rounded-lg transition-colors" title="Xóa">
                                            <Trash2 size={16} className="text-rose-400" />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    )
}
