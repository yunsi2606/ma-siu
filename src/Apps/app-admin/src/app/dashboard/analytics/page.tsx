'use client'

import { useState } from 'react'
import { TrendingUp, TrendingDown, Eye, MousePointerClick, Users, FileText, Ticket, Calendar } from 'lucide-react'

// Mock data for charts
const dailyStats = [
    { date: '11/01', views: 1200, clicks: 340 },
    { date: '12/01', views: 1450, clicks: 420 },
    { date: '13/01', views: 980, clicks: 280 },
    { date: '14/01', views: 1680, clicks: 520 },
    { date: '15/01', views: 2100, clicks: 680 },
    { date: '16/01', views: 1890, clicks: 590 },
    { date: '17/01', views: 2450, clicks: 780 },
]

const platformStats = [
    { name: 'Shopee', views: 12500, clicks: 3420, color: '#ee4d2d' },
    { name: 'Lazada', views: 8900, clicks: 2100, color: '#0f146d' },
    { name: 'TikTok', views: 6700, clicks: 1850, color: '#ff0050' },
]

const topPosts = [
    { id: '1', title: 'Deal Shopee 50% Flash Sale', views: 4520, clicks: 890, ctr: 19.7 },
    { id: '2', title: 'Mã giảm giá Lazada 100K', views: 3890, clicks: 720, ctr: 18.5 },
    { id: '3', title: 'TikTok Shop siêu sale', views: 2900, clicks: 480, ctr: 16.6 },
    { id: '4', title: 'Combo deal cuối tuần', views: 2450, clicks: 390, ctr: 15.9 },
]

const metrics = [
    { label: 'Tổng lượt xem', value: '28.1K', change: '+12.3%', trend: 'up', icon: Eye },
    { label: 'Tổng clicks', value: '7.4K', change: '+8.7%', trend: 'up', icon: MousePointerClick },
    { label: 'Người dùng mới', value: '1.2K', change: '+5.2%', trend: 'up', icon: Users },
    { label: 'CTR trung bình', value: '26.3%', change: '-1.4%', trend: 'down', icon: TrendingUp },
]

