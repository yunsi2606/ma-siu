'use client'

import { useState } from 'react'
import { Plus, Search, Copy, Check, Clock, AlertTriangle, XCircle } from 'lucide-react'

// Mock data
const vouchers = [
    { id: '1', code: 'SHOPEE50K', platform: 'Shopee', description: 'Giảm 50K cho đơn 200K', remainingPercent: 85, expiresAt: '2026-01-20', usageCount: 156 },
    { id: '2', code: 'LAZADA30', platform: 'Lazada', description: 'Giảm 30% tối đa 100K', remainingPercent: 45, expiresAt: '2026-01-18', usageCount: 234 },
    { id: '3', code: 'TIKTOK25', platform: 'TikTok', description: 'Giảm 25K cho đơn 99K', remainingPercent: 15, expiresAt: '2026-01-17', usageCount: 567 },
    { id: '4', code: 'FREESHIP', platform: 'Shopee', description: 'Miễn phí ship toàn quốc', remainingPercent: 0, expiresAt: '2026-01-15', usageCount: 890 },
]

const platformColors: Record<string, string> = {
    Shopee: 'text-orange-400 bg-orange-500/10',
    Lazada: 'text-blue-400 bg-blue-500/10',
    TikTok: 'text-pink-400 bg-pink-500/10',
}

function getStatusInfo(remainingPercent: number) {
    if (remainingPercent >= 50) return { icon: Clock, color: 'text-emerald-400', bg: 'bg-emerald-500/10', label: 'Còn nhiều' }
    if (remainingPercent >= 20) return { icon: AlertTriangle, color: 'text-amber-400', bg: 'bg-amber-500/10', label: 'Sắp hết' }
    if (remainingPercent > 0) return { icon: AlertTriangle, color: 'text-rose-400', bg: 'bg-rose-500/10', label: 'Gần hết' }
    return { icon: XCircle, color: 'text-slate-400', bg: 'bg-slate-500/10', label: 'Hết hạn' }
}

export default function VouchersPage() {
    const [searchQuery, setSearchQuery] = useState('')
    const [copiedId, setCopiedId] = useState<string | null>(null)

    const copyCode = (id: string, code: string) => {
        navigator.clipboard.writeText(code)
        setCopiedId(id)
        setTimeout(() => setCopiedId(null), 2000)
    }

    return (
        <div className="animate-fade-in">
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-3xl font-bold text-white mb-2">Voucher</h1>
                    <p className="text-slate-400">Quản lý mã giảm giá và theo dõi hiệu suất</p>
                </div>
                <button
                    className="flex items-center gap-2 px-5 py-3 bg-gradient-to-r from-emerald-500 to-cyan-600 text-white rounded-xl font-medium hover:opacity-90 transition-opacity"
                >
                    <Plus size={20} />
                    Thêm voucher
                </button>
            </div>

            {/* Search */}
            <div className="glass-card rounded-xl p-4 mb-6">
                <div className="flex items-center gap-4">
                    <div className="flex-1 relative">
                        <Search size={20} className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" />
                        <input
                            type="text"
                            placeholder="Tìm kiếm mã voucher..."
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
                </div>
            </div>

            {/* Voucher Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {vouchers.map((voucher, index) => {
                    const status = getStatusInfo(voucher.remainingPercent)
                    const StatusIcon = status.icon

                    return (
                        <div
                            key={voucher.id}
                            className="glass-card rounded-xl p-6 animate-fade-in"
                            style={{ animationDelay: `${index * 50}ms` }}
                        >
                            <div className="flex items-start justify-between mb-4">
                                <div>
                                    <div className="flex items-center gap-2 mb-2">
                                        <span className={`px-2 py-1 rounded text-xs font-medium ${platformColors[voucher.platform]}`}>
                                            {voucher.platform}
                                        </span>
                                        <span className={`px-2 py-1 rounded text-xs font-medium flex items-center gap-1 ${status.bg} ${status.color}`}>
                                            <StatusIcon size={12} />
                                            {status.label}
                                        </span>
                                    </div>
                                    <button
                                        onClick={() => copyCode(voucher.id, voucher.code)}
                                        className="flex items-center gap-2 px-4 py-2 bg-slate-800 rounded-lg font-mono text-lg text-white hover:bg-slate-700 transition-colors group"
                                    >
                                        {voucher.code}
                                        {copiedId === voucher.id ? (
                                            <Check size={16} className="text-emerald-400" />
                                        ) : (
                                            <Copy size={16} className="text-slate-500 group-hover:text-white transition-colors" />
                                        )}
                                    </button>
                                </div>
                                <span className="text-sm text-slate-500">{voucher.usageCount} lượt dùng</span>
                            </div>

                            <p className="text-slate-300 mb-4">{voucher.description}</p>

                            {/* Progress bar */}
                            <div className="mb-2">
                                <div className="flex justify-between text-sm mb-1">
                                    <span className="text-slate-400">Thời gian còn lại</span>
                                    <span className={status.color}>{voucher.remainingPercent}%</span>
                                </div>
                                <div className="h-2 bg-slate-800 rounded-full overflow-hidden">
                                    <div
                                        className={`h-full rounded-full transition-all duration-500 ${voucher.remainingPercent >= 50 ? 'bg-emerald-500' :
                                                voucher.remainingPercent >= 20 ? 'bg-amber-500' : 'bg-rose-500'
                                            }`}
                                        style={{ width: `${voucher.remainingPercent}%` }}
                                    />
                                </div>
                            </div>

                            <p className="text-xs text-slate-500">Hết hạn: {voucher.expiresAt}</p>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}
