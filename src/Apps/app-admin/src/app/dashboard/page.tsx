import { FileText, Ticket, Eye, TrendingUp, ArrowUpRight, ArrowDownRight } from 'lucide-react'

const stats = [
    {
        label: 'Tổng bài viết',
        value: '1,248',
        change: '+12%',
        trend: 'up',
        icon: FileText,
        gradient: 'from-blue-500 to-blue-600',
        bgClass: 'stat-blue'
    },
    {
        label: 'Voucher hoạt động',
        value: '456',
        change: '+8%',
        trend: 'up',
        icon: Ticket,
        gradient: 'from-emerald-500 to-emerald-600',
        bgClass: 'stat-emerald'
    },
    {
        label: 'Lượt xem hôm nay',
        value: '24.5K',
        change: '-3%',
        trend: 'down',
        icon: Eye,
        gradient: 'from-purple-500 to-purple-600',
        bgClass: 'stat-purple'
    },
    {
        label: 'Click affiliate',
        value: '8,567',
        change: '+24%',
        trend: 'up',
        icon: TrendingUp,
        gradient: 'from-amber-500 to-orange-600',
        bgClass: 'stat-amber'
    },
]

const recentActivity = [
    { action: 'Bài viết mới', title: 'Deal Shopee 50% flash sale', time: '5 phút trước', type: 'post' },
    { action: 'Voucher hết hạn', title: 'LAZADA30K', time: '12 phút trước', type: 'voucher' },
    { action: 'Affiliate click', title: 'Deal TikTok Shop', time: '18 phút trước', type: 'click' },
    { action: 'Bài viết mới', title: 'Mã giảm giá Grab Food', time: '25 phút trước', type: 'post' },
]

export default function DashboardPage() {
    return (
        <div className="animate-fade-in">
            {/* Header */}
            <div className="mb-8">
                <h1 className="text-3xl font-bold text-white mb-2">Tổng quan</h1>
                <p className="text-slate-400">Xem nhanh các số liệu quan trọng của hệ thống</p>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6 mb-8">
                {stats.map((stat, index) => (
                    <div
                        key={stat.label}
                        className="glass-card rounded-2xl p-6 animate-fade-in"
                        style={{ animationDelay: `${index * 100}ms` }}
                    >
                        <div className={`absolute inset-0 rounded-2xl ${stat.bgClass} opacity-50`} />
                        <div className="relative">
                            <div className="flex items-start justify-between mb-4">
                                <div className={`w-12 h-12 rounded-xl bg-gradient-to-br ${stat.gradient} flex items-center justify-center shadow-lg`}>
                                    <stat.icon className="text-white" size={22} />
                                </div>
                                <div className={`flex items-center gap-1 text-sm font-medium ${stat.trend === 'up' ? 'text-emerald-400' : 'text-rose-400'
                                    }`}>
                                    {stat.trend === 'up' ? <ArrowUpRight size={16} /> : <ArrowDownRight size={16} />}
                                    {stat.change}
                                </div>
                            </div>
                            <p className="text-3xl font-bold text-white mb-1">{stat.value}</p>
                            <p className="text-sm text-slate-400">{stat.label}</p>
                        </div>
                    </div>
                ))}
            </div>

            {/* Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Recent Activity */}
                <div className="lg:col-span-2 glass-card rounded-2xl p-6">
                    <div className="flex items-center justify-between mb-6">
                        <h2 className="text-lg font-semibold text-white">Hoạt động gần đây</h2>
                        <button className="text-sm text-blue-400 hover:text-blue-300 transition-colors">
                            Xem tất cả
                        </button>
                    </div>
                    <div className="space-y-4">
                        {recentActivity.map((activity, index) => (
                            <div
                                key={index}
                                className="flex items-center gap-4 p-4 rounded-xl bg-slate-800/30 hover:bg-slate-800/50 transition-colors cursor-pointer group"
                            >
                                <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${activity.type === 'post' ? 'bg-blue-500/20 text-blue-400' :
                                        activity.type === 'voucher' ? 'bg-amber-500/20 text-amber-400' :
                                            'bg-emerald-500/20 text-emerald-400'
                                    }`}>
                                    {activity.type === 'post' && <FileText size={18} />}
                                    {activity.type === 'voucher' && <Ticket size={18} />}
                                    {activity.type === 'click' && <TrendingUp size={18} />}
                                </div>
                                <div className="flex-1 min-w-0">
                                    <p className="text-sm text-slate-400">{activity.action}</p>
                                    <p className="font-medium text-white truncate">{activity.title}</p>
                                </div>
                                <span className="text-xs text-slate-500">{activity.time}</span>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Quick Actions */}
                <div className="glass-card rounded-2xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-6">Thao tác nhanh</h2>
                    <div className="space-y-3">
                        <button className="w-full flex items-center gap-3 p-4 rounded-xl bg-gradient-to-r from-blue-500/10 to-purple-500/10 border border-blue-500/20 hover:border-blue-500/40 transition-all group">
                            <div className="w-10 h-10 rounded-lg bg-blue-500/20 flex items-center justify-center">
                                <FileText size={18} className="text-blue-400" />
                            </div>
                            <div className="text-left">
                                <p className="font-medium text-white group-hover:text-blue-400 transition-colors">Tạo bài viết</p>
                                <p className="text-xs text-slate-500">Đăng deal mới</p>
                            </div>
                        </button>
                        <button className="w-full flex items-center gap-3 p-4 rounded-xl bg-gradient-to-r from-emerald-500/10 to-cyan-500/10 border border-emerald-500/20 hover:border-emerald-500/40 transition-all group">
                            <div className="w-10 h-10 rounded-lg bg-emerald-500/20 flex items-center justify-center">
                                <Ticket size={18} className="text-emerald-400" />
                            </div>
                            <div className="text-left">
                                <p className="font-medium text-white group-hover:text-emerald-400 transition-colors">Thêm voucher</p>
                                <p className="text-xs text-slate-500">Mã giảm giá mới</p>
                            </div>
                        </button>
                        <button className="w-full flex items-center gap-3 p-4 rounded-xl bg-gradient-to-r from-purple-500/10 to-pink-500/10 border border-purple-500/20 hover:border-purple-500/40 transition-all group">
                            <div className="w-10 h-10 rounded-lg bg-purple-500/20 flex items-center justify-center">
                                <BarChart3 size={18} className="text-purple-400" />
                            </div>
                            <div className="text-left">
                                <p className="font-medium text-white group-hover:text-purple-400 transition-colors">Xem báo cáo</p>
                                <p className="text-xs text-slate-500">Analytics chi tiết</p>
                            </div>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    )
}