export default function AnalyticsPage() {
    const [timeRange, setTimeRange] = useState('7d')

    const maxViews = Math.max(...dailyStats.map(d => d.views))
    const totalPlatformViews = platformStats.reduce((sum, p) => sum + p.views, 0)

    return (
        <div className="animate-fade-in">
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-3xl font-bold text-white mb-2">Thống kê</h1>
                    <p className="text-slate-400">Phân tích hiệu suất nội dung và affiliate</p>
                </div>
                <div className="flex items-center gap-2">
                    <Calendar size={18} className="text-slate-400" />
                    <select
                        value={timeRange}
                        onChange={(e) => setTimeRange(e.target.value)}
                        className="px-4 py-2 bg-slate-800/50 border border-slate-700 rounded-xl text-white focus:border-blue-500 transition-colors"
                    >
                        <option value="7d">7 ngày qua</option>
                        <option value="30d">30 ngày qua</option>
                        <option value="90d">90 ngày qua</option>
                    </select>
                </div>
            </div>

            {/* Metrics Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                {metrics.map((metric, index) => {
                    const Icon = metric.icon
                    return (
                        <div
                            key={metric.label}
                            className="glass-card rounded-xl p-5 animate-fade-in"
                            style={{ animationDelay: `${index * 50}ms` }}
                        >
                            <div className="flex items-center justify-between mb-3">
                                <div className="w-10 h-10 rounded-lg bg-blue-500/10 flex items-center justify-center">
                                    <Icon size={20} className="text-blue-400" />
                                </div>
                                <span className={`flex items-center gap-1 text-sm font-medium ${metric.trend === 'up' ? 'text-emerald-400' : 'text-rose-400'
                                    }`}>
                                    {metric.trend === 'up' ? <TrendingUp size={14} /> : <TrendingDown size={14} />}
                                    {metric.change}
                                </span>
                            </div>
                            <p className="text-2xl font-bold text-white">{metric.value}</p>
                            <p className="text-sm text-slate-400">{metric.label}</p>
                        </div>
                    )
                })}
            </div>

            {/* Charts Row */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
                {/* Line Chart - Views & Clicks */}
                <div className="lg:col-span-2 glass-card rounded-xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-6">Lượt xem & Clicks theo ngày</h2>
                    <div className="h-64 flex items-end gap-4">
                        {dailyStats.map((day, index) => (
                            <div key={day.date} className="flex-1 flex flex-col items-center gap-2">
                                <div className="w-full flex flex-col gap-1" style={{ height: '200px' }}>
                                    {/* Views bar */}
                                    <div
                                        className="w-full bg-gradient-to-t from-blue-600 to-blue-400 rounded-t-lg transition-all duration-500 hover:opacity-80"
                                        style={{
                                            height: `${(day.views / maxViews) * 100}%`,
                                            animationDelay: `${index * 50}ms`
                                        }}
                                        title={`${day.views} views`}
                                    />
                                    {/* Clicks bar (overlaid) */}
                                    <div
                                        className="w-1/2 mx-auto bg-gradient-to-t from-purple-600 to-purple-400 rounded-t-lg transition-all duration-500 hover:opacity-80 -mt-[100%] relative"
                                        style={{
                                            height: `${(day.clicks / maxViews) * 100}%`,
                                        }}
                                        title={`${day.clicks} clicks`}
                                    />
                                </div>
                                <span className="text-xs text-slate-500">{day.date}</span>
                            </div>
                        ))}
                    </div>
                    <div className="flex items-center justify-center gap-6 mt-4">
                        <div className="flex items-center gap-2">
                            <div className="w-3 h-3 rounded bg-blue-500" />
                            <span className="text-sm text-slate-400">Lượt xem</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <div className="w-3 h-3 rounded bg-purple-500" />
                            <span className="text-sm text-slate-400">Clicks</span>
                        </div>
                    </div>
                </div>

                {/* Platform Distribution */}
                <div className="glass-card rounded-xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-6">Theo nền tảng</h2>
                    <div className="space-y-4">
                        {platformStats.map((platform) => {
                            const percentage = Math.round((platform.views / totalPlatformViews) * 100)
                            return (
                                <div key={platform.name}>
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="font-medium text-white">{platform.name}</span>
                                        <span className="text-sm text-slate-400">{percentage}%</span>
                                    </div>
                                    <div className="h-3 bg-slate-800 rounded-full overflow-hidden">
                                        <div
                                            className="h-full rounded-full transition-all duration-700"
                                            style={{ width: `${percentage}%`, backgroundColor: platform.color }}
                                        />
                                    </div>
                                    <div className="flex justify-between text-xs text-slate-500 mt-1">
                                        <span>{platform.views.toLocaleString()} views</span>
                                        <span>{platform.clicks.toLocaleString()} clicks</span>
                                    </div>
                                </div>
                            )
                        })}
                    </div>
                </div>
            </div>

            {/* Top Posts Table */}
            <div className="glass-card rounded-xl p-6">
                <div className="flex items-center justify-between mb-6">
                    <h2 className="text-lg font-semibold text-white">Bài viết hàng đầu</h2>
                    <button className="text-sm text-blue-400 hover:text-blue-300 transition-colors">
                        Xem tất cả
                    </button>
                </div>
                <table className="w-full">
                    <thead>
                        <tr className="border-b border-slate-700/50">
                            <th className="text-left pb-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Bài viết</th>
                            <th className="text-right pb-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Lượt xem</th>
                            <th className="text-right pb-4 text-xs font-medium text-slate-400 uppercase tracking-wider">Clicks</th>
                            <th className="text-right pb-4 text-xs font-medium text-slate-400 uppercase tracking-wider">CTR</th>
                        </tr>
                    </thead>
                    <tbody>
                        {topPosts.map((post, index) => (
                            <tr
                                key={post.id}
                                className="border-b border-slate-700/30 animate-fade-in"
                                style={{ animationDelay: `${index * 50}ms` }}
                            >
                                <td className="py-4">
                                    <div className="flex items-center gap-3">
                                        <span className="w-6 h-6 rounded-full bg-slate-800 flex items-center justify-center text-xs text-slate-400">
                                            {index + 1}
                                        </span>
                                        <span className="font-medium text-white">{post.title}</span>
                                    </div>
                                </td>
                                <td className="py-4 text-right text-slate-300">{post.views.toLocaleString()}</td>
                                <td className="py-4 text-right text-slate-300">{post.clicks.toLocaleString()}</td>
                                <td className="py-4 text-right">
                                    <span className={`px-2 py-1 rounded text-sm font-medium ${post.ctr >= 18 ? 'text-emerald-400 bg-emerald-500/10' :
                                            post.ctr >= 15 ? 'text-amber-400 bg-amber-500/10' :
                                                'text-slate-400 bg-slate-500/10'
                                        }`}>
                                        {post.ctr}%
                                    </span>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    )
}
