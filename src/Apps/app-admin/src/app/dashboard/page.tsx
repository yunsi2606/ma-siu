import { FileText, Ticket, Eye, TrendingUp } from 'lucide-react'

const stats = [
    { label: 'Tổng bài viết', value: '124', icon: FileText, color: 'bg-blue-500' },
    { label: 'Voucher hoạt động', value: '45', icon: Ticket, color: 'bg-green-500' },
    { label: 'Lượt xem hôm nay', value: '2.4K', icon: Eye, color: 'bg-purple-500' },
    { label: 'Click affiliate', value: '856', icon: TrendingUp, color: 'bg-orange-500' },
]

export default function DashboardPage() {
    return (
        <div>
            <h1 className="text-2xl font-bold text-gray-900 mb-8">Dashboard</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                {stats.map((stat) => (
                    <div key={stat.label} className="bg-white rounded-xl p-6 shadow-sm">
                        <div className="flex items-center gap-4">
                            <div className={`w-12 h-12 ${stat.color} rounded-lg flex items-center justify-center`}>
                                <stat.icon className="text-white" size={24} />
                            </div>
                            <div>
                                <p className="text-sm text-gray-500">{stat.label}</p>
                                <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <div className="bg-white rounded-xl shadow-sm p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">Hoạt động gần đây</h2>
                <div className="text-gray-500 text-center py-8">
                    Chức năng đang được phát triển...
                </div>
            </div>
        </div>
    )
}
