'use client'

import { useState } from 'react'
import { ArrowLeft, Upload, Link as LinkIcon, Tag, Save, Eye } from 'lucide-react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'

export default function NewPostPage() {
    const router = useRouter()
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        platform: 'shopee',
        affiliateUrl: '',
        voucherCode: '',
        discount: '',
        expiresAt: '',
    })
    const [isSubmitting, setIsSubmitting] = useState(false)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setIsSubmitting(true)

        // TODO: Call API to create post
        await new Promise(resolve => setTimeout(resolve, 1000))

        router.push('/dashboard/posts')
    }

    return (
        <div className="animate-fade-in max-w-4xl">
            {/* Header */}
            <div className="flex items-center gap-4 mb-8">
                <Link
                    href="/dashboard/posts"
                    className="p-2 hover:bg-slate-800/50 rounded-lg transition-colors"
                >
                    <ArrowLeft size={24} className="text-slate-400" />
                </Link>
                <div>
                    <h1 className="text-3xl font-bold text-white">Tạo bài viết mới</h1>
                    <p className="text-slate-400">Đăng deal hoặc voucher mới</p>
                </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
                {/* Basic Info */}
                <div className="glass-card rounded-xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-4">Thông tin cơ bản</h2>

                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">Tiêu đề *</label>
                            <input
                                type="text"
                                value={formData.title}
                                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                                placeholder="VD: Deal Shopee 50% Flash Sale"
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">Mô tả</label>
                            <textarea
                                value={formData.description}
                                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                placeholder="Mô tả chi tiết về deal..."
                                rows={4}
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors resize-none"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">Platform *</label>
                            <div className="flex gap-3">
                                {['shopee', 'lazada', 'tiktok'].map((p) => (
                                    <button
                                        key={p}
                                        type="button"
                                        onClick={() => setFormData({ ...formData, platform: p })}
                                        className={`px-6 py-3 rounded-xl font-medium transition-all ${formData.platform === p
                                                ? p === 'shopee' ? 'bg-orange-500/20 text-orange-400 border-2 border-orange-500/50'
                                                    : p === 'lazada' ? 'bg-blue-500/20 text-blue-400 border-2 border-blue-500/50'
                                                        : 'bg-pink-500/20 text-pink-400 border-2 border-pink-500/50'
                                                : 'bg-slate-800/50 text-slate-400 border-2 border-transparent hover:border-slate-600'
                                            }`}
                                    >
                                        {p.charAt(0).toUpperCase() + p.slice(1)}
                                    </button>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Affiliate & Voucher */}
                <div className="glass-card rounded-xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-4">Link & Voucher</h2>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">
                                <LinkIcon size={14} className="inline mr-2" />
                                Link affiliate *
                            </label>
                            <input
                                type="url"
                                value={formData.affiliateUrl}
                                onChange={(e) => setFormData({ ...formData, affiliateUrl: e.target.value })}
                                placeholder="https://..."
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">
                                <Tag size={14} className="inline mr-2" />
                                Mã voucher
                            </label>
                            <input
                                type="text"
                                value={formData.voucherCode}
                                onChange={(e) => setFormData({ ...formData, voucherCode: e.target.value.toUpperCase() })}
                                placeholder="VD: SHOPEE50K"
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors font-mono"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">Giảm giá</label>
                            <input
                                type="text"
                                value={formData.discount}
                                onChange={(e) => setFormData({ ...formData, discount: e.target.value })}
                                placeholder="VD: 50% hoặc 30K"
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                            />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-slate-300 mb-2">Hết hạn</label>
                            <input
                                type="datetime-local"
                                value={formData.expiresAt}
                                onChange={(e) => setFormData({ ...formData, expiresAt: e.target.value })}
                                className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-colors"
                            />
                        </div>
                    </div>
                </div>

                {/* Image Upload */}
                <div className="glass-card rounded-xl p-6">
                    <h2 className="text-lg font-semibold text-white mb-4">Hình ảnh</h2>
                    <div className="border-2 border-dashed border-slate-700 rounded-xl p-8 text-center hover:border-blue-500/50 transition-colors cursor-pointer">
                        <Upload size={40} className="mx-auto text-slate-500 mb-4" />
                        <p className="text-slate-400 mb-2">Kéo thả hoặc click để upload</p>
                        <p className="text-sm text-slate-500">PNG, JPG tối đa 5MB</p>
                    </div>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-4">
                    <button
                        type="button"
                        className="flex items-center gap-2 px-6 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-slate-300 hover:bg-slate-800 transition-colors"
                    >
                        <Eye size={18} />
                        Xem trước
                    </button>
                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="flex items-center gap-2 px-8 py-3 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl font-medium hover:opacity-90 transition-opacity disabled:opacity-50"
                    >
                        <Save size={18} />
                        {isSubmitting ? 'Đang lưu...' : 'Đăng bài'}
                    </button>
                </div>
            </form>
        </div>
    )
}
