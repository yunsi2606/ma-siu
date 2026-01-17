import NextAuth, { NextAuthOptions } from 'next-auth'
import CredentialsProvider from 'next-auth/providers/credentials'

// Admin credentials (in production, these should be in a database)
const ADMIN_USERS = [
    { id: '1', username: 'admin', password: 'admin123', name: 'Admin', email: 'admin@masiu.vn' },
]

export const authOptions: NextAuthOptions = {
    providers: [
        CredentialsProvider({
            name: 'Credentials',
            credentials: {
                username: { label: 'Username', type: 'text' },
                password: { label: 'Password', type: 'password' }
            },
            async authorize(credentials) {
                if (!credentials?.username || !credentials?.password) {
                    return null
                }

                const user = ADMIN_USERS.find(
                    u => u.username === credentials.username && u.password === credentials.password
                )

                if (user) {
                    return {
                        id: user.id,
                        name: user.name,
                        email: user.email,
                    }
                }
                return null
            }
        })
    ],
    session: {
        strategy: 'jwt',
        maxAge: 24 * 60 * 60, // 24 hours
    },
    pages: {
        signIn: '/login',
    },
    callbacks: {
        async jwt({ token, user }) {
            if (user) {
                token.id = user.id
            }
            return token
        },
        async session({ session, token }) {
            if (session.user) {
                (session.user as any).id = token.id
            }
            return session
        },
    },
}

const handler = NextAuth(authOptions)
export { handler as GET, handler as POST }
