'use client'

import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { signOut, useSession } from 'next-auth/react'
import { LayoutDashboard, FileText, Ticket, BarChart3, LogOut, Settings, ChevronRight } from 'lucide-react'

const navItems = [
    { href: '/dashboard', icon: LayoutDashboard, label: 'Tổng quan' },
    { href: '/dashboard/posts', icon: FileText, label: 'Bài viết' },
    { href: '/dashboard/vouchers', icon: Ticket, label: 'Voucher' },
    { href: '/dashboard/analytics', icon: BarChart3, label: 'Thống kê' },
]

export function Sidebar() {
    const pathname = usePathname()
    const { data: session } = useSession()

    return (
        <aside className="w-72 glass border-r border-slate-700/50 min-h-screen flex flex-col animate-slide-in">
            {/* Logo */}
            <div className="p-6">
                <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center shadow-lg shadow-blue-500/25">
                        <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                        </svg>
                    </div>
                    <div>
                        <h1 className="text-lg font-bold text-white">Mã Siu</h1>
                        <p className="text-xs text-slate-500">Admin Panel</p>
                    </div>
                </div>
            </div>

            {/* Navigation */}
            <nav className="flex-1 px-4 py-2">
                <p className="text-xs font-medium text-slate-500 uppercase tracking-wider px-3 mb-3">Menu</p>
                <ul className="space-y-1">
                    {navItems.map((item, index) => {
                        const isActive = pathname === item.href || (item.href !== '/dashboard' && pathname.startsWith(item.href))
                        return (
                            <li key={item.href} style={{ animationDelay: `${index * 50}ms` }} className="animate-fade-in">
                                <Link
                                    href={item.href}
                                    className={`nav-item flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 group ${isActive
                                            ? 'active text-blue-400'
                                            : 'text-slate-400 hover:text-white hover:bg-slate-800/50'
                                        }`}
                                >
                                    <item.icon size={20} className={isActive ? 'text-blue-400' : 'text-slate-500 group-hover:text-blue-400 transition-colors'} />
                                    <span className="font-medium">{item.label}</span>
                                    {isActive && <ChevronRight size={16} className="ml-auto text-blue-400" />}
                                </Link>
                            </li>
                        )
                    })}
                </ul>

                <div className="mt-8">
                    <p className="text-xs font-medium text-slate-500 uppercase tracking-wider px-3 mb-3">Hệ thống</p>
                    <Link
                        href="/dashboard/settings"
                        className="nav-item flex items-center gap-3 px-4 py-3 rounded-xl text-slate-400 hover:text-white hover:bg-slate-800/50 transition-all duration-200"
                    >
                        <Settings size={20} className="text-slate-500" />
                        <span className="font-medium">Cài đặt</span>
                    </Link>
                </div>
            </nav>

            {/* User */}
            <div className="p-4 border-t border-slate-700/50">
                <div className="glass-card rounded-xl p-4 mb-3">
                    <div className="flex items-center gap-3">
                        {session?.user?.image ? (
                            <img
                                src={session.user.image}
                                alt=""
                                className="w-10 h-10 rounded-full ring-2 ring-blue-500/30"
                            />
                        ) : (
                            <div className="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                                <span className="text-white font-bold text-sm">
                                    {session?.user?.name?.charAt(0) || 'A'}
                                </span>
                            </div>
                        )}
                        <div className="flex-1 min-w-0">
                            <p className="font-medium text-white truncate text-sm">{session?.user?.name || 'Admin'}</p>
                            <p className="text-xs text-slate-500 truncate">{session?.user?.email}</p>
                        </div>
                    </div>
                </div>
                <button
                    onClick={() => signOut({ callbackUrl: '/login' })}
                    className="flex items-center gap-2 w-full px-4 py-2.5 text-slate-400 hover:text-rose-400 hover:bg-rose-500/10 rounded-xl transition-all duration-200"
                >
                    <LogOut size={18} />
                    <span className="font-medium text-sm">Đăng xuất</span>
                </button>
            </div>
        </aside>
    )
}
