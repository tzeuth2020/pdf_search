/** @type {import('next').NextConfig} */
const nextConfig = {
    output: 'export'
};
export default {
    output: 'standalone',
    async redirects() {
      return [
        {
          source: '/old-path',
          destination: '/new-path',
          permanent: true,
        },
      ];
    },
    async rewrites() {
      return [
        {
          source: '/some-path',
          destination: '/another-path',
        },
      ];
    },
  };

