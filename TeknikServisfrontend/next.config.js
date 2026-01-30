/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  // Sadece development'ta rewrite kullan
  async rewrites() {
    // Production'da rewrite kullanma, direkt API URL'e istek atılacak
    if (process.env.NODE_ENV === 'production') {
      return [];
    }
    
    // Development'ta localhost'a proxy yap
    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5000/api/:path*',
      },
    ];
  },
};

module.exports = nextConfig;
